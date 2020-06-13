namespace query_sql
{
    partial class Program
    {
        static void Main(string[] rawArgs)
        {
            var args = new Arguments(rawArgs);

            args.DataSource = "elasticsearch";

            switch (args.DataSource.ToLower())
            {
                case "prometheus":
                {
                    var metrics0 = DataSource.QueryPrometheus(args);
                    //var timeSeries0 = RegressionModels.DetectChangePoint(metrics0, args.Query);
                    //DataStorage.Index(timeSeries0);
                    break;
                }
                case "elasticsearch":
                {
                    var metrics = DataSource.QuerySqlElasticsearch(args);

                    //var timeSeries1 = RegressionModels.HourlyModel(metrics);
                    //DataStorage.Index(timeSeries1);

                    var timeSeries2 = RegressionModels.HourlyAndDayModel(metrics);
                    DataStorage.Index(timeSeries2);
                    break;
                }
            }
        }


    }
}
