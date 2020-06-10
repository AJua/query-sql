using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.ML.Data;
using Microsoft.ML;
using System.Collections.Generic;

namespace query_sql
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var protocol = Environment.GetEnvironmentVariable("ELASTIC_PROTOCOL");
            var user = Environment.GetEnvironmentVariable("ELASTIC_USER");
            var password = Environment.GetEnvironmentVariable("ELASTIC_PASSWORD");
            var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");

            var uri = new Uri($"{protocol}://{user}:{password}@{host}:{port}");
            var settings = new ConnectionSettings(uri)
                .DefaultIndex("metric");
            var client = new ElasticClient(settings);
            //var res = client.Sql.Query(x => x.Query("SELECT article_title FROM ptt LIMIT 10"));
            //foreach (var row in res.Rows)
            //{
            //    Console.WriteLine(row[0].As<string>());
            //}

            //var res = client.Sql.Query(x => x.Query("SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c FROM ptt GROUP BY h;"));
            var res = client.Sql.Query(x => x.Query(
                //"SELECT HISTOGRAM(timestamp, INTERVAL 30 MINUTE) AS h, COUNT(1) AS c " +
                "SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c " +
                "FROM ptt " +
                "WHERE timestamp > CAST('2020-06-01' AS DATE) " +
                "GROUP BY h"
            ));

            Console.WriteLine($"time | count");
            foreach (var row in res.Rows)
            {
                Console.WriteLine($"{row[0].As<DateTime>()} | {row[1].As<int>()} ");
                //client.IndexDocument(new Metric()
                //{
                //    Id = row[0].As<DateTime>().ToLongTimeString(),
                //    Time = row[0].As<DateTime>(),
                //    Count = row[1].As<int>()
                //}
                //);
            }
            //Console.WriteLine(JsonConvert.SerializeObject(res.Rows.Select(row => new Metric()
            //{
            //    Time = row[0].As<DateTime>(),
            //    Count = row[1].As<int>()
            //})));

            var time_series = res.Rows.Select(row => new Metric()
            {
                Time = row[0].As<DateTime>(),
                Count = row[1].As<float>()
            }).ToList();

            var docSize = time_series.Count();
            MLContext mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(time_series);
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(MetricPrediction.Prediction), inputColumnName: nameof(Metric.Count), confidence: 95, changeHistoryLength: 3);
            //var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(MetricPrediction.Prediction), inputColumnName: nameof(Metric.Count), confidence: 95, changeHistoryLength: docSize / 12);
            var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));
            IDataView transformedData = iidChangePointTransform.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<MetricPrediction>(transformedData, reuseRowObject: false);
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");

            int i = 0;
            foreach (var p in predictions)
            {
                time_series[i].IsAlert = 0;
                time_series[i].Martingale = p.Prediction[3] / 100;
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";
                if (p.Prediction[0] == 1)
                {
                    time_series[i].IsAlert = 1;
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
                i++;
            }

            var indexManyResponse = client.IndexMany(time_series);
            if (indexManyResponse.Errors)
            {
                // the response can be inspected for errors
                foreach (var itemWithError in indexManyResponse.ItemsWithErrors)
                {
                    // if there are errors, they can be enumerated and inspected
                    Console.WriteLine("Failed to index document {0}: {1}",
                        itemWithError.Id, itemWithError.Error);
                }
            }
        }

        static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<Metric> enumerableData = new List<Metric>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }

        public class Metric
        {
            public string Id { get; set; }
            public DateTime Time { get; set; }
            public float Count { get; set; }
            public int IsAlert { get; set; }
            public double Martingale { get; set; }
        }

        public class MetricPrediction
        {
            [VectorType(3)]
            public double[] Prediction { get; set; }
        }
    }
}
