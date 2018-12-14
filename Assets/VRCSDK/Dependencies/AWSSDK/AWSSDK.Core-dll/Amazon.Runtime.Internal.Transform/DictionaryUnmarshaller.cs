using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Transform
{
	public class DictionaryUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller> : IUnmarshaller<Dictionary<TKey, TValue>, XmlUnmarshallerContext>, IUnmarshaller<Dictionary<TKey, TValue>, JsonUnmarshallerContext> where TKeyUnmarshaller : IUnmarshaller<TKey, XmlUnmarshallerContext>, IUnmarshaller<TKey, JsonUnmarshallerContext> where TValueUnmarshaller : IUnmarshaller<TValue, XmlUnmarshallerContext>, IUnmarshaller<TValue, JsonUnmarshallerContext>
	{
		private KeyValueUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller> KVUnmarshaller;

		public DictionaryUnmarshaller(TKeyUnmarshaller kUnmarshaller, TValueUnmarshaller vUnmarshaller)
		{
			KVUnmarshaller = new KeyValueUnmarshaller<TKey, TValue, TKeyUnmarshaller, TValueUnmarshaller>(kUnmarshaller, vUnmarshaller);
		}

		Dictionary<TKey, TValue> IUnmarshaller<Dictionary<TKey, TValue>, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
		{
			throw new NotImplementedException();
		}

		public Dictionary<TKey, TValue> Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			if (context.CurrentTokenType == JsonToken.Null)
			{
				return new Dictionary<TKey, TValue>();
			}
			Dictionary<TKey, TValue> dictionary = new AlwaysSendDictionary<TKey, TValue>();
			while (!context.Peek(JsonToken.ObjectEnd))
			{
				KeyValuePair<TKey, TValue> keyValuePair = KVUnmarshaller.Unmarshall(context);
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			context.Read();
			return dictionary;
		}
	}
}
