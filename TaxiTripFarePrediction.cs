using Microsoft.ML.Data;

namespace query_sql
{
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount { get; set; }
    }
}
