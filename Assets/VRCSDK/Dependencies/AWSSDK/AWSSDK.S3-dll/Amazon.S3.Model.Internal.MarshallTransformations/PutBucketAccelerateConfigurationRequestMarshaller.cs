using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketAccelerateConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketAccelerateConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketAccelerateConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketAccelerateConfigurationRequest putBucketAccelerateRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketAccelerateRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketAccelerateRequest.BucketName));
			val.AddSubResource("accelerate");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				AccelerateConfiguration accelerateConfiguration = putBucketAccelerateRequest.AccelerateConfiguration;
				if (accelerateConfiguration != null)
				{
					xmlWriter.WriteStartElement("AccelerateConfiguration", "");
					BucketAccelerateStatus status = accelerateConfiguration.Status;
					if (accelerateConfiguration.IsSetBucketAccelerateStatus() && status != null)
					{
						xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(accelerateConfiguration.Status)));
					}
					xmlWriter.WriteEndElement();
				}
			}
			try
			{
				string text = stringWriter.ToString();
				val.set_Content(Encoding.UTF8.GetBytes(text));
				val.get_Headers()["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, fBase64Encode: true);
				val.get_Headers()["Content-MD5"] = value;
				return val;
			}
			catch (EncoderFallbackException ex)
			{
				throw new AmazonServiceException("Unable to marshall request to XML", (Exception)ex);
			}
		}
	}
}
