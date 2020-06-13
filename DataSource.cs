using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Nest;
using Newtonsoft.Json;

namespace query_sql
{
    public class DataSource
    {
        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
        }

        public static IEnumerable<MetricWithRegression> QuerySqlElasticsearch(Arguments args)
        {
            var protocol = Environment.GetEnvironmentVariable("ELASTIC_PROTOCOL");
            var user = Environment.GetEnvironmentVariable("ELASTIC_USER");
            var password = Environment.GetEnvironmentVariable("ELASTIC_PASSWORD");
            var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");

            var uri = new Uri($"{protocol}://{user}:{password}@{host}:{port}");
            var settings = new ConnectionSettings(uri).DefaultIndex("metric");

            var client = new ElasticClient(settings);

            var res = client.Sql.Query(x => x.Query(
                $"SELECT HISTOGRAM(timestamp, INTERVAL {args.Interval}) AS h, COUNT(1) AS c FROM {args.Index} " +
                $"WHERE timestamp > CAST('{DateTime.UtcNow.AddHours(-args.OffSet):yyyy-MM-ddTHH:mm:ssZ}' AS DATE) AND timestamp < CAST('{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}' AS DATE) GROUP BY h"));
            Console.WriteLine($"time | count");
            foreach (var row in res.Rows)
            {
                Console.WriteLine($"{row[0].As<DateTime>()} | {row[1].As<float>()} ");
            }
            return res.Rows.Select(row => new MetricWithRegression()
            {
                Time = row[0].As<DateTime>(),
                Count = row[1].As<float>()
            });
        }

        public static IEnumerable<TaxiTrip> QueryPrometheus(Arguments arguments)
        {
            var pProtocol = Environment.GetEnvironmentVariable("PROMETHEUS_PROTOCOL");
            var pUser = Environment.GetEnvironmentVariable("PROMETHEUS_USER");
            var pPassword = Environment.GetEnvironmentVariable("PROMETHEUS_PASSWORD");
            var pHost = Environment.GetEnvironmentVariable("PROMETHEUS_HOST");
            var pPort = Environment.GetEnvironmentVariable("PROMETHEUS_PORT");

            var request = (HttpWebRequest)WebRequest.Create($"{pProtocol}://{pHost}:{pPort}/api/v1/query_range?query={arguments.Query}&start={DateTime.UtcNow.AddHours(-arguments.OffSet):yyyy-MM-ddTHH:mm:ssZ}&end={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}&step={arguments.Step}");

            var encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(pUser + ":" + pPassword));
            request.Headers.Add("Authorization", "Basic " + encoded);
            var resp = request.GetResponse();
            var temp = JsonConvert.DeserializeObject<PrometheusMetric>(new StreamReader(resp.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd());
            var queryResult = temp.data.result[0].values.Select(x =>
                new TaxiTrip(UnixTimestampToDateTime(Convert.ToDouble(x[0])), Convert.ToInt32(x[1])));

            Console.WriteLine(queryResult.OrderBy(x => x.Time).First().Time);
            Console.WriteLine(queryResult.OrderByDescending(x => x.Time).First().Time);

            return queryResult;
        }
    }
}