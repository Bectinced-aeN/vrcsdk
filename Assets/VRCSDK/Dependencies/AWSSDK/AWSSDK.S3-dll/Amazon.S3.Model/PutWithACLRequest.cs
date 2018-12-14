using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public abstract class PutWithACLRequest : AmazonWebServiceRequest
	{
		private List<S3Grant> _grants;

		public List<S3Grant> Grants
		{
			get
			{
				if (_grants == null)
				{
					_grants = new List<S3Grant>();
				}
				return _grants;
			}
			set
			{
				_grants = value;
			}
		}

		protected PutWithACLRequest()
			: this()
		{
		}
	}
}
