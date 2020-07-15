using System;

namespace MarketData.TdAmeritrade
{
    public class Candle
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get;set; }
        public int Volume { get; set; }
        public long DateTime { get; set; }
    }
}