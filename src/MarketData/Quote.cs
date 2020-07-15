using System;

namespace MarketData
{
    public class Quote
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get;set; }
        public int Volume { get; set; }
        public DateTime DateTime { get; set; }
    }
}