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
	public class PutBucketNotificationRequestMarshaller : IMarshaller<IRequest, PutBucketNotificationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketNotificationRequest)input);
		}

		public IRequest Marshall(PutBucketNotificationRequest putBucketNotificationRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketNotificationRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketNotificationRequest.BucketName));
			val.AddSubResource("notification");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("NotificationConfiguration", "");
				if (putBucketNotificationRequest.IsSetTopicConfigurations())
				{
					foreach (TopicConfiguration topicConfiguration in putBucketNotificationRequest.TopicConfigurations)
					{
						if (topicConfiguration != null)
						{
							xmlWriter.WriteStartElement("TopicConfiguration", "");
							if (topicConfiguration.IsSetId())
							{
								xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(topicConfiguration.Id));
							}
							if (topicConfiguration.IsSetTopic())
							{
								xmlWriter.WriteElementString("Topic", "", S3Transforms.ToXmlStringValue(topicConfiguration.Topic));
							}
							WriteConfigurationCommon(xmlWriter, topicConfiguration);
							xmlWriter.WriteEndElement();
						}
					}
				}
				if (putBucketNotificationRequest.IsSetQueueConfigurations())
				{
					foreach (QueueConfiguration queueConfiguration in putBucketNotificationRequest.QueueConfigurations)
					{
						if (queueConfiguration != null)
						{
							xmlWriter.WriteStartElement("QueueConfiguration", "");
							if (queueConfiguration.IsSetId())
							{
								xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(queueConfiguration.Id));
							}
							if (queueConfiguration.IsSetQueue())
							{
								xmlWriter.WriteElementString("Queue", "", S3Transforms.ToXmlStringValue(queueConfiguration.Queue));
							}
							WriteConfigurationCommon(xmlWriter, queueConfiguration);
							xmlWriter.WriteEndElement();
						}
					}
				}
				if (putBucketNotificationRequest.IsSetLambdaFunctionConfigurations())
				{
					foreach (LambdaFunctionConfiguration lambdaFunctionConfiguration in putBucketNotificationRequest.LambdaFunctionConfigurations)
					{
						if (lambdaFunctionConfiguration != null)
						{
							xmlWriter.WriteStartElement("CloudFunctionConfiguration", "");
							if (lambdaFunctionConfiguration.IsSetId())
							{
								xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(lambdaFunctionConfiguration.Id));
							}
							if (lambdaFunctionConfiguration.IsSetFunctionArn())
							{
								xmlWriter.WriteElementString("CloudFunction", "", S3Transforms.ToXmlStringValue(lambdaFunctionConfiguration.FunctionArn));
							}
							WriteConfigurationCommon(xmlWriter, lambdaFunctionConfiguration);
							xmlWriter.WriteEndElement();
						}
					}
				}
				xmlWriter.WriteEndElement();
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

		private static void WriteConfigurationCommon(XmlWriter xmlWriter, NotificationConfiguration notificationConfiguration)
		{
			if (notificationConfiguration.IsSetEvents())
			{
				foreach (EventType @event in notificationConfiguration.Events)
				{
					xmlWriter.WriteElementString("Event", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(@event)));
				}
			}
			if (notificationConfiguration.IsSetFilter())
			{
				xmlWriter.WriteStartElement("Filter", "");
				Filter filter = notificationConfiguration.Filter;
				if (filter.IsSetS3KeyFilter())
				{
					xmlWriter.WriteStartElement("S3Key", "");
					S3KeyFilter s3KeyFilter = filter.S3KeyFilter;
					if (s3KeyFilter.IsSetFilterRules())
					{
						foreach (FilterRule filterRule in s3KeyFilter.FilterRules)
						{
							if (filterRule != null)
							{
								xmlWriter.WriteStartElement("FilterRule", "");
								xmlWriter.WriteElementString("Name", filterRule.Name);
								xmlWriter.WriteElementString("Value", filterRule.Value);
								xmlWriter.WriteEndElement();
							}
						}
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
		}
	}
}
