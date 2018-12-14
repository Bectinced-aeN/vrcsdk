using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AbortIncompleteMultipartUploadUnmarshaller : IUnmarshaller<LifecycleRuleAbortIncompleteMultipartUpload, XmlUnmarshallerContext>, IUnmarshaller<LifecycleRuleAbortIncompleteMultipartUpload, JsonUnmarshallerContext>
	{
		private static AbortIncompleteMultipartUploadUnmarshaller _instance;

		public static AbortIncompleteMultipartUploadUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AbortIncompleteMultipartUploadUnmarshaller();
				}
				return _instance;
			}
		}

		public LifecycleRuleAbortIncompleteMultipartUpload Unmarshall(XmlUnmarshallerContext context)
		{
			LifecycleRuleAbortIncompleteMultipartUpload lifecycleRuleAbortIncompleteMultipartUpload = new LifecycleRuleAbortIncompleteMultipartUpload();
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
					if (context.TestExpression("DaysAfterInitiation", num))
					{
						lifecycleRuleAbortIncompleteMultipartUpload.DaysAfterInitiation = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return lifecycleRuleAbortIncompleteMultipartUpload;
				}
			}
			return lifecycleRuleAbortIncompleteMultipartUpload;
		}

		public LifecycleRuleAbortIncompleteMultipartUpload Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
