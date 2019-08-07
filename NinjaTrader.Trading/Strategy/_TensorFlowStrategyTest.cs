namespace NinjaTrader.NinjaScript.Strategies
{
    using NinjaTrader.Data;
    using NinjaTrader.NinjaScript;
    using NinjaTrader.NinjaScript.Indicators;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    /// <summary>
    /// This is very simple (and brittle) TensorFlow Serving integration test.
    /// With this proof of concept, a TF Serving model can be queried hundreds of times per second.
    /// Performance has in no way been optimized and no calls are async.  This is strictly a proof of concept test.
    /// </summary>
    public class __TensorFlowStrategyTest : Strategy
    {
        //private _BareIndicator touchIndicator;
        //private double touchIndicatorValue;

        private const string restRequestString = "http://localhost:8501/v1/models/half_plus_two:predict";
        private static int requests = 0;
        Stopwatch sw = new Stopwatch();

        protected override void OnBarUpdate()
        {
            base.OnBarUpdate();

            try
            {
                // this "touch mechanism" is required to have the indicator OnBarUpdate get called
                // after strategy OnBarUpdate
                //this.touchIndicatorValue = this.touchIndicator.Value[0];

                GetPrediction(Close[0]);

                if (requests % 50 == 0)
                {
                    Print(string.Format("{0} requests per second", requests / sw.Elapsed.TotalSeconds));
                }
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnMarketDepth(MarketDepthEventArgs marketDepthUpdate)
        {
            base.OnMarketDepth(marketDepthUpdate);

            GetPrediction(marketDepthUpdate.Price);

            if (requests % 50 == 0)
            {
                Print(string.Format("{0} requests per second", requests / sw.Elapsed.TotalSeconds));
            }
        }

        protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
        {
            base.OnMarketData(marketDataUpdate);
        }

        private static void GetPrediction(double close)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(restRequestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write("{\"instances\": [" + close + "]}");
                }
            }
            catch (System.Net.WebException exception)
            {
                //Assert.Fail("TensorFlow Serving is not running.");
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                var expect = (close / 2) + 2;
                requests++;
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
                    base.Description = @"This is a test strategy that communicates with TensorFlow Serving model half_plus_two.";

                    base.Calculate = Calculate.OnEachTick;
                    break;
                //case State.Configure:
                //    break;
                //case State.Active:
                //    break;
                case State.DataLoaded:
                    ClearOutputWindow();
                    Print("stopwatch started");
                    requests = 0;
                    sw.Start();
                    // NOTE: verified working mechanism which fires OnBarUpdate for the indicator
                    //lock (this.NinjaScripts)
                    //{
                    //    this.NinjaScripts.Add(this.touchIndicator = new _BareIndicator() { Parent = this, IsCreatedByStrategy = true });
                    //}

                    // NOTE: this mechanism does not work properly in a 3rd party dll scenario.  Use above method which does work.
                    //base.AddChartIndicator(this.touchIndicator = new _BareIndicator());

                    break;
                    //case State.Historical:
                    //    break;
                    //case State.Transition:
                    //    break;
                case State.Realtime:
                    break;
                case State.Terminated:
                    if (sw != null && sw.IsRunning)
                    {
                        Print("stopwatch stopped");
                        sw.Stop();
                    }
                    break;
                    //case State.Finalized:
                    //    break;
                    //default:
                    //    break;
            }
        }
    }
}