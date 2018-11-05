namespace MarketData.TdAmeritrade
{
    public interface IPriceHistoryClientFactory
    {
        IPriceHistoryClient CreatePriceHistoryClient();
    }
}