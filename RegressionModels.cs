using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace query_sql
{
    public class ProductSalesData
    {
        public DateTime Month;

        public float numSales;
    }

    public class ProductSalesPrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
    internal class RegressionModels
    {
        public static IEnumerable<dynamic> HourlyModel(IEnumerable<MetricWithRegression> timeSeries)
        {
            var mlContext = new MLContext(0);
            var dataView = mlContext.Data.LoadFromEnumerable(timeSeries.Select(x => new TaxiTrip(x.Time, x.Count)));
            var pipeline = mlContext.Transforms.Concatenate("Features", "Is0100", "Is0200", "Is0300", "Is0400", "Is0500", "Is0600", "Is0700", "Is0800", "Is0900", "Is1000", "Is1100", "Is1200", "Is1300", "Is1400", "Is1500", "Is1600", "Is1700", "Is1800", "Is1900", "Is2000", "Is2100", "Is2200", "Is2300")
                .Append(mlContext.Regression.Trainers.Sdca());

            var model = pipeline.Fit(dataView);
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine();

            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);

            return timeSeries.Select(x => new
            {
                x.Time,
                x.Count,
                HourlyPredict2 = predictionFunction.Predict(new TaxiTrip(x.Time, 0)).FareAmount
            }
            );
        }
        public static IEnumerable<dynamic> HourlyAndDayModel(IEnumerable<MetricWithRegression> timeSeries)
        {
            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(timeSeries.Select(x => new TaxiTrip(x.Time, x.Count)));
            var pipeline = mlContext.Transforms.Concatenate(
                    "Features",
                    "Is2", "Is3", "Is4", "Is5", "Is6", "Is7", "Is8", "Is9", "Is10", "Is11", "Is12", "Is13", "Is14", "Is15", "Is16", "Is17", "Is18", "Is19", "Is20", "Is21", "Is22", "Is23", "Is24", "Is25", "Is26", "Is27", "Is28", "Is29", "Is30", "Is31",
                    "Is0100", "Is0200", "Is0300", "Is0400", "Is0500", "Is0600", "Is0700", "Is0800", "Is0900", "Is1000", "Is1100", "Is1200", "Is1300", "Is1400", "Is1500", "Is1600", "Is1700", "Is1800", "Is1900", "Is2000", "Is2100", "Is2200", "Is2300")
                .Append(mlContext.Regression.Trainers.Sdca());

            var model = pipeline.Fit(dataView);
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine();

            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);

            return timeSeries.Select(x => new
            {
                x.Time,
                x.Count,
                HourlyAndDayPredict2 = predictionFunction.Predict(new TaxiTrip(x.Time, 0)).FareAmount
            }
            );
        }

        static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<ProductSalesData> enumerableData = new List<ProductSalesData>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }

        public static IEnumerable<dynamic> DetectChangePoint(IEnumerable<TaxiTrip> metrics, string query)
        {
            var mlContext = new MLContext();
            var dataView = mlContext.Data.LoadFromEnumerable(metrics.Select(x => new ProductSalesData(){Month = x.Time, numSales = x.FareAmount}));
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(nameof(ProductSalesPrediction.Prediction), nameof(ProductSalesData.numSales), confidence: 95, changeHistoryLength: metrics.Count()/50);
            var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));
            var transformedData = iidChangePointTransform.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");
            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
            }
            Console.WriteLine("");
            return metrics.Zip(predictions, (x, y) => new
            {
                Id = $"{query}_{nameof(DetectChangePoint)} {x.Time.Ticks}", x.Time, Value = x.FareAmount, IsAlert3 = y.Prediction[0]
            });
        }
    }
}