/*  CTRADER GURU --> Indicator Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SARTrailingStop : Robot
    {

        #region Enums

        #endregion

        #region Identity

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "SAR Trailing Stop";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.2";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/sar-trailing-stop/")]
        public string ProductInfo { get; set; }

        [Parameter("Min AF", Group = "Params", DefaultValue = 0.02, MinValue = 0)]
        public double MinAF { get; set; }

        [Parameter("Max AF", Group = "Params", DefaultValue = 0.2, MinValue = 0)]
        public double MaxAF { get; set; }

        [Parameter("Auto Stop ?", Group = "Params", DefaultValue = true)]
        public bool AutoStop { get; set; }

        #endregion

        #region Property
        
        #endregion

        #region cBot Events

        protected override void OnStart()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

        }

        protected override void OnTick()
        {

            // --> Fermiamo in automatico
            _checkAutoStop();

        }

        protected override void OnBar()
        {
            
            // --> Al momento le API non forniscono un metodo per ottenere tutte le posizioni di un simbolo se non per LABEL
            // --> quindi devo ciclare
            foreach (var position in Positions)
            {

                if (position.SymbolName != SymbolName) continue;

                bool canModify = false;
                double SARlevel = Math.Round(Indicators.ParabolicSAR(MinAF, MaxAF).Result.LastValue, Symbol.Digits);

                switch (position.TradeType)
                {
                    case TradeType.Buy:

                        canModify = (SARlevel < Bid);

                        break;

                    case TradeType.Sell:

                        canModify = (SARlevel > Ask);

                        break;

                }

                if (canModify && SARlevel != position.StopLoss) ModifyPositionAsync(position, SARlevel, position.TakeProfit);

            }

        }

        #endregion

        #region Private Methods

        private void _checkAutoStop()
        {

            if (!AutoStop) return;

            foreach (var position in Positions)
            {

                if (position.SymbolName != SymbolName) continue;

                // --> Ok c'è una posizione interrompo e basta
                return;

            }

            // --> Se non ho interrotto vuol dire che non c'è nulla per noi, quindi fermo tutto;
            Stop();

        }

        #endregion

    }
    
}