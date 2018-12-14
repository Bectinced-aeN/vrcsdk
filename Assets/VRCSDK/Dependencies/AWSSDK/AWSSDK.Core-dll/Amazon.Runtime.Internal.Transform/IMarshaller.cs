namespace Amazon.Runtime.Internal.Transform
{
	public interface IMarshaller<T, R>
	{
		T Marshall(R input);
	}
}
