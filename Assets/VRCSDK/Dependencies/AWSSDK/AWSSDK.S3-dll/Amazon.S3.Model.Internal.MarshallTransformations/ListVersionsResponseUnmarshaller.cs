using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListVersionsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListVersionsResponseUnmarshaller _instance;

		public static ListVersionsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListVersionsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListVersionsResponse listVersionsResponse = new ListVersionsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listVersionsResponse);
				}
			}
			return listVersionsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListVersionsResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("KeyMarker", num))
					{
						response.KeyMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Delimiter", num))
					{
						response.Delimiter = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("VersionIdMarker", num))
					{
						response.VersionIdMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextKeyMarker", num))
					{
						response.NextKeyMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextVersionIdMarker", num))
					{
						response.NextVersionIdMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Version", num))
					{
						response.Versions.Add(VersionsItemUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("DeleteMarker", num))
					{
						S3ObjectVersion s3ObjectVersion = VersionsItemUnmarshaller.Instance.Unmarshall(context);
						s3ObjectVersion.IsDeleteMarker = true;
						response.Versions.Add(s3ObjectVersion);
					}
					else if (context.TestExpression("Name", num))
					{
						response.Name = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						response.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxKeys", num))
					{
						response.MaxKeys = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("CommonPrefixes", num))
					{
						string text = CommonPrefixesItemUnmarshaller.Instance.Unmarshall(context);
						if (text != null)
						{
							response.CommonPrefixes.Add(text);
						}
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					break;
				}
			}
		}
	}
}
