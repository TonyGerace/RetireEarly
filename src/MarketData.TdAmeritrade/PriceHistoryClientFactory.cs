using System;
using System.Collections.Generic;
using System.Text;
using RestEase;

namespace MarketData.TdAmeritrade
{

    public class PriceHistoryClientFactory : IPriceHistoryClientFactory
    {
        public readonly string _tdAmeritradeBaseUri = "https://api.tdameritrade.com";
        private object _lockObject = new Object();
        private IPriceHistoryClient _priceHistoryClient;

        #region Implementation of IPriceHistoryClientFactory

        public IPriceHistoryClient CreatePriceHistoryClient()
        {
            lock(_lockObject)
            {
                if (_priceHistoryClient == null)
                {
                    _priceHistoryClient = RestClient.For<IPriceHistoryClient>(_tdAmeritradeBaseUri);
                }

                return _priceHistoryClient;
            }
        }

        #endregion
    }
}
