namespace Amazon.Runtime.Internal.Transform
{
	public class ResponseMetadataUnmarshaller : IUnmarshaller<ResponseMetadata, XmlUnmarshallerContext>, IUnmarshaller<ResponseMetadata, JsonUnmarshallerContext>
	{
		private static ResponseMetadataUnmarshaller _instance = new ResponseMetadataUnmarshaller();

		public static ResponseMetadataUnmarshaller Instance => _instance;

		private ResponseMetadataUnmarshaller()
		{
		}

		public static ResponseMetadataUnmarshaller GetInstance()
		{
			return Instance;
		}

		public ResponseMetadata Unmarshall(XmlUnmarshallerContext context)
		{
			ResponseMetadata responseMetadata = new ResponseMetadata();
			int currentDepth = context.CurrentDepth;
			while (currentDepth <= context.CurrentDepth)
			{
				context.Read();
				if (context.IsStartElement)
				{
					if (context.TestExpression("ResponseMetadata/RequestId"))
					{
						responseMetadata.RequestId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else
					{
						responseMetadata.Metadata.Add(context.CurrentPath.Substring(context.CurrentPath.LastIndexOf('/') + 1), StringUnmarshaller.GetInstance().Unmarshall(context));
					}
				}
			}
			return responseMetadata;
		}

		public ResponseMetadata Unmarshall(JsonUnmarshallerContext context)
		{
			ResponseMetadata responseMetadata = new ResponseMetadata();
			int currentDepth = context.CurrentDepth;
			while (context.CurrentDepth >= currentDepth)
			{
				context.Read();
				if (context.TestExpression("ResponseMetadata/RequestId"))
				{
					responseMetadata.RequestId = StringUnmarshaller.GetInstance().Unmarshall(context);
				}
			}
			return responseMetadata;
		}
	}
}
