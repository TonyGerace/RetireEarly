using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.TdAmeritrade
{
    public class TDAmeritradeDailyHistoricalQuoteService
        : IDailyHistoricalQuoteService
    {
        private readonly IPriceHistoryClientFactory _priceHistoryClientFactory;
        private readonly ICandleToQuoteMappingService _mappingService;

        public TDAmeritradeDailyHistoricalQuoteService(
            IPriceHistoryClientFactory priceHistoryClientFactory,
            ICandleToQuoteMappingService mappingService)
        {
            _priceHistoryClientFactory = priceHistoryClientFactory;
            _mappingService = mappingService;
        }

        #region Implementation of IDailyHistoricalQuoteService

        public async Task<IEnumerable<Quote>> GetDailyQuotesAsync(
            string symbol,
            int years)
        {
            var client =
                _priceHistoryClientFactory.CreatePriceHistoryClient();

            var result = await client
                .GetPriceHistoryAsync(
                    symbol,
                    "CONSOLE@AMER.OAUTHAP",
                    "year",
                    years,
                    "daily",
                    1);

            var retval = result
                .Candles
                .Select(_mappingService.Map);

            return retval;
        }

        #endregion
    }

   
}
