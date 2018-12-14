using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Util
{
	internal class AlwaysSendList<T> : List<T>
	{
		public AlwaysSendList()
		{
		}

		public AlwaysSendList(IEnumerable<T> collection)
			: base(collection ?? new List<T>())
		{
		}
	}
}
