using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocoding.Google
{
    public class Geometry
    {
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }
        
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }
        
        [JsonProperty("viewport")]
        public Bounds Viewport { get; set; }
    }
}
