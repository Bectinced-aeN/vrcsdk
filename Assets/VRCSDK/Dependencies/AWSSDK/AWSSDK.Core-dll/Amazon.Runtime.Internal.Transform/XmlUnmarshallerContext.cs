using Amazon.Runtime.Internal.Util;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Amazon.Runtime.Internal.Transform
{
	public class XmlUnmarshallerContext : UnmarshallerContext
	{
		private static readonly XmlReaderSettings READER_SETTINGS = new XmlReaderSettings
		{
			ProhibitDtd = false,
			IgnoreWhitespace = true
		};

		private static HashSet<XmlNodeType> nodesToSkip = new HashSet<XmlNodeType>
		{
			XmlNodeType.None,
			XmlNodeType.XmlDeclaration,
			XmlNodeType.Comment,
			XmlNodeType.DocumentType
		};

		private StreamReader streamReader;

		private XmlReader _xmlReader;

		private Stack<string> stack = new Stack<string>();

		private string stackString = "";

		private Dictionary<string, string> attributeValues;

		private List<string> attributeNames;

		private IEnumerator<string> attributeEnumerator;

		private XmlNodeType nodeType;

		private string nodeContent = string.Empty;

		private bool disposed;

		public Stream Stream => streamReader.BaseStream;

		private XmlReader XmlReader
		{
			get
			{
				if (_xmlReader == null)
				{
					_xmlReader = XmlReader.Create(streamReader, READER_SETTINGS);
				}
				return _xmlReader;
			}
		}

		public override string CurrentPath => stackString;

		public override int CurrentDepth => stack.Count;

		public override bool IsStartElement => nodeType == XmlNodeType.Element;

		public override bool IsEndElement => nodeType == XmlNodeType.EndElement;

		public override bool IsStartOfDocument => XmlReader.ReadState == ReadState.Initial;

		public bool IsAttribute => nodeType == XmlNodeType.Attribute;

		public XmlUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData responseData)
		{
			if (maintainResponseBody)
			{
				base.WrappingStream = new CachingWrapperStream(responseStream, AWSConfigs.LoggingConfig.LogResponsesSizeLimit);
				responseStream = base.WrappingStream;
			}
			streamReader = new StreamReader(responseStream);
			base.WebResponseData = responseData;
			base.MaintainResponseBody = maintainResponseBody;
		}

		public override bool Read()
		{
			if (attributeEnumerator != null && attributeEnumerator.MoveNext())
			{
				nodeType = XmlNodeType.Attribute;
				stackString = string.Format(CultureInfo.InvariantCulture, "{0}/@{1}", StackToPath(stack), attributeEnumerator.Current);
			}
			else
			{
				if (nodesToSkip.Contains(XmlReader.NodeType))
				{
					XmlReader.Read();
				}
				while (XmlReader.IsEmptyElement)
				{
					XmlReader.Read();
				}
				switch (XmlReader.NodeType)
				{
				case XmlNodeType.EndElement:
					nodeType = XmlNodeType.EndElement;
					stack.Pop();
					stackString = StackToPath(stack);
					XmlReader.Read();
					break;
				case XmlNodeType.Element:
					nodeType = XmlNodeType.Element;
					stack.Push(XmlReader.LocalName);
					stackString = StackToPath(stack);
					ReadElement();
					break;
				}
			}
			if (XmlReader.ReadState != ReadState.EndOfFile && XmlReader.ReadState != ReadState.Error)
			{
				return XmlReader.ReadState != ReadState.Closed;
			}
			return false;
		}

		public override string ReadText()
		{
			if (nodeType == XmlNodeType.Attribute)
			{
				return attributeValues[attributeEnumerator.Current];
			}
			return nodeContent;
		}

		private static string StackToPath(Stack<string> stack)
		{
			string text = null;
			string[] array = stack.ToArray();
			foreach (string text2 in array)
			{
				text = ((text == null) ? text2 : string.Format(CultureInfo.InvariantCulture, "{0}/{1}", text2, text));
			}
			return "/" + text;
		}

		private void ReadElement()
		{
			if (XmlReader.HasAttributes)
			{
				attributeValues = new Dictionary<string, string>();
				attributeNames = new List<string>();
				while (XmlReader.MoveToNextAttribute())
				{
					attributeValues.Add(XmlReader.LocalName, XmlReader.Value);
					attributeNames.Add(XmlReader.LocalName);
				}
				attributeEnumerator = attributeNames.GetEnumerator();
			}
			XmlReader.MoveToElement();
			XmlReader.Read();
			if (XmlReader.NodeType == XmlNodeType.Text)
			{
				nodeContent = XmlReader.ReadContentAsString();
			}
			else
			{
				nodeContent = string.Empty;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (streamReader != null)
					{
						streamReader.Dispose();
						streamReader = null;
					}
					if (_xmlReader != null)
					{
						_xmlReader.Close();
						_xmlReader = null;
					}
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
	}
}
