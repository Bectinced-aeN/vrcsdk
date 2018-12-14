using System.Collections.Generic;

namespace Amazon.Util
{
	internal class Marker<U>
	{
		private List<U> data;

		private string nextToken;

		internal List<U> Data => data;

		internal string NextToken => nextToken;

		internal Marker(List<U> data, string nextToken)
		{
			this.data = data;
			this.nextToken = nextToken;
		}
	}
}
