using System;
using System.Threading.Tasks;
using Xunit;

namespace MarketData.TdAmeritrade.IntegrationTests
{
    public class PriceHistoryClientTests
    {
        [Fact]
        public async Task GetPriceHistory()
        {
            //arrange
            var factory = new PriceHistoryClientFactory();
            var client = factory.CreatePriceHistoryClient();
            var symbol = "VTI";

            //act
            var results = await client.GetPriceHistoryAsync(
                symbol,
                "CONSOLE@AMER.OAUTHAP",
                "year",
                1,
                "daily",
                1);

            //assert
            Assert.NotNull(results);
            Assert.NotEmpty(results.Candles);
            Assert.Equal(symbol, results.Symbol);

        }
    }
}
