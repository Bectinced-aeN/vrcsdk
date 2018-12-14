using Amazon.Runtime.Internal.Util;
using System;
using System.IO;
using System.Text;
using ThirdParty.Ionic.Zlib;

namespace Amazon.Runtime.Internal.Transform
{
	public abstract class UnmarshallerContext : IDisposable
	{
		private bool disposed;

		protected bool MaintainResponseBody
		{
			get;
			set;
		}

		protected CrcCalculatorStream CrcStream
		{
			get;
			set;
		}

		protected int Crc32Result
		{
			get;
			set;
		}

		protected IWebResponseData WebResponseData
		{
			get;
			set;
		}

		protected CachingWrapperStream WrappingStream
		{
			get;
			set;
		}

		public string ResponseBody
		{
			get
			{
				if (MaintainResponseBody)
				{
					byte[] array = WrappingStream.AllReadBytes.ToArray();
					return Encoding.UTF8.GetString(array, 0, array.Length);
				}
				return string.Empty;
			}
		}

		public IWebResponseData ResponseData => WebResponseData;

		public abstract string CurrentPath
		{
			get;
		}

		public abstract int CurrentDepth
		{
			get;
		}

		public abstract bool IsStartElement
		{
			get;
		}

		public abstract bool IsEndElement
		{
			get;
		}

		public abstract bool IsStartOfDocument
		{
			get;
		}

		internal void ValidateCRC32IfAvailable()
		{
			if (CrcStream != null && CrcStream.Crc32 != Crc32Result)
			{
				throw new IOException("CRC value returned with response does not match the computed CRC value for the returned response body.");
			}
		}

		protected void SetupCRCStream(IWebResponseData responseData, Stream responseStream, long contentLength)
		{
			CrcStream = null;
			if (responseData != null && uint.TryParse(responseData.GetHeaderValue("x-amz-crc32"), out uint result))
			{
				Crc32Result = (int)result;
				CrcStream = new CrcCalculatorStream(responseStream, contentLength);
			}
		}

		public bool TestExpression(string expression)
		{
			return TestExpression(expression, CurrentPath);
		}

		public bool TestExpression(string expression, int startingStackDepth)
		{
			return TestExpression(expression, startingStackDepth, CurrentPath, CurrentDepth);
		}

		public bool ReadAtDepth(int targetDepth)
		{
			if (Read())
			{
				return CurrentDepth >= targetDepth;
			}
			return false;
		}

		private static bool TestExpression(string expression, string currentPath)
		{
			if (expression.Equals("."))
			{
				return true;
			}
			return currentPath.EndsWith(expression, StringComparison.OrdinalIgnoreCase);
		}

		private static bool TestExpression(string expression, int startingStackDepth, string currentPath, int currentDepth)
		{
			if (expression.Equals("."))
			{
				return true;
			}
			int num = -1;
			while ((num = expression.IndexOf("/", num + 1, StringComparison.Ordinal)) > -1)
			{
				if (expression[0] != '@')
				{
					startingStackDepth++;
				}
			}
			if (startingStackDepth == currentDepth)
			{
				return currentPath.EndsWith("/" + expression, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public abstract bool Read();

		public abstract string ReadText();

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (CrcStream != null)
					{
						CrcStream.Dispose();
						CrcStream = null;
					}
					if (WrappingStream != null)
					{
						WrappingStream.Dispose();
						WrappingStream = null;
					}
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
