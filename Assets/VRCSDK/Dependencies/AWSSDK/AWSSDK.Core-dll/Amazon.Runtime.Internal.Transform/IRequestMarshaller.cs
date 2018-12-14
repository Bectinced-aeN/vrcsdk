namespace Amazon.Runtime.Internal.Transform
{
	public interface IRequestMarshaller<R, T> where T : MarshallerContext
	{
		void Marshall(R requestObject, T context);
	}
}
