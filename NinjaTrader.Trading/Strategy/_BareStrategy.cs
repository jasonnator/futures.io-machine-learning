namespace NinjaTrader.NinjaScript.Strategies
{
    using NinjaTrader.NinjaScript;
    using NinjaTrader.NinjaScript.Indicators;
    using System;

    public class _BareStrategy : Strategy
    {
        private _BareIndicator touchIndicator;
        private double touchIndicatorValue;

        protected override void OnBarUpdate()
        {
            base.OnBarUpdate();

            try
            {
                // this "touch mechanism" is required to have the indicator OnBarUpdate get called
                // after strategy OnBarUpdate
                this.touchIndicatorValue = this.touchIndicator.Value[0];
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnStateChange()
        {
            base.OnStateChange();

            switch (base.State)
            {
                //case State.Undefined:
                //    break;
                case State.SetDefaults:
                    base.Name = string.Format("_{0}", this.GetType().Name);
                    base.Description = @"Template description.";

                    base.Calculate = Calculate.OnEachTick;
                    break;
                //case State.Configure:
                //    break;
                //case State.Active:
                //    break;
                case State.DataLoaded:

                    // NOTE: verified working mechanism which fires OnBarUpdate for the indicator
                    lock (this.NinjaScripts)
                    {
                        this.NinjaScripts.Add(this.touchIndicator = new _BareIndicator() { Parent = this, IsCreatedByStrategy = true });
                    }

                    // NOTE: this mechanism does not work properly in a 3rd party dll scenario.  Use above method which does work.
                    //base.AddChartIndicator(this.touchIndicator = new _BareIndicator());

                    break;
                    //case State.Historical:
                    //    break;
                    //case State.Transition:
                    //    break;
                    //case State.Realtime:
                    //    break;
                    //case State.Terminated:
                    //    break;
                    //case State.Finalized:
                    //    break;
                    //default:
                    //    break;
            }
        }
    }
}