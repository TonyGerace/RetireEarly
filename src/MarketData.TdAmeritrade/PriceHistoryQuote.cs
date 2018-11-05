using System.Collections;
using System.Collections.Generic;

namespace MarketData.TdAmeritrade
{
    public class PriceHistoryQuote
    {
        public string Symbol { get; set; }

        public IEnumerable<Candle> Candles { get; set; }
    }
}
