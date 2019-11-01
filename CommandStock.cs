using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using fr34kyn01535.Uconomy; 

namespace Stock
{
    public class CommandStock : IRocketCommand
    {
        public string Help
        {
            get { return "Buy/sell stocks"; }
        }

        public string Name
        {
            get { return "stock"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Syntax
        {
            get { return "<stock>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(){"stk"}; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "lpx.stock" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            string comd = String.Join(" ", command);
            string[] oper = comd.Split(' ');
            if (String.IsNullOrEmpty(comd.Trim()))
            {
                string[] AllStock;
                if (Stock.Instance.Database.CheckPlayerOwnedStock(caller.Id) && Stock.Instance.Database.GetPlayerStockAmt(caller.Id) == "0")
                {
                    Stock.Instance.Database.RemovePlayer(caller.Id);
                }

                string List;
                if (Stock.Instance.Database.CheckPlayerOwnedStock(caller.Id))
                {
                    string stock = Stock.Instance.Database.GetPlayerStockName(caller.Id);
                    Stock.Instance.Database.SetPlayerProfit(caller.Id, stock);
                    List = stock + ": " + Stock.Instance.Database.GetStockPrice(stock).ToString() + ", Profit: " + Stock.Instance.Database.GetPlayerProfit(caller.Id) + ", Owned: " + Stock.Instance.Database.GetPlayerStockAmt(caller.Id);
                    UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_list_Allstock", List));
                }
                else
                {
                    AllStock = Stock.Instance.Database.GetAllStockName();
                    for (int i = 0; i < AllStock.Length; i++)
                    {
                        List = AllStock[i] + ": " + Stock.Instance.Database.GetStockPrice(AllStock[i]).ToString();
                        UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_list_Allstock", List));
                    }
                }
                return;
            }
            if (oper.Length == 1)
            {
                switch (oper[0])
                {
                    case "buy":
                        UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_help_buystock"));
                        break;
                    case "sell":
                        UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_help_sellstock"));
                        break;
                    default:
                        break;
                }
                return;
            }
            else
            {
                string[] param = string.Join(" ", oper.Skip(1).ToArray()).Split(' ');
                int Amt = 1;
                switch (oper[0])
                {
                    case "buy":
                        if (Stock.Instance.Database.CheckPlayerOwnedStock(caller.Id) == false || Stock.Instance.Database.GetPlayerStockName(caller.Id) == param[0])
                        {
                            if (param.Length == 1)
                                UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_help_buystock2"));
                            else if (param.Length == 2 && int.TryParse(param[1], out Amt))
                            {
                                decimal price = decimal.Parse(Stock.Instance.Database.GetStockPrice(param[0]).ToString()) * Amt;
                                decimal balance = decimal.Parse(Uconomy.Instance.Database.GetBalance(caller.Id).ToString());
                                if (balance < price)
                                {
                                    UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("not_enough_currency_msg", Uconomy.Instance.Configuration.Instance.MoneyName, Amt, param[0]));
                                    return;
                                }
                                decimal newbal = Uconomy.Instance.Database.IncreaseBalance(caller.Id, (price * -1));
                                Stock.Instance.Database.SetBalance(caller.Id, newbal);
                                Stock.Instance.Database.SetPlayerStock(caller.Id, param[0], decimal.Parse(Stock.Instance.Database.GetStockPrice(param[0]).ToString()), Amt);
                                UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_stock_buy", Amt, param[0], price, Uconomy.Instance.Configuration.Instance.MoneyName, newbal));
                                return;
                            }
                            else
                                UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_help_buystock"));
                        }
                        else
                        {
                            UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_stock_owned"));
                        }
                        break;
                    case "sell":
                        if (param[0] == Stock.Instance.Database.GetPlayerStockName(caller.Id))
                        {
                            if (param.Length == 1)
                                Amt = 1;
                            else if (param.Length == 2 && int.TryParse(param[1], out Amt)) { }
                            else if (param[1] == "*") Amt = int.Parse(Stock.Instance.Database.GetPlayerStockAmt(caller.Id));
                            decimal sellprice = decimal.Parse(Stock.Instance.Database.GetStockSellingPrice(param[0]).ToString()) * Amt;
                            _ = decimal.Parse(Uconomy.Instance.Database.GetBalance(caller.Id).ToString());
                            int PlayerAmt = int.Parse(Stock.Instance.Database.GetPlayerStockAmt(caller.Id));
                            if (Amt > PlayerAmt)
                            {
                                UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("not_enough_stock_sell", PlayerAmt));
                                return;
                            }
                            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(caller.Id, sellprice);
                            Stock.Instance.Database.SetBalance(caller.Id, newbal);
                            Stock.Instance.Database.SetPlayerStockAmt(caller.Id, (PlayerAmt - Amt).ToString());
                            UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_stock_sell", Amt, param[0], sellprice, Uconomy.Instance.Configuration.Instance.MoneyName, newbal));
                            return;
                        }
                        else
                            UnturnedChat.Say(caller, Stock.Instance.DefaultTranslations.Translate("lpx_no_stock"));
                        break;
                        
                }
            }
        }
    }
}
