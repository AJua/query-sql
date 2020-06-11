using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.ML.Data;
using Microsoft.ML;
using System.Collections.Generic;
using Microsoft.ML.Transforms.TimeSeries;

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

            //var res = client.Sql.Query(x => x.Query("SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c FROM ptt GROUP BY h;"));
            var res = client.Sql.Query(x => x.Query(
                //"SELECT HISTOGRAM(timestamp, INTERVAL 30 MINUTE) AS h, COUNT(1) AS c " +
                "SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c " +
                "FROM ptt " +
                "WHERE timestamp > CAST('2020-05-30' AS DATE) AND  timestamp < CAST('2020-06-12' AS DATE) " +
                "GROUP BY h"
            ));

            Console.WriteLine($"time | count");
            foreach (var row in res.Rows)
            {
                Console.WriteLine($"{row[0].As<DateTime>()} | {row[1].As<float>()} ");
                //client.IndexDocument(new Metric()
                //{
                //    Id = row[0].As<DateTime>().ToLongTimeString(),
                //    Time = row[0].As<DateTime>(),
                //    Count = row[1].As<int>()
                //}
                //);
            }

            var time_series = res.Rows.Select(row => new Metric()
            {
                Time = row[0].As<DateTime>(),
                Count = row[1].As<float>()
            }).ToList();

            var docSize = time_series.Count();
            MLContext mlContext = new MLContext(0);
            var dataView = mlContext.Data.LoadFromEnumerable(time_series.Select(x => new TaxiTrip(x.Time, x.Count)));
            var pipeline = mlContext.Transforms.Concatenate("Features", "Is0100", "Is0200", "Is0300", "Is0400", "Is0500", "Is0600", "Is0700", "Is0800", "Is0900", "Is1000", "Is1100", "Is1200", "Is1300", "Is1400", "Is1500", "Is1600", "Is1700", "Is1800", "Is1900", "Is2000", "Is2100", "Is2200", "Is2300")
                .Append(mlContext.Regression.Trainers.Sdca());

            var model = pipeline.Fit(dataView);
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine();

            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);
            foreach (var metric in time_series)
            {
                Console.WriteLine(JsonConvert.SerializeObject(predictionFunction.Predict(new TaxiTrip(metric.Time, 0))));
                metric.Predict = predictionFunction.Predict(new TaxiTrip(metric.Time, 0)).FareAmount;
            };

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

        public class Metric
        {
            public string Id { get; set; }
            public DateTime Time { get; set; }
            public float Count { get; set; }
            public float Predict { get; internal set; }
        }

        public class TaxiTrip
        {
            private DateTime _time;

            public TaxiTrip(DateTime time, float count)
            {
                _time = time;
                FareAmount = count;
            }
            [ColumnName("Label")]
            public float FareAmount { get; }
            //public float Is0000 => _time.ToString("HH:mm") == "00:00"? 1:0;
            public float Is0100 => _time.ToString("HH:mm") == "01:00" ? 1 : 0;
            public float Is0200 => _time.ToString("HH:mm") == "02:00" ? 1 : 0;
            public float Is0300 => _time.ToString("HH:mm") == "03:00" ? 1 : 0;
            public float Is0400 => _time.ToString("HH:mm") == "04:00" ? 1 : 0;
            public float Is0500 => _time.ToString("HH:mm") == "05:00" ? 1 : 0;
            public float Is0600 => _time.ToString("HH:mm") == "06:00" ? 1 : 0;
            public float Is0700 => _time.ToString("HH:mm") == "07:00" ? 1 : 0;
            public float Is0800 => _time.ToString("HH:mm") == "08:00" ? 1 : 0;
            public float Is0900 => _time.ToString("HH:mm") == "09:00" ? 1 : 0;
            public float Is1000 => _time.ToString("HH:mm") == "10:00" ? 1 : 0;
            public float Is1100 => _time.ToString("HH:mm") == "11:00" ? 1 : 0;
            public float Is1200 => _time.ToString("HH:mm") == "12:00" ? 1 : 0;
            public float Is1300 => _time.ToString("HH:mm") == "13:00" ? 1 : 0;
            public float Is1400 => _time.ToString("HH:mm") == "14:00" ? 1 : 0;
            public float Is1500 => _time.ToString("HH:mm") == "15:00" ? 1 : 0;
            public float Is1600 => _time.ToString("HH:mm") == "16:00" ? 1 : 0;
            public float Is1700 => _time.ToString("HH:mm") == "17:00" ? 1 : 0;
            public float Is1800 => _time.ToString("HH:mm") == "18:00" ? 1 : 0;
            public float Is1900 => _time.ToString("HH:mm") == "19:00" ? 1 : 0;
            public float Is2000 => _time.ToString("HH:mm") == "20:00" ? 1 : 0;
            public float Is2100 => _time.ToString("HH:mm") == "21:00" ? 1 : 0;
            public float Is2200 => _time.ToString("HH:mm") == "22:00" ? 1 : 0;
            public float Is2300 => _time.ToString("HH:mm") == "23:00" ? 1 : 0;

        }

        public class TaxiTripFarePrediction
        {
            [ColumnName("Score")]
            public float FareAmount { get; set; }
        }
    }
}
