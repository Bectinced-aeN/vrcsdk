namespace Amazon.Runtime.Internal.Transform
{
	public abstract class MarshallerContext
	{
		public IRequest Request
		{
			get;
			private set;
		}

		protected MarshallerContext(IRequest request)
		{
			Request = request;
		}
	}
}
