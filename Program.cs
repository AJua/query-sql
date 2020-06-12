using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.ML;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace query_sql
{
    partial class Program
    {
        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        private static IEnumerable<TaxiTrip> QueryPrometheus()
        {
            var pProtocol = Environment.GetEnvironmentVariable("PROMETHEUS_PROTOCOL");
            var pUser = Environment.GetEnvironmentVariable("PROMETHEUS_USER");
            var pPassword = Environment.GetEnvironmentVariable("PROMETHEUS_PASSWORD");
            var pHost = Environment.GetEnvironmentVariable("PROMETHEUS_HOST");
            var pPort = Environment.GetEnvironmentVariable("PROMETHEUS_PORT");

            var request = (HttpWebRequest)WebRequest.Create($"{pProtocol}://{pHost}:{pPort}/api/v1/query_range?query=up&start=2020-06-10T20:10:30.781Z&end=2020-06-13T20:11:00.781Z&step=175s");

            string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(pUser + ":" + pPassword));
            request.Headers.Add("Authorization", "Basic " + encoded);
            var resp = request.GetResponse();
            var temp = JsonConvert.DeserializeObject<PrometheusMetric>(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            Console.WriteLine(JsonConvert.SerializeObject(temp.data.result[0].values.Select(x =>
                new TaxiTrip(UnixTimestampToDateTime(Convert.ToDouble(x[0])), Convert.ToInt32(x[1]))))
            );
            return temp.data.result[0].values.Select(x =>
                new TaxiTrip(UnixTimestampToDateTime(Convert.ToDouble(x[0])), Convert.ToInt32(x[1])));
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            QueryPrometheus();

            //var protocol = Environment.GetEnvironmentVariable("ELASTIC_PROTOCOL");
            //var user = Environment.GetEnvironmentVariable("ELASTIC_USER");
            //var password = Environment.GetEnvironmentVariable("ELASTIC_PASSWORD");
            //var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            //var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");

            //var uri = new Uri($"{protocol}://{user}:{password}@{host}:{port}");
            //var settings = new ConnectionSettings(uri)
            //    .DefaultIndex("metric");
            //var client = new ElasticClient(settings);

            ////var res = client.Sql.Query(x => x.Query("SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c FROM ptt GROUP BY h;"));
            //var res = client.Sql.Query(x => x.Query(
            //    //"SELECT HISTOGRAM(timestamp, INTERVAL 30 MINUTE) AS h, COUNT(1) AS c " +
            //    "SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c " +
            //    "FROM ptt " +
            //    "WHERE timestamp > CAST('2020-05-30' AS DATE) AND  timestamp < CAST('2020-06-13' AS DATE) " +
            //    "GROUP BY h"
            //));

            //Console.WriteLine($"time | count");
            //foreach (var row in res.Rows)
            //{
            //    Console.WriteLine($"{row[0].As<DateTime>()} | {row[1].As<float>()} ");
            //}

            //var time_series = HourlyModel(res);
            //time_series = HourlyAndDayModel(res);

            //var indexManyResponse = client.IndexMany(time_series);
            //if (indexManyResponse.Errors)
            //{
            //    // the response can be inspected for errors
            //    foreach (var itemWithError in indexManyResponse.ItemsWithErrors)
            //    {
            //        // if there are errors, they can be enumerated and inspected
            //        Console.WriteLine("Failed to index document {0}: {1}",
            //            itemWithError.Id, itemWithError.Error);
            //    }
            //}
        }

        private static List<MetricWithRegression> HourlyModel(QuerySqlResponse res)
        {
            var time_series = res.Rows.Select(row => new MetricWithRegression()
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
                metric.HourlyPredict = predictionFunction.Predict(new TaxiTrip(metric.Time, 0)).FareAmount;
            };
            return time_series;
        }
        private static List<MetricWithRegression> HourlyAndDayModel(QuerySqlResponse res)
        {
            var time_series = res.Rows.Select(row => new MetricWithRegression()
            {
                Time = row[0].As<DateTime>(),
                Count = row[1].As<float>()
            }).ToList();

            var docSize = time_series.Count();
            MLContext mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(time_series.Select(x => new TaxiTrip(x.Time, x.Count)));
            var pipeline = mlContext.Transforms.Concatenate(
                "Features",
                "Is2", "Is3", "Is4", "Is5", "Is6", "Is7", "Is8", "Is9", "Is10", "Is11", "Is12", "Is13", "Is14", "Is15", "Is16", "Is17", "Is18", "Is19", "Is20", "Is21", "Is22", "Is23", "Is24", "Is25", "Is26", "Is27", "Is28", "Is29", "Is30", "Is31",
                "Is0100", "Is0200", "Is0300", "Is0400", "Is0500", "Is0600", "Is0700", "Is0800", "Is0900", "Is1000", "Is1100", "Is1200", "Is1300", "Is1400", "Is1500", "Is1600", "Is1700", "Is1800", "Is1900", "Is2000", "Is2100", "Is2200", "Is2300")
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
                metric.HourlyAndDayPredict = predictionFunction.Predict(new TaxiTrip(metric.Time, 0)).FareAmount;
            };
            return time_series;
        }
    }
}
