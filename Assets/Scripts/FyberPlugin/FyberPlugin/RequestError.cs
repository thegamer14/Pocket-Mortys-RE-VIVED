namespace FyberPlugin
{
	public sealed class RequestError
	{
		public string Description { get; set; }

		public static RequestError DEVICE_NOT_SUPPORTED
		{
			get
			{
				return new RequestError("Only devices running Android API level 10 and above are supported");
			}
		}

		public static RequestError CONNECTION_ERROR
		{
			get
			{
				return new RequestError("Internet connection error");
			}
		}

		public static RequestError UNKNOWN_ERROR
		{
			get
			{
				return new RequestError("An unknown error occurred");
			}
		}

		public static RequestError SDK_NOT_STARTED
		{
			get
			{
				return new RequestError("You need to start the SDK");
			}
		}

		public static RequestError NULL_CONTEXT_REFERENCE
		{
			get
			{
				return new RequestError("The context reference cannot be null");
			}
		}

		public static RequestError SECURITY_TOKEN_NOT_PROVIDED
		{
			get
			{
				return new RequestError("The security token was not provided when starting the SDK.");
			}
		}

		public static RequestError ERROR_REQUESTING_ADS
		{
			get
			{
				return new RequestError("An error happened while trying to retrieve ads");
			}
		}

		public static RequestError UNABLE_TO_REQUEST_ADS
		{
			get
			{
				return new RequestError("The SDK is unable to request right now because it is either already performing a request or showing an ad");
			}
		}

		private RequestError(string description)
		{
			Description = description;
		}

		internal static RequestError FromNative(int ordinal)
		{
			switch (ordinal)
			{
			case 0:
				return DEVICE_NOT_SUPPORTED;
			case 1:
				return CONNECTION_ERROR;
			default:
				return UNKNOWN_ERROR;
			case 3:
				return SDK_NOT_STARTED;
			case 5:
				return NULL_CONTEXT_REFERENCE;
			case 6:
				return SECURITY_TOKEN_NOT_PROVIDED;
			case 7:
				return ERROR_REQUESTING_ADS;
			case 8:
				return UNABLE_TO_REQUEST_ADS;
			}
		}
	}
}
