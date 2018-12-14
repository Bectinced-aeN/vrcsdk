using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Transform
{
	public class KeyValueUnmarshaller<K, V, KUnmarshaller, VUnmarshaller> : IUnmarshaller<KeyValuePair<K, V>, XmlUnmarshallerContext>, IUnmarshaller<KeyValuePair<K, V>, JsonUnmarshallerContext> where KUnmarshaller : IUnmarshaller<K, XmlUnmarshallerContext>, IUnmarshaller<K, JsonUnmarshallerContext> where VUnmarshaller : IUnmarshaller<V, XmlUnmarshallerContext>, IUnmarshaller<V, JsonUnmarshallerContext>
	{
		private KUnmarshaller keyUnmarshaller;

		private VUnmarshaller valueUnmarshaller;

		public KeyValueUnmarshaller(KUnmarshaller keyUnmarshaller, VUnmarshaller valueUnmarshaller)
		{
			this.keyUnmarshaller = keyUnmarshaller;
			this.valueUnmarshaller = valueUnmarshaller;
		}

		public KeyValuePair<K, V> Unmarshall(XmlUnmarshallerContext context)
		{
			K key = default(K);
			V value = default(V);
			int currentDepth = context.CurrentDepth;
			int startingStackDepth = currentDepth + 1;
			while (context.Read())
			{
				if (context.TestExpression("key", startingStackDepth))
				{
					key = keyUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("name", startingStackDepth))
				{
					key = keyUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("value", startingStackDepth))
				{
					value = valueUnmarshaller.Unmarshall(context);
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					break;
				}
			}
			return new KeyValuePair<K, V>(key, value);
		}

		public KeyValuePair<K, V> Unmarshall(JsonUnmarshallerContext context)
		{
			K key = keyUnmarshaller.Unmarshall(context);
			V value = valueUnmarshaller.Unmarshall(context);
			return new KeyValuePair<K, V>(key, value);
		}
	}
}
