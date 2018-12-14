using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class StorageClassAnalysisUnmarshaller : IUnmarshaller<StorageClassAnalysis, XmlUnmarshallerContext>, IUnmarshaller<StorageClassAnalysis, JsonUnmarshallerContext>
	{
		private static StorageClassAnalysisUnmarshaller _instance;

		public static StorageClassAnalysisUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StorageClassAnalysisUnmarshaller();
				}
				return _instance;
			}
		}

		public StorageClassAnalysis Unmarshall(XmlUnmarshallerContext context)
		{
			StorageClassAnalysis storageClassAnalysis = new StorageClassAnalysis();
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
					if (context.TestExpression("DataExport", num))
					{
						storageClassAnalysis.DataExport = StorageClassAnalysisDataExportUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return storageClassAnalysis;
				}
			}
			return storageClassAnalysis;
		}

		public StorageClassAnalysis Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
