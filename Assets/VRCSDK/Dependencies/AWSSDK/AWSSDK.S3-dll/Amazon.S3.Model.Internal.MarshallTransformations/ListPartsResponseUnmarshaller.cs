using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListPartsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListPartsResponseUnmarshaller _instance;

		public static ListPartsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListPartsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListPartsResponse listPartsResponse = new ListPartsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listPartsResponse);
				}
			}
			return listPartsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListPartsResponse response)
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
					if (context.TestExpression("Bucket", num))
					{
						response.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						response.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("UploadId", num))
					{
						response.UploadId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("PartNumberMarker", num))
					{
						response.PartNumberMarker = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextPartNumberMarker", num))
					{
						response.NextPartNumberMarker = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxParts", num))
					{
						response.MaxParts = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Part", num))
					{
						response.Parts.Add(PartDetailUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Initiator", num))
					{
						response.Initiator = InitiatorUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Owner", num))
					{
						response.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						response.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return;
				}
			}
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-abort-date"))
			{
				response.AbortDate = S3Transforms.ToDateTime(responseData.GetHeaderValue("x-amz-abort-date"));
			}
			if (responseData.IsHeaderPresent("x-amz-abort-rule-id"))
			{
				response.AbortRuleId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-abort-rule-id"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
		}
	}
}
