namespace MarketData.TdAmeritrade
{
    public interface ICandleToQuoteMappingService
    {
        Quote Map(Candle candle);
    }
}