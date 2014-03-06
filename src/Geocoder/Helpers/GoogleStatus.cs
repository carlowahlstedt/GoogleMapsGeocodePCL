using System;

namespace Geocoding.Google
{
	/// <remarks>
	/// http://code.google.com/apis/maps/documentation/geocoding/#StatusCodes
	/// </remarks>
	public enum GoogleStatus
	{
		Error,
		Ok,
		ZeroResults,
		OverQueryLimit,
		RequestDenied,
		InvalidRequest
	}
}