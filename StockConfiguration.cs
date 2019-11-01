using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stock
{
    public class StockConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseStockMarket;
        public string DatabaseStockholder;
        public int DatabasePort;
        public int StockMarketUpdateInterval;
        public double StockMarketSellingPriceDifference;
        public int BankruptReopenIntervalMin;

        public void LoadDefaults()
        {
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseStockMarket = "stockmarket";
            DatabaseStockholder = "stockholder";
            DatabasePort = 3306;
            StockMarketUpdateInterval = 3;
            StockMarketSellingPriceDifference = 0.9;
            BankruptReopenIntervalMin = 30;
        }
    }
}