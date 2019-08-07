namespace TensorFlowTesting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using System.Net.Http;

    /// <summary>
    /// This is a simple test class which tests a toy half_plus_two tensorflow model.
    /// TensorFlow Serving must be up and running for the test to complete.
    /// </summary>
    [TestClass]
    public class TensorFlowServingTest
    {
        private const string jsonData = "{\"instances\": [1.0, 2.0, 5.0]}";
        private const string restRequestString = "http://localhost:8501/v1/models/half_plus_two:predict";

        /// <summary>
        /// Powershell command used:
        /// docker run -d -t --rm -p 8501:8501 --name=tfServing --mount type=bind,source="C:\Users\Administrator\Documents\serving\tensorflow_serving\servables\tensorflow\testdata\saved_model_half_plus_two_cpu",target=/models/half_plus_two -e MODEL_NAME=half_plus_two tensorflow/serving
        /// </summary>
        [TestMethod]
        public void TestTensorFlowServingWebReqeuest()
        {
            string result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(restRequestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonData);
                }
            }
            catch (System.Net.WebException exception)
            {
                Assert.Fail("TensorFlow Serving is not running.");
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Dictionary<string, double[]> data = JsonConvert.DeserializeObject<Dictionary<string, double[]>>(result);

            Assert.AreEqual(data.Keys.ToList()[0], "predictions");

            double[] dataArray = data.Values.ToArray()[0];
            Assert.AreEqual(dataArray.Length, 3);

            Assert.AreEqual(dataArray[0], 2.5);
            Assert.AreEqual(dataArray[1], 3.0);
            Assert.AreEqual(dataArray[2], 4.5);
        }

        [TestMethod]
        public void TestTensorFlowServingGRPC()
        {
            Assert.Fail("TODO: gRPC test is not set up yet.");
        }
    }
}
