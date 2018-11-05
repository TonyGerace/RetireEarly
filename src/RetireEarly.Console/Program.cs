using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eviCore.Registration.AutoRegistration;
using MarketData;
using MarketData.TdAmeritrade;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.Data.IO;
using Microsoft.ML.Runtime.Learners;
using Microsoft.ML.StaticPipe;
using Microsoft.ML.StaticPipe.Runtime;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Categorical;


namespace RetireEarly
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockTicks =
                CreateStockTicks()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();

            foreach (var tick in stockTicks)
            {
                Console.WriteLine($"Open: {tick.Open} Close: {tick.Close}");
            }

            var today = stockTicks[stockTicks.Count - 1];

            stockTicks.RemoveAt(stockTicks.Count - 1);

            // Create a new context for ML.NET operations. It can be used for exception tracking and logging, 
            // as a catalog of available operations and as the source of randomness.
            var mlContext = new MLContext();

            // Step one: read the data as an IDataView.
            // Let's assume that 'GetChurnData()' fetches and returns the training data from somewhere.
            IList<StockTick> stockTickData = stockTicks;

            // Turn the data into the ML.NET data view.
            // We can use CreateDataView or CreateStreamingDataView, depending on whether 'churnData' is an IList, 
            // or merely an IEnumerable.
            var trainData = mlContext.CreateDataView(stockTickData);


            var staticData = trainData
                .AssertStatic(mlContext, c => (
                    Close: c.R4.Scalar,
                    Open: c.R4.Scalar,
                    High: c.R4.Scalar,
                    Low: c.R4.Scalar,
                    Volume: c.R4.Scalar,
                    Year: c.R4.Scalar,
                    Month: c.R4.Scalar,
                    Day: c.R4.Scalar));


            var staticDataPipeline =
                staticData
                    .MakeNewEstimator()
                    .Append(r => (
                        Close: r.Close,
                        Features: r.Open
                            .ConcatWith(r.Low)
                            .ConcatWith(r.High)
                            .ConcatWith(r.Volume)
                            .ConcatWith(r.Year)
                            .ConcatWith(r.Month)
                            .ConcatWith(r.Day)))
                    .Append(r =>
                        mlContext
                            .Regression
                            .Trainers
                            .FastTree(r.Close, r.Features, null, stockTickData.Count, stockTickData.Count, 10, 0.2D, null, null));

            var staticModel = staticDataPipeline.Fit(staticData).AsDynamic;

            var predictionEngine = staticModel.MakePredictionFunction<StockTick, StockTickPredication>(mlContext);

            var prediction = predictionEngine.Predict(new StockTick()
            {
                Open = today.Open,
                High = today.High,
                Low = today.Low,
                Volume = today.Volume,
                Day = today.Day,
                Month = today.Month,
                Year = today.Year

            });

            Console.WriteLine("==============");
            Console.WriteLine($"Open: {today.Open}");

            Console.WriteLine("Close Prediction  -  {0}", prediction.Score);
            Console.WriteLine("Close Actual      -  {0}", today.Close);

            Console.ReadLine();
        }


        private static async Task<IEnumerable<StockTick>> CreateStockTicks()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection
                .UseAutoRegistration()
                .IncludeAssembly(typeof(Candle).Assembly)
                .IncludeAssembly(typeof(Quote).Assembly)
                .Fill();

            var provider = serviceCollection.BuildServiceProvider();

            var priceHistoryService = provider.GetService<IDailyHistoricalQuoteService>();

            var quotes = await priceHistoryService.GetDailyQuotesAsync("VTI", 5);

            return quotes.Select(q => new StockTick()
            {
                Close = q.Close,
                High = q.High,
                Low = q.Low,
                Open = q.Open,
                Volume = (float)q.Volume,
                Year = q.DateTime.Year,
                Month = q.DateTime.Month,
                Day = q.DateTime.Day
            });
        }


        public class StockTick
        {
            [Column("0")]
            [ColumnName("Open")]
            public float Open { get; set; }

            [Column("1")]
            [ColumnName("Close")]
            public float Close { get; set; }

            [Column("2")]
            public float High { get; set; }

            [Column("3")]
            public float Low { get; set; }

            [Column("4")]
            public float Volume { get; set; }

            [Column("5")]
            public float Year { get; set; }
            [Column("6")]
            public float Month { get; set; }
            [Column("7")]
            public float Day { get; set; }
        }

        public class StockTickPredication
        {
            public float Score { get; set; }
        }
    }
}
