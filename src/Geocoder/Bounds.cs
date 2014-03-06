using Newtonsoft.Json;
using System;

namespace Geocoding.Google
{
	public class Bounds
	{
		[JsonProperty("southwest")]
		public Location SouthWest { get; set; }

		[JsonProperty("northeast")]
		public Location NorthEast { get; set; }

        public Bounds() { }

        public Bounds(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude)
            : this(new Location(southWestLatitude, southWestLongitude), new Location(northEastLatitude, northEastLongitude)) { }

        public Bounds(Location southWest, Location northEast)
		{
			if (southWest == null)
				throw new ArgumentNullException("southWest");

			if (northEast == null)
				throw new ArgumentNullException("northEast");

			SouthWest = southWest;
			NorthEast = northEast;

            CheckLocation();
		}

        public void CheckLocation()
        {
            if (SouthWest.Latitude > NorthEast.Latitude)
                throw new ArgumentException("southWest latitude cannot be greater than northEast latitude");
        }

		public override bool Equals(object obj)
		{
			return Equals(obj as Bounds);
		}

		public bool Equals(Bounds bounds)
		{
			if (bounds == null)
				return false;

			return (SouthWest.Equals(bounds.SouthWest) && NorthEast.Equals(bounds.NorthEast));
		}

		public override int GetHashCode()
		{
			return SouthWest.GetHashCode() ^ NorthEast.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{0} | {1}", SouthWest, NorthEast);
		}
	}
}
