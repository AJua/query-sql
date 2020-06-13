using System;
using System.Linq;
using Microsoft.ML;

namespace query_sql
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var query = "process_virtual_memory_bytes";

            var metrics0 = DataSource.QueryPrometheus($"query={query}&start=2020-06-12T00:00:00.781Z&end=2020-06-14T20:11:00.781Z&step=60s");
            var timeSeries0 = RegressionModels.DetectChangePoint(metrics0, query);

            DataStorage.Index(timeSeries0);
            //DataStorage.Index(ts.Select(x => new { x.Time, x.FareAmount }));

            //var metrics = DataSource.QuerySqlElasticsearch(
            //    "SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c " +
            //    "FROM ptt " +
            //    "WHERE timestamp > CAST('2020-05-30' AS DATE) AND timestamp < CAST('2020-06-13' AS DATE) " +
            //    "GROUP BY h"
            //);

            //var timeSeries1 = RegressionModels.HourlyModel(metrics);
            //DataStorage.Index(timeSeries1);

            //var timeSeries2 = RegressionModels.HourlyAndDayModel(metrics);
            //DataStorage.Index(timeSeries2);

        }
    }
}
