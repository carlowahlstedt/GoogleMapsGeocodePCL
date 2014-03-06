using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Geocoding.Google
{
	public class AddressComponent
	{
		List<string> types;

		public List<GoogleAddressType> AddressTypes
		{
			get
			{
				List<GoogleAddressType> addressTypes = new List<GoogleAddressType>();
				foreach (var item in Types)
				{
					addressTypes.Add(Address.EvaluateType(item));
				}
				return addressTypes;
			}
		}

		[JsonProperty("types")]
		public List<string> Types
		{
			get
			{
				return types;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("types");

				if (value.Count < 1)
					throw new ArgumentException("Value cannot be empty.", "types");

				types = value;
			}
		}

		[JsonProperty("long_name")]
		public string LongName { get; set; }

		[JsonProperty("short_name")]
		public string ShortName { get; set; }

		public AddressComponent() { }

		public override string ToString()
		{
			return String.Format("{0}: {1}", Types[0], LongName);
		}
	}
}