using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace query_sql
{

    public class Metric
    {
        public string __name__ { get; set; }
        public string instance { get; set; }
        public string job { get; set; }
    }

    public class Result
    {
        public PrometheusMetric metric { get; set; }
        public IList<IList<string>> values { get; set; }
        //public DateTime Time() => DateTime.Parse(values[0]);
        //public int Value() => Convert.ToInt32(values[1]);
    }

    public class Data
    {
        public string resultType { get; set; }
        public IList<Result> result { get; set; }
    }

    public class PrometheusMetric
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
}
