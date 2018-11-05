using System;

namespace MarketData.TdAmeritrade
{
    public class Candle
    {
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get;set; }
        public int Volume { get; set; }
        public long DateTime { get; set; }
    }
}