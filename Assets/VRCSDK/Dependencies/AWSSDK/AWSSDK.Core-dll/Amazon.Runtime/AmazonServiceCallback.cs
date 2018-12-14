namespace Amazon.Runtime
{
	public delegate void AmazonServiceCallback<TRequest, TResponse>(AmazonServiceResult<TRequest, TResponse> responseObject) where TRequest : AmazonWebServiceRequest where TResponse : AmazonWebServiceResponse;
}
