using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketWebsiteRequestMarshaller : IMarshaller<IRequest, PutBucketWebsiteRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketWebsiteRequest)input);
		}

		public IRequest Marshall(PutBucketWebsiteRequest putBucketWebsiteRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putBucketWebsiteRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			val.set_ResourcePath("/" + S3Transforms.ToStringValue(putBucketWebsiteRequest.BucketName));
			val.AddSubResource("website");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				WebsiteConfiguration websiteConfiguration = putBucketWebsiteRequest.WebsiteConfiguration;
				if (websiteConfiguration != null)
				{
					xmlWriter.WriteStartElement("WebsiteConfiguration", "");
					if (websiteConfiguration != null)
					{
						string errorDocument = websiteConfiguration.ErrorDocument;
						if (errorDocument != null)
						{
							xmlWriter.WriteStartElement("ErrorDocument", "");
							xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(errorDocument));
							xmlWriter.WriteEndElement();
						}
					}
					if (websiteConfiguration != null)
					{
						string indexDocumentSuffix = websiteConfiguration.IndexDocumentSuffix;
						if (indexDocumentSuffix != null)
						{
							xmlWriter.WriteStartElement("IndexDocument", "");
							xmlWriter.WriteElementString("Suffix", "", S3Transforms.ToXmlStringValue(indexDocumentSuffix));
							xmlWriter.WriteEndElement();
						}
					}
					if (websiteConfiguration != null)
					{
						RoutingRuleRedirect redirectAllRequestsTo = websiteConfiguration.RedirectAllRequestsTo;
						if (redirectAllRequestsTo != null)
						{
							xmlWriter.WriteStartElement("RedirectAllRequestsTo", "");
							if (redirectAllRequestsTo.IsSetHostName())
							{
								xmlWriter.WriteElementString("HostName", "", S3Transforms.ToXmlStringValue(redirectAllRequestsTo.HostName));
							}
							if (redirectAllRequestsTo.IsSetHttpRedirectCode())
							{
								xmlWriter.WriteElementString("HttpRedirectCode", "", S3Transforms.ToXmlStringValue(redirectAllRequestsTo.HttpRedirectCode));
							}
							if (redirectAllRequestsTo.IsSetProtocol())
							{
								xmlWriter.WriteElementString("Protocol", "", S3Transforms.ToXmlStringValue(redirectAllRequestsTo.Protocol));
							}
							if (redirectAllRequestsTo.IsSetReplaceKeyPrefixWith())
							{
								xmlWriter.WriteElementString("ReplaceKeyPrefixWith", "", S3Transforms.ToXmlStringValue(redirectAllRequestsTo.ReplaceKeyPrefixWith));
							}
							if (redirectAllRequestsTo.IsSetReplaceKeyWith())
							{
								xmlWriter.WriteElementString("ReplaceKeyWith", "", S3Transforms.ToXmlStringValue(redirectAllRequestsTo.ReplaceKeyWith));
							}
							xmlWriter.WriteEndElement();
						}
					}
					if (websiteConfiguration != null)
					{
						List<RoutingRule> routingRules = websiteConfiguration.RoutingRules;
						if (routingRules != null && routingRules.Count > 0)
						{
							xmlWriter.WriteStartElement("RoutingRules", "");
							foreach (RoutingRule item in routingRules)
							{
								xmlWriter.WriteStartElement("RoutingRule", "");
								if (item != null)
								{
									RoutingRuleCondition condition = item.Condition;
									if (condition != null)
									{
										xmlWriter.WriteStartElement("Condition", "");
										if (condition.IsSetHttpErrorCodeReturnedEquals())
										{
											xmlWriter.WriteElementString("HttpErrorCodeReturnedEquals", "", S3Transforms.ToXmlStringValue(condition.HttpErrorCodeReturnedEquals));
										}
										if (condition.IsSetKeyPrefixEquals())
										{
											xmlWriter.WriteElementString("KeyPrefixEquals", "", S3Transforms.ToXmlStringValue(condition.KeyPrefixEquals));
										}
										xmlWriter.WriteEndElement();
									}
								}
								if (item != null)
								{
									RoutingRuleRedirect redirect = item.Redirect;
									if (redirect != null)
									{
										xmlWriter.WriteStartElement("Redirect", "");
										if (redirect.IsSetHostName())
										{
											xmlWriter.WriteElementString("HostName", "", S3Transforms.ToXmlStringValue(redirect.HostName));
										}
										if (redirect.IsSetHttpRedirectCode())
										{
											xmlWriter.WriteElementString("HttpRedirectCode", "", S3Transforms.ToXmlStringValue(redirect.HttpRedirectCode));
										}
										if (redirect.IsSetProtocol())
										{
											xmlWriter.WriteElementString("Protocol", "", S3Transforms.ToXmlStringValue(redirect.Protocol));
										}
										if (redirect.IsSetReplaceKeyPrefixWith())
										{
											xmlWriter.WriteElementString("ReplaceKeyPrefixWith", "", S3Transforms.ToXmlStringValue(redirect.ReplaceKeyPrefixWith));
										}
										if (redirect.IsSetReplaceKeyWith())
										{
											xmlWriter.WriteElementString("ReplaceKeyWith", "", S3Transforms.ToXmlStringValue(redirect.ReplaceKeyWith));
										}
										xmlWriter.WriteEndElement();
									}
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
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
