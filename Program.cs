using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;

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
                "SELECT HISTOGRAM(timestamp, INTERVAL 30 MINUTE) AS h, COUNT(1) AS c " +
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
                Count = row[1].As<int>()
            });
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
            public int Count { get; set; }
        }
    }
}
