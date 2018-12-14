using System.Xml;

namespace Amazon.Runtime.Internal.Transform
{
	public class XmlMarshallerContext : MarshallerContext
	{
		public XmlWriter Writer
		{
			get;
			private set;
		}

		public XmlMarshallerContext(IRequest request, XmlWriter writer)
			: base(request)
		{
			Writer = writer;
		}
	}
}
