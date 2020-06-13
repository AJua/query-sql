using System;

namespace query_sql
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var ts = DataSource.QueryPrometheus("query=up&start=2020-06-10T20:10:30.781Z&end=2020-06-13T20:11:00.781Z&step=175s");

            var metrics = DataSource.QuerySqlElasticsearch(
                "SELECT HISTOGRAM(timestamp, INTERVAL 1 HOUR) AS h, COUNT(1) AS c " +
                "FROM ptt " +
                "WHERE timestamp > CAST('2020-05-30' AS DATE) AND timestamp < CAST('2020-06-13' AS DATE) " +
                "GROUP BY h"
            );

            var timeSeries1 = RegressionModels.HourlyModel(metrics);
            DataStorage.Index(timeSeries1);

            var timeSeries2 = RegressionModels.HourlyAndDayModel(metrics);
            DataStorage.Index(timeSeries2);

        }
    }
}
