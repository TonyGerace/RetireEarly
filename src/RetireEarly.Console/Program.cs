using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
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
using RetireEarly.Registration.Feature;
using static RetireEarly.Program;

namespace RetireEarly
{
    public class MovementTracker
    {


        public enum Direction
        {
            Up,
            None,
            Down
        }


        private decimal PercentChange = 0;
        private decimal LastPrice = 0;
        private decimal PriceAtLastDirectionChange = 0;
        private decimal PercentChangeSinceLastDirectionChange = 0;
        private Direction CurrentDirection = Direction.None;

        public void RecordMovement(Quote quote)
        {
            RecordMovementForPrice(quote.Open);
            Console.WriteLine($"Direction{CurrentDirection} {PercentChange.ToString("P2", System.Globalization.CultureInfo.InvariantCulture)}  Price: {LastPrice}");
        }

        private void RecordMovementForPrice(decimal price)
        {
            Direction newDirection;

            
            if(price < LastPrice)
            {
                newDirection = Direction.Down;
                PercentChange = CalculatePriceDecrease(price, LastPrice);

            } 
            else if(price > LastPrice)
            {
                newDirection = Direction.Up;
                PercentChange = CalculatePriceIncrease(price, LastPrice);
            } 
            else
            {
                newDirection = Direction.None;
                PercentChange = 0;
            }



            //Check to see if the direction is new
            if(newDirection != CurrentDirection)
            {
                PercentChangeSinceLastDirectionChange = PercentChange;
                CurrentDirection = newDirection;
                PriceAtLastDirectionChange = price;
            }
            else
            {


                switch (CurrentDirection)
                {
                    case Direction.Down:
                        PercentChangeSinceLastDirectionChange = CalculatePriceDecrease(price, PriceAtLastDirectionChange);
                        break;
                    case Direction.Up:
                        PercentChangeSinceLastDirectionChange = CalculatePriceIncrease(price, PriceAtLastDirectionChange);
                        break;
                    default:
                        PercentChangeSinceLastDirectionChange = 0;
                        break;
                }
            }

            LastPrice = price;
        }

        private decimal CalculatePriceIncrease(decimal newNumber, decimal orginalNumber)
        {
            if (orginalNumber == 0)
                orginalNumber = 1;

            var increase = newNumber - orginalNumber;
            var precentage = (increase / orginalNumber);
            return precentage;
        }

        private decimal CalculatePriceDecrease(decimal newNumber, decimal orginalNumber)
        {
            var decrease = orginalNumber - newNumber;
            var percentage = (decrease - orginalNumber);
            return percentage;
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            var movementTracker = new MovementTracker();
            var stockTicks =
                CreateStockTicks()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();

            foreach (var tick in stockTicks)
            {
                Console.WriteLine($"Open: {tick.Open} Close: {tick.Close}");
                movementTracker.RecordMovement(tick);
            }



            Console.ReadLine();
        }


        private static async Task<IEnumerable<Quote>> CreateStockTicks()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection
                .UseAutoRegistration()
                .IncludeAssembly(typeof(Candle).Assembly)
                .IncludeAssembly(typeof(Quote).Assembly)
                .Fill();

            var provider = serviceCollection.BuildServiceProvider();

            var priceHistoryService = provider.GetService<IDailyHistoricalQuoteService>();

            var quotes = await priceHistoryService.GetDailyQuotesAsync("SPY", 5);


            return quotes;
            //return quotes.Select(q => new StockTick()
            //{
            //    Close = q.Close,
            //    High = q.High,
            //    Low = q.Low,
            //    Open = q.Open,
            //    Volume = (decimal)q.Volume,
            //    Year = q.DateTime.Year,
            //    Month = q.DateTime.Month,
            //    Day = q.DateTime.Day
            //});
        }


        public class StockTick
        {
            [Column("0")]
            [ColumnName("Open")]
            public decimal Open { get; set; }

            [Column("1")]
            [ColumnName("Close")]
            public decimal Close { get; set; }

            [Column("2")]
            public decimal High { get; set; }

            [Column("3")]
            public decimal Low { get; set; }

            [Column("4")]
            public decimal Volume { get; set; }

            [Column("5")]
            public decimal Year { get; set; }
            [Column("6")]
            public decimal Month { get; set; }
            [Column("7")]
            public decimal Day { get; set; }
        }

        public class StockTickPredication
        {
            public decimal Score { get; set; }
        }
    }
}
