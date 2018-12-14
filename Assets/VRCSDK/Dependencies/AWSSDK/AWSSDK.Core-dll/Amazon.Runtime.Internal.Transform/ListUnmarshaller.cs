using Amazon.Runtime.Internal.Util;
using System.Collections.Generic;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Transform
{
	public class ListUnmarshaller<I, IUnmarshaller> : IUnmarshaller<List<I>, XmlUnmarshallerContext>, IUnmarshaller<List<I>, JsonUnmarshallerContext> where IUnmarshaller : IUnmarshaller<I, XmlUnmarshallerContext>, IUnmarshaller<I, JsonUnmarshallerContext>
	{
		private IUnmarshaller iUnmarshaller;

		public ListUnmarshaller(IUnmarshaller iUnmarshaller)
		{
			this.iUnmarshaller = iUnmarshaller;
		}

		public List<I> Unmarshall(XmlUnmarshallerContext context)
		{
			int startingStackDepth = context.CurrentDepth + 1;
			List<I> list = new List<I>();
			while (context.Read())
			{
				if (context.IsStartElement && context.TestExpression("member", startingStackDepth))
				{
					I item = iUnmarshaller.Unmarshall(context);
					list.Add(item);
				}
			}
			return list;
		}

		public List<I> Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return new List<I>();
			}
			List<I> list = new AlwaysSendList<I>();
			while (!context.Peek(JsonToken.ArrayEnd))
			{
				list.Add(iUnmarshaller.Unmarshall(context));
			}
			context.Read();
			return list;
		}
	}
}
