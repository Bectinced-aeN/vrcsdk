using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal.Transform
{
	public class JsonMarshallerContext : MarshallerContext
	{
		public JsonWriter Writer
		{
			get;
			private set;
		}

		public JsonMarshallerContext(IRequest request, JsonWriter writer)
			: base(request)
		{
			Writer = writer;
		}
	}
}
