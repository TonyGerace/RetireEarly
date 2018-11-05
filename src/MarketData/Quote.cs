using System;

namespace MarketData
{
    public class Quote
    {
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get;set; }
        public int Volume { get; set; }
        public DateTime DateTime { get; set; }
    }
}