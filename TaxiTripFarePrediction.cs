using Microsoft.ML.Data;

namespace query_sql
{
    partial class Program
    {
        public class TaxiTripFarePrediction
        {
            [ColumnName("Score")]
            public float FareAmount { get; set; }
        }
    }
}
