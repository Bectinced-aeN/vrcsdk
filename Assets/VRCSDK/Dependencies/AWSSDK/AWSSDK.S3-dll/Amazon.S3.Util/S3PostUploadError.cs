using System.Xml;
using System.Xml.Serialization;

namespace Amazon.S3.Util
{
	[XmlRoot("Error")]
	public class S3PostUploadError
	{
		[XmlElement("Code")]
		public string ErrorCode
		{
			get;
			set;
		}

		[XmlElement("Message")]
		public string ErrorMessage
		{
			get;
			set;
		}

		[XmlElement("RequestId")]
		public string RequestId
		{
			get;
			set;
		}

		[XmlElement("HostId")]
		public string HostId
		{
			get;
			set;
		}

		[XmlAnyElement]
		public XmlElement[] elements
		{
			get;
			set;
		}
	}
}
