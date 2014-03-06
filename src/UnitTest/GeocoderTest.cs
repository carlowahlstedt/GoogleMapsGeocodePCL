using Geocoding.Google;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace Geocoding.Tests
{
	public abstract class GeocoderTest
	{
		GoogleGeocoder geoCoder;

		public GeocoderTest()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			geoCoder = new GoogleGeocoder
			{
				//ApiKey = ConfigurationManager.AppSettings["googleApiKey"]
			};
		}

		[Fact]
		public void CanGeocodeAddress()
		{
			geoCoder.GeocodeAsync("1600 pennsylvania ave washington dc").ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Fact]
		public void CanGeocodeNormalizedAddress()
		{
			var address = String.Format("{0} {1}, {2} {3}, {4}", "1600 pennsylvania ave",  "washington", "dc", null, null);
			geoCoder.GeocodeAsync(address).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouse();
			});
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			geoCoder.GeocodeAsync("24 sussex drive ottawa, ontario").ContinueWith(task =>
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertCanadianPrimeMinister();
			});
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			geoCoder.ReverseGeocodeAsync(38.8976777, -77.036517).ContinueWith(task =>
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}

		[Fact]
		public void ShouldNotBlowUpOnBadAddress()
		{
			geoCoder.GeocodeAsync("sdlkf;jasl;kjfldksjfasldf").ContinueWith(task =>
			{
				var addresses = task.Result;
				Assert.Empty(addresses);
			});
		}

		[Fact]
		public void CanGeocodeWithSpecialCharacters()
		{
			geoCoder.GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053").ContinueWith(task =>
			{
				var addresses = task.Result;

				//asserting no exceptions are thrown and that we get something
				Assert.NotEmpty(addresses);
			});
		}

		[Fact]
		public void CanReverseGeocode()
		{
			geoCoder.ReverseGeocodeAsync(38.8976777, -77.036517).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}

		[Theory]
		[InlineData("1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA")]
		[InlineData("miss, MO")]
		//https://github.com/chadly/Geocoding.net/issues/6
		public void CanGeocodeInvalidZipCodes(string address)
		{
			geoCoder.GeocodeAsync(address).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				Assert.NotEmpty(addresses);
			});
		}

		[Theory]
		[InlineData("United States", GoogleAddressType.Country)]
		[InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
		[InlineData("New York, New York", GoogleAddressType.Locality)]
		[InlineData("90210, US", GoogleAddressType.PostalCode)]
		[InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.StreetAddress)]
		public void CanParseAddressTypes(string address, GoogleAddressType type)
		{
			geoCoder.GeocodeAsync(address).ContinueWith(task =>
			{
				Address[] addresses = task.Result.ToArray();
				Assert.Equal(type, addresses[0].AddressTypes[0]);
			});
		}

	}
}