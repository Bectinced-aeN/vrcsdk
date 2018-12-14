using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Util;

namespace Amazon.S3.Internal
{
	public class AmazonS3RedirectHandler : RedirectHandler
	{
		protected override void FinalizeForRedirect(IExecutionContext executionContext, string redirectedLocation)
		{
			IRequest request = executionContext.get_RequestContext().get_Request();
			if (request.get_UseChunkEncoding() && request.get_Headers().ContainsKey("X-Amz-Decoded-Content-Length"))
			{
				request.get_Headers()["Content-Length"] = request.get_Headers()["X-Amz-Decoded-Content-Length"];
			}
			if (request.get_Headers().ContainsKey("host"))
			{
				request.get_Headers().Remove("host");
			}
			this.FinalizeForRedirect(executionContext, redirectedLocation);
			AmazonS3KmsHandler.EvaluateIfSigV4Required(executionContext.get_RequestContext().get_Request());
			AmazonS3Uri amazonS3Uri = new AmazonS3Uri(redirectedLocation);
			if (AWSConfigsS3.UseSignatureVersion4 || request.get_UseSigV4() || amazonS3Uri.Region.GetEndpointForService("s3").get_SignatureVersionOverride() == "4" || amazonS3Uri.Region.GetEndpointForService("s3").get_SignatureVersionOverride() == null)
			{
				request.set_AuthenticationRegion(amazonS3Uri.Region.get_SystemName());
				Signer.SignRequest(executionContext.get_RequestContext());
			}
		}

		public AmazonS3RedirectHandler()
			: this()
		{
		}
	}
}
