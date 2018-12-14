using System;
using System.Collections.Generic;

namespace Amazon.Runtime
{
	[Serializable]
	public class ResponseMetadata
	{
		private string requestIdField;

		private IDictionary<string, string> _metadata;

		public string RequestId
		{
			get
			{
				return requestIdField;
			}
			set
			{
				requestIdField = value;
			}
		}

		public IDictionary<string, string> Metadata
		{
			get
			{
				if (_metadata == null)
				{
					_metadata = new Dictionary<string, string>();
				}
				return _metadata;
			}
		}
	}
}
