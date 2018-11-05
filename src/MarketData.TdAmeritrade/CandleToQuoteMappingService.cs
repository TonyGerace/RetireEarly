using System;

namespace MarketData.TdAmeritrade
{
    public class CandleToQuoteMappingService 
        : ICandleToQuoteMappingService
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Implementation of ICandleToQuoteMappingService

        public Quote Map(Candle candle)
        {
            var retval = new Quote
            {
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low,
                Open = candle.Open,
                Volume = candle.Volume,
                DateTime = epoch.AddMilliseconds(candle.DateTime)
            };

            return retval;
        }

        #endregion
    }
}