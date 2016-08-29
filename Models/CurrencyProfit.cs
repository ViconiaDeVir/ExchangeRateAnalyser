using ExchangeRateAnalyser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Models
{
    /// <summary>
    /// Represents Currency profit class
    /// </summary>
    public class CurrencyProfit
    {
        /// <summary>
        /// Gets or sets currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the best rate to buy Currency
        /// </summary>
        public ExchangeRate BestRateToBuy { get; set; }

        /// <summary>
        /// Gets or sets the best rate to sell Currency
        /// </summary>
        public ExchangeRate BestRateToSell { get; set; }

        /// <summary>
        /// Gets the most profitable deviation between Currency exchange rates
        /// </summary>
        private double MostProfitableDelta
        {
            get
            {
                if (this.BestRateToBuy != null && this.BestRateToSell != null)
                    return this.BestRateToSell.Rate - this.BestRateToBuy.Rate;
                else
                    return double.MinValue;

            }
        }

        /// <summary>
        /// Initializes a new instance of the CurrencyProfit class
        /// </summary>
        private CurrencyProfit()
        {

        }

        /// <summary>
        /// Gets the possible profit with the certain capital value
        /// </summary>
        /// <param name="initialCapital"></param>
        /// <returns></returns>
        public decimal GetCapitalProfit(decimal initialCapital)
        {
            return (decimal)this.MostProfitableDelta * initialCapital / (decimal)this.BestRateToBuy.Rate;
        }

        /// <summary>
        /// Loads the specified CurrencyProfit using certain currency.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static CurrencyProfit Load(Currency currency)
        {
            CurrencyProfit curProfit = new CurrencyProfit();
            curProfit.Currency = currency;
            curProfit.LoadBestRates();
            return curProfit;
        }

        /// <summary>
        /// Analyses the currency exchange rates and then fills BestRateToBuy and BestRateToSell properties.
        /// O(n) complexity
        /// </summary>
        /// <returns>True if succeeded to load best rates of the Currency, otherwise - false</returns>
        private bool LoadBestRates()
        {
            var result = true;

            ExchangeRate erBestToBuy = null;
            ExchangeRate erBestToSell = null;

            List<ExchangeRate> exchangeRates = this.Currency.ExchangeRates;

            if (exchangeRates != null && exchangeRates.Count > 0 && exchangeRates[0] != null)
            {
                try
                {
                    double startRateToBuy = exchangeRates[0].Rate;
                    double potentialStartRateToBuy = double.MaxValue;

                    erBestToBuy = exchangeRates[0];
                    ExchangeRate erPotentialBestToBuy = null;

                    erBestToSell = exchangeRates[0];

                    double maxDelta = erBestToSell.Rate - erBestToBuy.Rate;

                    for (int i = 1; i < exchangeRates.Count; i++)
                    {
                        if (exchangeRates[i] != null)
                        {
                            if (maxDelta < exchangeRates[i].Rate - startRateToBuy)
                            {
                                maxDelta = exchangeRates[i].Rate - startRateToBuy;
                                erBestToSell = exchangeRates[i];
                            }

                            if (exchangeRates[i].Rate < startRateToBuy &&
                                exchangeRates[i].Rate < potentialStartRateToBuy)
                            {
                                potentialStartRateToBuy = exchangeRates[i].Rate;
                                erPotentialBestToBuy = exchangeRates[i];
                            }

                            if (maxDelta < exchangeRates[i].Rate - potentialStartRateToBuy)
                            {
                                maxDelta = exchangeRates[i].Rate - potentialStartRateToBuy;

                                startRateToBuy = potentialStartRateToBuy;
                                potentialStartRateToBuy = double.MaxValue;

                                erBestToBuy = erPotentialBestToBuy;
                                erPotentialBestToBuy = null;

                                erBestToSell = exchangeRates[i];
                            }

                        }
                    }

                    if (maxDelta != double.MinValue)
                    {
                        this.BestRateToBuy = erBestToBuy;
                        this.BestRateToSell = erBestToSell;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(Strings.ErrorOccured, ex);

                    erBestToBuy = null;
                    erBestToSell = null;
                }
            }


            if (erBestToBuy == null || erBestToSell == null)
            {
                result = false;
            }

            return result;
        }
    }
}
