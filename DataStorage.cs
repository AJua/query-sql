using System;
using System.Collections.Generic;
using Nest;

namespace query_sql
{
    internal class DataStorage
    {
        public static void Index(IEnumerable<dynamic> time_series)
        {
            var protocol = Environment.GetEnvironmentVariable("ELASTIC_PROTOCOL");
            var user = Environment.GetEnvironmentVariable("ELASTIC_USER");
            var password = Environment.GetEnvironmentVariable("ELASTIC_PASSWORD");
            var host = Environment.GetEnvironmentVariable("ELASTIC_HOST");
            var port = Environment.GetEnvironmentVariable("ELASTIC_PORT");

            var uri = new Uri($"{protocol}://{user}:{password}@{host}:{port}");
            var settings = new ConnectionSettings(uri)
                .DefaultIndex("metric");
            var client = new ElasticClient(settings);
            var indexManyResponse = client.IndexMany(time_series);
            if (!indexManyResponse.Errors) return;
            // the response can be inspected for errors
            foreach (var itemWithError in indexManyResponse.ItemsWithErrors)
            {
                // if there are errors, they can be enumerated and inspected
                Console.WriteLine("Failed to index document {0}: {1}",
                    itemWithError.Id, itemWithError.Error);
            }
        }
    }
}