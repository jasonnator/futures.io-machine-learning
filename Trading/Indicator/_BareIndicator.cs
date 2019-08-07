namespace NinjaTrader.NinjaScript.Indicators
{
    using NinjaTrader.Data;

    public sealed class _BareIndicator : Indicator
    {
#if DEBUG

        /// <summary>
        /// This will automatically hook the debugger instead of doing attach to process from Visual
        /// Studio each time.
        /// </summary>
        static _BareIndicator()
        {
            Trading.AutoDebug.Launch();
        }

#endif

        public override string DisplayName
        {
            get
            {
                return base.Name;
            }
        }

        protected override void OnBarUpdate()
        {
            base.OnBarUpdate();

            if (base.CurrentBars[0] < base.BarsRequiredToPlot)
            {
                return;
            }

            // REQUIRED: necessary to assign any value so strategies using this indicator the
            //           OnBarUpdate will be called in this indicator.
            base.Value[0] = double.NaN;
        }

        protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
        {
            base.OnMarketData(marketDataUpdate);
        }

        protected override void OnStateChange()
        {
            base.OnStateChange();

            switch (base.State)
            {
                case State.Undefined:
                    break;

                case State.SetDefaults:
                    base.Name = string.Format("_{0}", this.GetType().Name);
                    base.Description = @"Template description.";

                    base.Calculate = Calculate.OnEachTick;
                    base.IsSuspendedWhileInactive = true;
                    base.IsAutoScale = true;
                    base.IsOverlay = true;

                    // REQUIRED: setting this to false and this indicator used by a strategy, this indicator's OnBarUpdate will not fire properly
                    //base.IsVisible = true;

                    base.BarsRequiredToPlot = 0;

                    break;

                case State.Configure:
                    // REQUIRED: necessary to add at least 1 plot so strategies using this indicator
                    //           the OnBarUpdate will be called in this indicator.
                    base.AddPlot(System.Windows.Media.Brushes.Transparent, "touchPlot");

                    // add any data series necessary
                    //base.AddDataSeries(BarsPeriodType.Minute, 1);
                    break;

                case State.Active:
                    break;

                case State.DataLoaded:

                    // initialize any Series<T>(this) for synchronization with price
                    break;

                case State.Historical:
                    break;

                case State.Transition:
                    break;

                case State.Realtime:
                    break;

                case State.Terminated:
                    break;

                case State.Finalized:
                    break;

                default:
                    break;
            }
        }
    }
}