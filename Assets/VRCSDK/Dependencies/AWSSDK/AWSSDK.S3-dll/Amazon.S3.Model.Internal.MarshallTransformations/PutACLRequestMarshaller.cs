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
	public class PutACLRequestMarshaller : IMarshaller<IRequest, PutACLRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutACLRequest)input);
		}

		public IRequest Marshall(PutACLRequest putObjectAclRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(putObjectAclRequest, "AmazonS3");
			val.set_HttpMethod("PUT");
			if (putObjectAclRequest.IsSetCannedACL())
			{
				val.get_Headers().Add("x-amz-acl", S3Transforms.ToStringValue(ConstantClass.op_Implicit(putObjectAclRequest.CannedACL)));
			}
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectAclRequest.BucketName), S3Transforms.ToStringValue(putObjectAclRequest.Key)));
			val.AddSubResource("acl");
			if (putObjectAclRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(putObjectAclRequest.VersionId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				S3AccessControlList accessControlList = putObjectAclRequest.AccessControlList;
				if (accessControlList != null)
				{
					xmlWriter.WriteStartElement("AccessControlPolicy", "");
					List<S3Grant> grants = accessControlList.Grants;
					if (grants != null && grants.Count > 0)
					{
						xmlWriter.WriteStartElement("AccessControlList", "");
						foreach (S3Grant item in grants)
						{
							xmlWriter.WriteStartElement("Grant", "");
							if (item != null)
							{
								S3Grantee grantee = item.Grantee;
								if (grantee != null)
								{
									xmlWriter.WriteStartElement("Grantee", "");
									if (grantee.IsSetType())
									{
										xmlWriter.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", ((object)grantee.Type).ToString());
									}
									if (grantee.IsSetDisplayName())
									{
										xmlWriter.WriteElementString("DisplayName", "", S3Transforms.ToXmlStringValue(grantee.DisplayName));
									}
									if (grantee.IsSetEmailAddress())
									{
										xmlWriter.WriteElementString("EmailAddress", "", S3Transforms.ToXmlStringValue(grantee.EmailAddress));
									}
									if (grantee.IsSetCanonicalUser())
									{
										xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(grantee.CanonicalUser));
									}
									if (grantee.IsSetURI())
									{
										xmlWriter.WriteElementString("URI", "", S3Transforms.ToXmlStringValue(grantee.URI));
									}
									xmlWriter.WriteEndElement();
								}
								if (item.IsSetPermission())
								{
									xmlWriter.WriteElementString("Permission", "", S3Transforms.ToXmlStringValue(ConstantClass.op_Implicit(item.Permission)));
								}
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
						Owner owner = accessControlList.Owner;
						if (owner != null)
						{
							xmlWriter.WriteStartElement("Owner", "");
							if (owner.IsSetDisplayName())
							{
								xmlWriter.WriteElementString("DisplayName", "", S3Transforms.ToXmlStringValue(owner.DisplayName));
							}
							if (owner.IsSetId())
							{
								xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(owner.Id));
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
