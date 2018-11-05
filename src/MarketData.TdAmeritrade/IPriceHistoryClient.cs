using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace MarketData.TdAmeritrade
{
    
    public interface IPriceHistoryClient
    {
        [Get("/v1/marketdata/{symbol}/pricehistory")]
        Task<PriceHistoryQuote> GetPriceHistoryAsync(
            [Path] string symbol,
            [Query("apikey")]string apiKey,
            string periodType,
            int period,
            string frequencyType,
            int frequency);
    }
}
