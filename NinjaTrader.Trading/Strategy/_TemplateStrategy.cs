namespace NinjaTrader.NinjaScript.Strategies
{
    using NinjaTrader.Cbi;
    using NinjaTrader.Data;
    using NinjaTrader.NinjaScript;
    using NinjaTrader.NinjaScript.Indicators;
    using System;
    using System.Timers;

    public sealed class TemplateStrategy : Strategy
    {
        private CandleStickPatternLogic candleStickPatternLogic;
        private DateTime endTimeForLongEntries;
        private DateTime endTimeForLongExits;
        private Data.SessionIterator sessionIterator;
        private DateTime startTimeForLongEntries;
        private DateTime startTimeForLongExits;
        private bool testCanEnterTrades = true;
        private readonly double testIndicatorValue = 0;
        private Timer testTimer;
        private _BareIndicator touchIndicator;
        private double touchIndicatorValue;

        /// <summary>
        /// Use the base Name property which uses the class name for convention.
        /// </summary>
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

            if (this.CurrentBars[0] < this.BarsRequiredToTrade)
            {
                return;
            }

            if (this.sessionIterator == null || this.BarsArray[0].IsFirstBarOfSession)
            {
                if (this.sessionIterator == null)
                {
                    this.sessionIterator = new Data.SessionIterator(this.BarsArray[0]);
                    this.sessionIterator.GetNextSession(this.Times[0][0], true);
                }
                else if (this.BarsArray[0].IsFirstBarOfSession)
                {
                    this.sessionIterator.GetNextSession(this.Times[0][0], true);
                }

                this.startTimeForLongEntries = this.sessionIterator.ActualSessionBegin.AddMinutes(15);
                this.endTimeForLongEntries = this.startTimeForLongEntries.AddMinutes(75);
                this.startTimeForLongExits = this.sessionIterator.ActualSessionEnd.AddMinutes(-90);
                this.endTimeForLongExits = this.startTimeForLongExits.AddMinutes(45);
            }

            // example entry & exit logic
            if (this.startTimeForLongEntries < this.Times[0][0]
                && this.Times[0][0] <= this.endTimeForLongEntries
                && (this.Times[0][0].DayOfWeek == DayOfWeek.Monday || this.Times[0][0].DayOfWeek == DayOfWeek.Friday))
            {
                //this.EnterLong();
            }

            if (this.startTimeForLongExits < this.Times[0][0]
                && this.Times[0][0] <= this.endTimeForLongExits
                && (this.Times[0][0].DayOfWeek == DayOfWeek.Monday || this.Times[0][0].DayOfWeek == DayOfWeek.Friday))
            {
                //this.ExitLong();
            }

            this.touchIndicatorValue = this.touchIndicator.Value[0];
        }

        protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
        {
            base.OnExecutionUpdate(execution, executionId, price, quantity, marketPosition, orderId, time);

            if (this.Position.MarketPosition == MarketPosition.Flat)
            {
                // stop the time from entering trades. we only need 1 round of entries and exits to test
                this.testTimer.Stop();
            }

            // handle different trade cases trade entry add to trade scale out of trade hit profit
            // target hit stop loss go flat
        }

        protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
        {
            base.OnMarketData(marketDataUpdate);
        }

        protected override void OnStateChange()
        {
            base.OnStateChange();

            #region State Flow

            // adding a strategy via Strategies Panel (Ctrl+S)
            //
            // SetDefaults (strat window) Terminated (strat window) SetDefaults (strat window)
            // SetDefaults (click add) SetDefaults (click OK) Terminated (click OK) Terminated (strat
            // window closed) Configure Account gets set EditGuid gets set ChartBars gets set
            // ChartControl gets set Instrument gets set TradingHours gets set SetDefaults Name gets
            // set EditGuid gets set to null NinjaScripts gets initialized Terminated TradingHours
            // gets set Name gets set NinjaScripts gets incremented Configure Id gets set ChartBars
            // gets set ChartControl gets set ChartPanel gets set TradingHours gets set DataLoaded
            // Bars gets set ChartPanel gets set TradingHours gets set Historical Position gets set
            // Bars gets set CurrentBar = -1 Dispatcher gets set Transition Realtime ChartPanel gets set

            #endregion State Flow

            switch (base.State)
            {
                case State.Undefined:
                    break;

                case State.SetDefaults:

                    base.Name = string.Format("_{0}", this.GetType().Name);
                    base.Calculate = Calculate.OnEachTick;

                    base.EntriesPerDirection = 5;
                    base.IncludeCommission = true;

                    base.IncludeTradeHistoryInBacktest = false;
                    base.IsExitOnSessionCloseStrategy = true;
                    base.IsInstantiatedOnEachOptimizationIteration = true;
                    base.MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                    base.SupportsOptimizationGraph = false;
                    break;

                case State.Configure:
                    this.candleStickPatternLogic = new CandleStickPatternLogic(this, 7);
                    //base.SetProfitTarget(CalculationMode.Percent, 0.02);
                    //base.SetTrailStop(CalculationMode.Percent, 0.01);

                    // add any data series necessary
                    //base.AddDataSeries(BarsPeriodType.Minute, 1);
                    break;

                case State.Active:
                    break;

                case State.DataLoaded:

                    // verified working mechanism which fires OnBarUpdate for the indicator
                    lock (this.NinjaScripts)
                    {
                        this.NinjaScripts.Add(this.touchIndicator = new _BareIndicator() { Parent = this, IsCreatedByStrategy = true });
                    }

                    this.testTimer = new Timer(5000);
                    this.testTimer.Elapsed += this.TestTimer_Elapsed;

                    // NOTE: this method does not fire indiciator data events
                    //this.templateIndicator = new TemplateIndicator() { Parent = this };
                    //base.AddChartIndicator(this.templateIndicator = new TemplateIndicator() { Parent = this });
                    break;

                case State.Historical:
                    break;

                case State.Transition:
                    break;

                case State.Realtime:
                    this.testTimer.Start();
                    break;

                case State.Terminated:
                    if (this.testTimer != null)
                    {
                        this.testTimer.Elapsed -= this.TestTimer_Elapsed;
                    }
                    break;

                case State.Finalized:
                    break;

                default:
                    break;
            }
        }

        private void TestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.Position != null)
            {
                if (this.Position.Quantity == this.EntriesPerDirection && this.testCanEnterTrades)
                {
                    this.testTimer.Interval = 2500.0; // scale out every 2.5 seconds until flat
                    this.testCanEnterTrades = false;
                }
                else if (this.Position.Quantity < this.EntriesPerDirection && this.testCanEnterTrades)
                {
                    this.EnterLong(1);
                }
                else if (this.Position.MarketPosition != MarketPosition.Flat)
                {
                    this.ExitLong(1);
                }
            }
        }
    }
}