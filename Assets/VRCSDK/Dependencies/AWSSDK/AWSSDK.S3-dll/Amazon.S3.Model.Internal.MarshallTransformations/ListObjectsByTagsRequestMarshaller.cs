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
	public class ListObjectsByTagsRequestMarshaller : IMarshaller<IRequest, ListObjectsByTagsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListObjectsByTagsRequest)input);
		}

		public IRequest Marshall(ListObjectsByTagsRequest listObjectsByTagsRequest)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			IRequest val = new DefaultRequest(listObjectsByTagsRequest, "AmazonS3");
			val.set_HttpMethod("POST");
			val.set_ResourcePath(string.Format(CultureInfo.InvariantCulture, "/{0}", S3Transforms.ToStringValue(listObjectsByTagsRequest.BucketName)));
			val.AddSubResource("tag-query");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				SerializeTagsQuery(xmlWriter, listObjectsByTagsRequest.TagQuery);
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
				throw new AmazonServiceException("Unable to marhsall request to XML", (Exception)ex);
			}
		}

		private static void SerializeTagsQuery(XmlWriter xmlWriter, TagQuery tagQuery)
		{
			xmlWriter.WriteStartElement("TagQuery", "");
			if (tagQuery.IsSetMaxKeys())
			{
				xmlWriter.WriteElementString("MaxKeys", "", S3Transforms.ToXmlStringValue(tagQuery.MaxKeys.Value));
			}
			xmlWriter.WriteElementString("IncludeTags", "", tagQuery.IncludeTags ? "true" : "false");
			if (tagQuery.IsSetContinuationToken())
			{
				xmlWriter.WriteElementString("ContinuationToken", "", tagQuery.ContinuationToken);
			}
			xmlWriter.WriteStartElement("And", "");
			foreach (TagQueryFilter item in tagQuery.And)
			{
				SerializeTagQueryFilter(xmlWriter, item);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		private static void SerializeTagQueryFilter(XmlWriter xmlWriter, TagQueryFilter filter)
		{
			xmlWriter.WriteStartElement("Filter", "");
			xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(filter.Key));
			xmlWriter.WriteStartElement("Or", "");
			foreach (string item in filter.Or)
			{
				xmlWriter.WriteElementString("Value", "", S3Transforms.ToXmlStringValue(item));
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}
	}
}
