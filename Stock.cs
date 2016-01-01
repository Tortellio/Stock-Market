﻿using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Core.Logging;
using Rocket.API;
using System;
using Rocket.Unturned.Plugins;

namespace Stock
{
    public class Stock: RocketPlugin<StockConfiguration>
    {
        private DateTime lastcheck;
        public DateTime[] Bankrupt;
        public DatabaseManager Database;
        public static Stock Instance;
        private int countMinus = 0;
        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            this.lastcheck = DateTime.Now;
        }
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                {"Oil_StockFall_01","The Organization of the Petroleum Exporting Countries said it will not change its policy of maintaining high production levels to keep its market share"},
                {"All_StockFall_01", "Stock Crashed."},
                {"lpx_list_Allstock", "Stock >> {0}"},
                {"lpx_help_buystock", "/stock buy <StockName> <Amount>"},
                {"lpx_help_sellstock", "/stock sell <StockName> <Amount or *(for all)>"},
                {"lpx_help_buystock2","Missing <Amount> Parameter. Please Indicate the amount of units you would like to buy."},
                {"not_enough_currency_msg","You do not have enough {0} to buy {1} units of {2}"},
                {"lpx_stock_buy","You bought {0} units of {1} for {2} {3}. Your balance: {4}"},
                {"lpx_stock_owned","You already own a stock. Sell to change stocks."},
                {"lpx_no_stock","You do not own this stock"},
                {"not_enough_stock_sell","You do not have enough stocks to sell, you owned {0} units."},
                {"lpx_stock_sell","You sold {0} units of {1} for {2} {3}. Your balance: {4}"}
                };
            }
        }
        private void FixedUpdate()
        {
            if((DateTime.Now - this.lastcheck).TotalSeconds >=  Stock.Instance.Configuration.Instance.StockMarketUpdateInterval )
            {
                this.lastcheck = DateTime.Now;                
                string[] AllStocks = Stock.Instance.Database.GetAllStockName();
                Bankrupt = new DateTime[AllStocks.Length]; 
                double[] stockprice = new double[AllStocks.Length];
                double NewPrice = 0.00;               
                for(int i = 0; i < AllStocks.Length; i ++)
                {
                    if (Stock.Instance.Database.Getstatus(AllStocks[i]).Trim() != "bankrupt")
                    {
                        stockprice[i] = Stock.Instance.Database.GetStockPrice(AllStocks[i]);
                        double Value = value();
                        switch (UpOrDown())
                        {
                            case '+':
                                NewPrice = stockprice[i] + Value;
                                break;
                            case '-':
                                NewPrice = stockprice[i] - Value;
                                break;
                        }
                        if (NewPrice <= 0)
                        {
                            Stock.Instance.Database.SetUpdateStockPrice(0.00, AllStocks[i], 0.00);
                            Stock.Instance.Database.SetStatus(AllStocks[i], "bankrupt");
                            Bankrupt[i] = DateTime.Now;
                        }
                        else
                            Stock.Instance.Database.SetUpdateStockPrice(Math.Round(NewPrice, 2), AllStocks[i], Math.Round(NewPrice, 2) - Stock.Instance.Configuration.Instance.StockMarketSellingPriceDifference);

                    }
                    if (Stock.Instance.Database.Getstatus(AllStocks[i]) == "bankrupt")
                    {
                        string[] Players = Stock.Instance.Database.GetAllPlayerBankrupt(AllStocks[i]);
                        for (int x = 0; x < Players.Length; x++)
                        {
                            Stock.Instance.Database.RemovePlayer(Players[x]);
                        }
                        if ((DateTime.Now - Bankrupt[i]).Minutes >= Stock.Instance.Configuration.Instance.BankruptReopenIntervalMin)
                        {
                            Stock.Instance.Database.SetStatus(AllStocks[i], "available");
                            Stock.Instance.Database.SetUpdateStockPrice(50, AllStocks[i], Math.Round(NewPrice, 2) - Stock.Instance.Configuration.Instance.StockMarketSellingPriceDifference);
                        }
                    }
                }
            }

        }
        private char UpOrDown()
        {
            char UpDown = ' ';
            int randomNumber = 0;
            System.Random random = new System.Random();
            for (int i = 0; i < 2; i++)
            {
                randomNumber = random.Next(1, 101);
            }
            if(randomNumber <= 50)
            {
                UpDown = '+';
            }
            else
            {
                if (countMinus < 15)
                    UpDown = '-';
                else
                {
                    UpDown = '+';
                    countMinus = 0;
                }
                countMinus++;
            }
            return UpDown;
        }
        private double value()
        {
            double randomNo = 0.00;
            System.Random random = new System.Random();
            for (int i = 0; i < 2; i++)
            {
                randomNo = (random.NextDouble() * (0.20-0.01)) + 0.01;
            }
            return randomNo;
        }
    }
}
