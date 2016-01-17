using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using fr34kyn01535.Uconomy; 

namespace Stock
{
    public class DatabaseManager
    {

        internal DatabaseManager()
        {
            new I18N.West.CP1250(); //Workaround for database encoding issues with mono
            CheckSchema();
        }
        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (Stock.Instance.Configuration.Instance.DatabasePort == 0) Stock.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Stock.Instance.Configuration.Instance.DatabaseAddress, Stock.Instance.Configuration.Instance.DatabaseName, Stock.Instance.Configuration.Instance.DatabaseUsername, Stock.Instance.Configuration.Instance.DatabasePassword, Stock.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }
        public string[] GetAllStockName()
        {
            string[] Stocks = {};
            string stock = "";
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `stockname` from `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "`";
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    stock += dt.Rows[i].ItemArray[0].ToString() + " ";
                }
                stock = stock.Trim();
                Stocks = stock.Split(' ');
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return Stocks;
        }
        public string[] GetAllPlayerBankrupt(string stock)
        {
            string[] Players = { };
            string Player = "";
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `steamId` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `stocks` = '" + stock + "'";
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dt);
                connection.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Player += dt.Rows[i].ItemArray[0].ToString() + " ";
                }
                Player = Player.Trim();
                Players = Player.Split(' ');
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return Players;
        }
        public string GetPlayerStockName(string id)
        {
            string stock = "";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `stocks` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '"+id+"'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    stock = obj.ToString();
                stock = stock.Trim();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return stock;
        }
        public double GetStockPrice(string stockname)
        {
            double price = 0.00;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `price` from `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` where `stockname` = '"+stockname+"'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if(obj != null)
                {
                    price = double.Parse(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return price;
        }
        public double GetStockSellingPrice(string stockname)
        {
            double price = 0.00;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `sellingprice` from `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` where `stockname` = '" + stockname + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                {
                    price = double.Parse(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return price;
        }
        public void SetUpdateStockPrice(double newprice, string stockname, double sellingprice)
        {
            decimal price = decimal.Parse(newprice.ToString());
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` set `price` = " + newprice + " ,`sellingprice` = "+ sellingprice+" where `stockname` = '" + stockname + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public bool CheckPlayerOwnedStock(string Id)
        {
            bool havestock = false;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `stocks` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + Id + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    havestock = true;
                else
                    havestock = false;               
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return havestock;
        }
        public string GetPlayerProfit(string id)
        {
            string profit = "+0.00%";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `profit` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + id + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                {
                    profit = obj.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return profit;
        }
        public bool SetBalance(string id, decimal newbal)
        {
            bool Updated = false;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update `" + Uconomy.Instance.Configuration.Instance.DatabaseTableName + "` set `balance` = "+newbal+" where `steamId` = '" + id + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
                Updated = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Updated = false;
            }
            return Updated;
        }
        public bool SetPlayerProfit(string id, string stockname)
        {
            bool Updated = false;
            double price = 0.00, newprice = 0.00, profit = 0.00;
            string newprofit = "+0.00%";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `price` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + id + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                    price = double.Parse(obj.ToString());
                newprice = GetStockSellingPrice(stockname);
                profit = ((newprice-price) / price) * 100;
                profit = Math.Round(profit, 2);
                if(profit >= 0.00)
                    newprofit = "+" + profit.ToString() + "%";
                else
                    newprofit = profit.ToString() + "%";
                command.CommandText = "update `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` set `profit` = '" + newprofit + "' where `steamId` = '" + id + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
                Updated = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Updated = false;
            }
            return Updated;
        }
        public bool SetPlayerStock(string id,string stocks, decimal price, int amount)
        {
            bool Updated = false;
            string Oldstock = "";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if(CheckPlayerOwnedStock(id))
                {
                    int Amt = 0;
                    string AllAmt = GetPlayerStockAmt(id);
                    string Amount = "";
                    command.CommandText = "select `stocks` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + id + "'";
                    connection.Open();
                    object obj = command.ExecuteScalar();
                    connection.Close();
                    if (obj != null)
                        Oldstock = obj.ToString();
                    if (Oldstock.Contains(stocks))
                    {
                                           
                        Amt = int.Parse(AllAmt);
                        Amt += amount;
                        Amount = Amt.ToString();
                        command.CommandText = "update `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` set `amt` = '" + Amount + "' where `steamId` = '" + id + "'";
                        connection.Open();
                        command.ExecuteScalar();
                        connection.Close();
                    }
                }
                else
                    command.CommandText = "insert into `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` (`steamId`, `stocks`,`price`, `profit`, `amt`) values('"+id+"','"+stocks+"',"+price+", '+0.00%','"+amount.ToString()+"')";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
                Updated = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Updated = false;
            }
            return Updated;
        }
        public void SetPlayerStockAmt(string steamid, string Amt)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` set `amt` = '" + Amt + "' where `steamId` = '" + steamid + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public string GetPlayerStockAmt(string id)
        {
            string Amt= "";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `amt` from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + id + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                {
                    Amt = obj.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return Amt;
        }
        public bool RemovePlayer(string id)
        {
            bool Removed = false;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "delete from `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` where `steamId` = '" + id + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
                Removed = true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Removed = false;
            }
            return Removed;
        }
        public string Getstatus(string stockname)
        {
            string status = "";
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `status` from `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` where `stockname` = '" + stockname + "'";
                connection.Open();
                object obj = command.ExecuteScalar();
                connection.Close();
                if (obj != null)
                {
                    status = obj.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return status;
        }
        public void SetStatus(string stockname, string status)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` set `Status` = '" + status + "' where `stockname` = '" + stockname + "'";
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + Stock.Instance.Configuration.Instance.DatabaseStockholder + "` (`steamId` varchar(32) NOT NULL,`stocks` varchar(702),`price` decimal(20,2) NOT NULL DEFAULT 0,`profit` varchar(20) NOT NULL DEFAULT '+0.00%',`amt` varchar(156) NOT NULL, PRIMARY KEY (`steamId`)) ";
                    command.ExecuteNonQuery();
                }
                command.CommandText = "show tables like '" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "'";
                test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` (`stockname` varchar(50) NOT NULL,`sentiment` varchar(10) NOT NULL DEFAULT '0.00%',`price` decimal(20,2),`sellingprice` decimal(20,2),`PastMinute` varchar(50) NOT NULL DEFAULT '+0.00%',`Status` varchar(20) NOT NULL DEFAULT 'available',PRIMARY KEY (`stockname`)) ";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` (`stockname`,`sentiment`,`price`,`sellingprice`,`PastMinute`) values('Gold', '0.00%', 1139.70, 1138.80, '+0.00%')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` (`stockname`,`sentiment`,`price`,`sellingprice`,`PastMinute`) values('Oil', '0.00%', 45.06, 44.96, '+0.00%')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into `" + Stock.Instance.Configuration.Instance.DatabaseStockMarket + "` (`stockname`,`sentiment`,`price`,`sellingprice`,`PastMinute`) values('Silver', '0.00%', 15.19, 15.14, '+0.00%')";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}

