using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Geocoding.Google
{
    /// <remarks>
    /// http://code.google.com/apis/maps/documentation/geocoding/
    /// </remarks>
    public class GoogleGeocoder : IAsyncGeocoder
    {
        public string ApiKey { get; set; }
        public string Language { get; set; }
        public string RegionBias { get; set; }
        public Bounds BoundsBias { get; set; }

        public string ServiceUrl
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("https://maps.googleapis.com/maps/api/geocode/json?{0}={1}&sensor=false");

                if (!String.IsNullOrEmpty(Language))
                {
                    builder.Append("&language=");
                    builder.Append(GeocodeEncoder.UrlEncode(Language));
                }

                if (!String.IsNullOrEmpty(RegionBias))
                {
                    builder.Append("&region=");
                    builder.Append(GeocodeEncoder.UrlEncode(RegionBias));
                }

                if (!String.IsNullOrEmpty(ApiKey))
                {
                    builder.Append("&key=");
                    builder.Append(GeocodeEncoder.UrlEncode(ApiKey));
                }

                if (BoundsBias != null)
                {
                    builder.Append("&bounds=");
                    builder.Append(BoundsBias.SouthWest.Latitude.ToString(CultureInfo.InvariantCulture));
                    builder.Append(",");
                    builder.Append(BoundsBias.SouthWest.Longitude.ToString(CultureInfo.InvariantCulture));
                    builder.Append("|");
                    builder.Append(BoundsBias.NorthEast.Latitude.ToString(CultureInfo.InvariantCulture));
                    builder.Append(",");
                    builder.Append(BoundsBias.NorthEast.Longitude.ToString(CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        public Task<IEnumerable<Address>> GeocodeAsync(string address)
        {
            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            HttpWebRequest request = BuildWebRequest("address", GeocodeEncoder.UrlEncode(address));
            return ProcessRequestAsync(request);
        }

        public Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            HttpWebRequest request = BuildWebRequest("address", GeocodeEncoder.UrlEncode(address));
            return ProcessRequestAsync(request, cancellationToken);
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude)
        {
            HttpWebRequest request = BuildWebRequest("latlng", BuildGeolocation(latitude, longitude));
            return ProcessRequestAsync(request);
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            HttpWebRequest request = BuildWebRequest("latlng", BuildGeolocation(latitude, longitude));
            return ProcessRequestAsync(request, cancellationToken);
        }

        private string BuildAddress(string street, string city, string state, string postalCode, string country)
        {
            return String.Format("{0} {1}, {2} {3}, {4}", street, city, state, postalCode, country);
        }

        private string BuildGeolocation(double latitude, double longitude)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude);
        }

        private Task<IEnumerable<Address>> ProcessRequestAsync(HttpWebRequest request, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken != null)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
                cancellationToken.Value.Register(() => request.Abort());
            }

            var requestState = new RequestState(request, cancellationToken);
            return Task.Factory.FromAsync(
                (callback, asyncState) => SendRequestAsync((RequestState)asyncState, callback),
                result => ProcessResponseAsync((RequestState)result.AsyncState, result),
                requestState
            );
        }

        private IAsyncResult SendRequestAsync(RequestState requestState, AsyncCallback callback)
        {
            try
            {
                return requestState.request.BeginGetResponse(callback, requestState);
            }
            catch (Exception ex)
            {
                throw new GoogleGeocodingException(ex);
            }
        }

        private IEnumerable<Address> ProcessResponseAsync(RequestState requestState, IAsyncResult result)
        {
            if (requestState.cancellationToken != null)
                requestState.cancellationToken.Value.ThrowIfCancellationRequested();

            try
            {
                using (var response = (HttpWebResponse)requestState.request.EndGetResponse(result))
                {
                    return ProcessWebResponse(response);
                }
            }
            catch (GoogleGeocodingException)
            {
                //let these pass through
                throw;
            }
            catch (Exception ex)
            {
                //wrap in google exception
                throw new GoogleGeocodingException(ex);
            }
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string address)
        {
            return GeocodeAsync(address)
                .ContinueWith(task => task.Result.Cast<Address>());
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string address, CancellationToken cancellationToken)
        {
            return GeocodeAsync(address, cancellationToken)
                .ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country)
        {
            return GeocodeAsync(BuildAddress(street, city, state, postalCode, country))
                .ContinueWith(task => task.Result.Cast<Address>());
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken)
        {
            return GeocodeAsync(BuildAddress(street, city, state, postalCode, country), cancellationToken)
                .ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.ReverseGeocodeAsync(double latitude, double longitude)
        {
            return ReverseGeocodeAsync(latitude, longitude)
                .ContinueWith(task => task.Result.Cast<Address>());
        }

        Task<IEnumerable<Address>> IAsyncGeocoder.ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            return ReverseGeocodeAsync(latitude, longitude, cancellationToken)
                .ContinueWith(task => task.Result.Cast<Address>(), cancellationToken);
        }

        private HttpWebRequest BuildWebRequest(string type, string value)
        {
            string url = String.Format(ServiceUrl, type, value);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            return req;
        }

        private IEnumerable<Address> ProcessWebResponse(WebResponse response)
        {
            string json = string.Empty;
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                while (!stream.EndOfStream)
                {
                    json = stream.ReadToEnd();
                }
            }
            var result = JsonConvert.DeserializeObject<GoogleGeocodeResult>(json);

            if (result.StatusResults != GoogleStatus.Ok && result.StatusResults != GoogleStatus.ZeroResults)
                throw new GoogleGeocodingException(result.StatusResults);

            if (result.StatusResults == GoogleStatus.Ok)
                return result.Results;

            return new Address[0];
        }

        protected class RequestState
        {
            public readonly HttpWebRequest request;
            public readonly CancellationToken? cancellationToken;

            public RequestState(HttpWebRequest request, CancellationToken? cancellationToken)
            {
                if (request == null) throw new ArgumentNullException("request");

                this.request = request;
                this.cancellationToken = cancellationToken;
            }
        }
    }
}