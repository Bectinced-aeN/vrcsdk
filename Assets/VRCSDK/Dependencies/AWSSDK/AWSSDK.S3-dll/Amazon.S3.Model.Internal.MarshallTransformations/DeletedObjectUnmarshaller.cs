using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeletedObjectUnmarshaller : IUnmarshaller<DeletedObject, XmlUnmarshallerContext>, IUnmarshaller<DeletedObject, JsonUnmarshallerContext>
	{
		private static DeletedObjectUnmarshaller _instance;

		public static DeletedObjectUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeletedObjectUnmarshaller();
				}
				return _instance;
			}
		}

		public DeletedObject Unmarshall(XmlUnmarshallerContext context)
		{
			DeletedObject deletedObject = new DeletedObject();
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
					if (context.TestExpression("DeleteMarker", num))
					{
						deletedObject.DeleteMarker = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("DeleteMarkerVersionId", num))
					{
						deletedObject.DeleteMarkerVersionId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Key", num))
					{
						deletedObject.Key = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("VersionId", num))
					{
						deletedObject.VersionId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return deletedObject;
				}
			}
			return deletedObject;
		}

		public DeletedObject Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
