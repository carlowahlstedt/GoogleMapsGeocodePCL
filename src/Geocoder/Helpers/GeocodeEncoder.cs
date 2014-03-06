using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geocoding.Google
{
    public static class GeocodeEncoder
    {
        /// <summary>
        /// There is no HttpUtility.UrlEncode in PCL, so this method was created.
        /// http://stackoverflow.com/questions/11473031/portable-class-library-httputility-urlencode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        {
            //looks like we only need to do one of these
            //value = Uri.EscapeUriString(value);
            value = Uri.EscapeDataString(value);

            // While the to above do most of the encoding, we still need to manually do these:
            // http://www.w3schools.com/tags/ref_urlencode.asp
            value = value.Replace("-", "%2D");
            value = value.Replace("_", "%5F");
            value = value.Replace(".", "%2E");
            value = value.Replace("!", "%21");
            value = value.Replace("~", "%7E");
            value = value.Replace("*", "%2A");
            value = value.Replace("'", "%27");
            value = value.Replace("(", "%28");
            value = value.Replace(")", "%29");
            
            return value;
        }
    }
}
