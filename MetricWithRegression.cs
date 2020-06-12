using System;

namespace query_sql
{
    public class MetricWithRegression
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public float Count { get; set; }
        public float HourlyPredict { get; internal set; }
        public float HourlyAndDayPredict { get; internal set; }
    }
}
