using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocoding.Google
{
    public class GoogleGeocodeResult
    {
        [JsonProperty("results")]
        public List<Address> Results { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }

        public GoogleStatus StatusResults
        {
            get
            {
                return EvaluateStatus(Status);
            }
        }

        /// <remarks>
        /// http://code.google.com/apis/maps/documentation/geocoding/#StatusCodes
        /// </remarks>
        private GoogleStatus EvaluateStatus(string status)
        {
            switch (status)
            {
                case "OK": return GoogleStatus.Ok;
                case "ZERO_RESULTS": return GoogleStatus.ZeroResults;
                case "OVER_QUERY_LIMIT": return GoogleStatus.OverQueryLimit;
                case "REQUEST_DENIED": return GoogleStatus.RequestDenied;
                case "INVALID_REQUEST": return GoogleStatus.InvalidRequest;
                default: return GoogleStatus.Error;
            }
        }

    }
}
