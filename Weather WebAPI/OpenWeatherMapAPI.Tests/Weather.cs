using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenWeatherMapAPI.Tests
{
    [TestClass]
    public class Weather
    {
        [TestMethod]
        public async Task GetWeather()
        {
            // make a request to get the current weather in Phoenix
            //  id=5308655
            string json =  await Requests.Weather.Current(5308655);

            // json should contain "Phoenix"
            Assert.IsTrue(json.Contains("Phoenix"));
        }

        [TestMethod]
        public async Task GetForecast()
        {
            // make a request to get the forecasted weather in Phoenix
            //  id=5308655
            string json = await Requests.Weather.Forecast(5308655);

            // json should contain "Phoenix"
            Assert.IsTrue(json.Contains("Phoenix"));
        }
    }
}
