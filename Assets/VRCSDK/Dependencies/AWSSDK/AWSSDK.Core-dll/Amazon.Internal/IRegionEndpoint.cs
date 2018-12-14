namespace Amazon.Internal
{
	public interface IRegionEndpoint
	{
		string RegionName
		{
			get;
		}

		string DisplayName
		{
			get;
		}

		RegionEndpoint.Endpoint GetEndpointForService(string serviceName, bool dualStack);
	}
}
