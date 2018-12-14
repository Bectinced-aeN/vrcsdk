using System.Text;

namespace VRC.Core.BestHTTP.Forms
{
	internal class HTTPFieldData
	{
		public string Name
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string MimeType
		{
			get;
			set;
		}

		public Encoding Encoding
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public byte[] Binary
		{
			get;
			set;
		}

		public byte[] Payload
		{
			get
			{
				if (Binary != null)
				{
					return Binary;
				}
				if (Encoding == null)
				{
					Encoding = Encoding.UTF8;
				}
				return Binary = Encoding.GetBytes(Text);
			}
		}
	}
}
