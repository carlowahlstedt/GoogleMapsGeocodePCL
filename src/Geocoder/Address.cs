using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geocoding.Google
{
    public class Address
    {
        #region Properties

        #region Private Properties

        List<AddressComponent> components;
        string formattedAddress;

        #endregion

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("address_components")]
        public List<AddressComponent> Components
        {
            get { return components; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("components");

                if (value.Count < 1)
                    throw new ArgumentException("Value cannot be empty.", "components");

                components = value;
            }
        }

        [JsonProperty("formatted_address")]
        public string FormattedAddress
        {
            get
            {
                return formattedAddress ?? "";
            }
            set
            {
                value = (value ?? "").Trim();

                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("formattedAddress");

                formattedAddress = value;
            }
        }

        [JsonProperty("types")]
        public List<string> Types { get; set; }

        public List<GoogleAddressType> AddressTypes
        {
            get
            {
                List<GoogleAddressType> addressTypes = new List<GoogleAddressType>();
                foreach (var item in Types)
                {
                    addressTypes.Add(EvaluateType(item));
                }
                return addressTypes;
            }
        }

        [JsonProperty("partial_match")]
        public bool IsPartialMatch { get; set; }

        #endregion

        #region Extension

        public AddressComponent this[GoogleAddressType type]
        {
            get { return Components.FirstOrDefault(c => c.AddressTypes.Contains(type)); }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return FormattedAddress;
        }

        #endregion

        #region Private Methods

        /// <remarks>
        /// http://code.google.com/apis/maps/documentation/geocoding/#Types
        /// </remarks>
        public static GoogleAddressType EvaluateType(string type)
        {
            switch (type)
            {
                case "street_address": return GoogleAddressType.StreetAddress;
                case "route": return GoogleAddressType.Route;
                case "intersection": return GoogleAddressType.Intersection;
                case "political": return GoogleAddressType.Political;
                case "country": return GoogleAddressType.Country;
                case "administrative_area_level_1": return GoogleAddressType.AdministrativeAreaLevel1;
                case "administrative_area_level_2": return GoogleAddressType.AdministrativeAreaLevel2;
                case "administrative_area_level_3": return GoogleAddressType.AdministrativeAreaLevel3;
                case "colloquial_area": return GoogleAddressType.ColloquialArea;
                case "locality": return GoogleAddressType.Locality;
                case "sublocality": return GoogleAddressType.SubLocality;
                case "neighborhood": return GoogleAddressType.Neighborhood;
                case "premise": return GoogleAddressType.Premise;
                case "subpremise": return GoogleAddressType.Subpremise;
                case "postal_code": return GoogleAddressType.PostalCode;
                case "natural_feature": return GoogleAddressType.NaturalFeature;
                case "airport": return GoogleAddressType.Airport;
                case "park": return GoogleAddressType.Park;
                case "point_of_interest": return GoogleAddressType.PointOfInterest;
                case "post_box": return GoogleAddressType.PostBox;
                case "street_number": return GoogleAddressType.StreetNumber;
                case "floor": return GoogleAddressType.Floor;
                case "room": return GoogleAddressType.Room;

                default: return GoogleAddressType.Unknown;
            }
        }

        #endregion
    }
}