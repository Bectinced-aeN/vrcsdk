using System.Collections.Generic;

namespace Amazon.Internal
{
	public interface IRegionEndpointProvider
	{
		IEnumerable<IRegionEndpoint> AllRegionEndpoints
		{
			get;
		}

		IRegionEndpoint GetRegionEndpoint(string regionName);
	}
}
