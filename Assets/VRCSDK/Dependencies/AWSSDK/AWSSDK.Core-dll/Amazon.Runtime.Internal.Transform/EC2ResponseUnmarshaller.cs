using System.IO;

namespace Amazon.Runtime.Internal.Transform
{
	public abstract class EC2ResponseUnmarshaller : XmlResponseUnmarshaller
	{
		public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
		{
			AmazonWebServiceResponse amazonWebServiceResponse = base.Unmarshall(input);
			if (amazonWebServiceResponse.ResponseMetadata == null)
			{
				amazonWebServiceResponse.ResponseMetadata = new ResponseMetadata();
			}
			EC2UnmarshallerContext eC2UnmarshallerContext = input as EC2UnmarshallerContext;
			if (eC2UnmarshallerContext != null && !string.IsNullOrEmpty(eC2UnmarshallerContext.RequestId))
			{
				amazonWebServiceResponse.ResponseMetadata.RequestId = eC2UnmarshallerContext.RequestId;
			}
			return amazonWebServiceResponse;
		}

		protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response)
		{
			return new EC2UnmarshallerContext(responseStream, maintainResponseBody, response);
		}
	}
}
