using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarketData
{
    public interface IDailyHistoricalQuoteService
    {
        Task<IEnumerable<Quote>> GetDailyQuotesAsync(string symbol,
            int years);
    }
}
