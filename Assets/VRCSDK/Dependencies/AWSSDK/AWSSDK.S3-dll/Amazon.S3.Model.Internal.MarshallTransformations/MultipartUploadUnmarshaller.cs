using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class MultipartUploadUnmarshaller : IUnmarshaller<MultipartUpload, XmlUnmarshallerContext>, IUnmarshaller<MultipartUpload, JsonUnmarshallerContext>
	{
		private static MultipartUploadUnmarshaller _instance;

		public static MultipartUploadUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MultipartUploadUnmarshaller();
				}
				return _instance;
			}
		}

		public MultipartUpload Unmarshall(XmlUnmarshallerContext context)
		{
			MultipartUpload multipartUpload = new MultipartUpload();
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
					if (context.TestExpression("Initiated", num))
					{
						multipartUpload.Initiated = DateTimeUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Initiator", num))
					{
						multipartUpload.Initiator = InitiatorUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						multipartUpload.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Owner", num))
					{
						multipartUpload.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						multipartUpload.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("UploadId", num))
					{
						multipartUpload.UploadId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return multipartUpload;
				}
			}
			return multipartUpload;
		}

		public MultipartUpload Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
