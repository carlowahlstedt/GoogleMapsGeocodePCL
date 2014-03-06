using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geocoding.Google;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void TestMethod()
        {
            var geoCoder = new GoogleGeocoder
            {
                //ApiKey = ConfigurationManager.AppSettings["googleApiKey"]
            };

            var task = await geoCoder.GeocodeAsync("1600 pennsylvania ave washington dc");

            Address[] addresses = task.ToArray();
            var address = addresses[0];
            Assert.IsTrue(
                address.FormattedAddress.Contains("The White House") ||
                address.FormattedAddress.Contains("1600 Pennsylvania Ave NW") ||
                address.FormattedAddress.Contains("1600 Pennsylvania Avenue Northwest")
            );
            Assert.AreEqual(GoogleAddressType.StreetAddress, addresses[0].AddressTypes[0]);
            Assert.IsTrue(address.FormattedAddress.Contains("Washington, DC"));

            //just hoping that each geocoder implementation gets it somewhere near the vicinity
            double lat = Math.Round(address.Geometry[0].Location.Latitude, 2);
            Assert.AreEqual(38.90, lat);

            double lng = Math.Round(address.Geometry[0].Location.Longitude, 2);
            Assert.AreEqual(-77.04, lng);

        }
    }
}
