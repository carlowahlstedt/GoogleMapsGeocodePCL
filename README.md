GoogleMapsGeocodePCL
====================

.NET Portable Class Library for Geocoding in Google Maps

This is based on <a href="https://github.com/chadly/Geocoding.net">Geocoding.net</a> but ported to be a Portable Class Library. Yahoo! and Bing maps support is gone, but Google Maps remains. Support for the Business Key (<a href="https://developers.google.com/maps/documentation/business/">Google Maps API for Business</a>) has been removed due to the fact that there is <a href="http://pclcontrib.codeplex.com/">no direct PCL support for cryptogrophy</a> and you <a href="https://developers.google.com/maps/documentation/business/webservices/auth">may not want to use it client side</a> anyway.
