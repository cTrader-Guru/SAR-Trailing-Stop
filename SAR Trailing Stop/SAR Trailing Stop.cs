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

        #region Identity

        public const string NAME = "SAR Trailing Stop";

        public const string VERSION = "1.0.2";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+sar+trailing+stop")]
        public string ProductInfo { get; set; }

        [Parameter("Min AF", Group = "Params", DefaultValue = 0.02, MinValue = 0)]
        public double MinAF { get; set; }

        [Parameter("Max AF", Group = "Params", DefaultValue = 0.2, MinValue = 0)]
        public double MaxAF { get; set; }

        [Parameter("Auto Stop ?", Group = "Params", DefaultValue = true)]
        public bool AutoStop { get; set; }

        #endregion

        #region cBot Events

        protected override void OnStart()
        {

            Print("{0} : {1}", NAME, VERSION);

        }

        protected override void OnTick()
        {

            CheckAutoStop();

        }

        protected override void OnBar()
        {

            foreach (var position in Positions)
            {

                if (position.SymbolName != SymbolName)
                    continue;

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

                if (canModify && SARlevel != position.StopLoss)
                    ModifyPositionAsync(position, SARlevel, position.TakeProfit);

            }

        }

        #endregion

        #region Private Methods

        private void CheckAutoStop()
        {

            if (!AutoStop)
                return;

            foreach (var position in Positions)
            {

                if (position.SymbolName != SymbolName)
                    continue;

                return;

            }

            Stop();

        }

        #endregion

    }

}
