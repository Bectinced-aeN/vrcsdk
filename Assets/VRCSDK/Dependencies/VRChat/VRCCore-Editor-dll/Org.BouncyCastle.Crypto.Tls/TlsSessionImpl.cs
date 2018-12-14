using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsSessionImpl : TlsSession
	{
		internal readonly byte[] mSessionID;

		internal SessionParameters mSessionParameters;

		public virtual byte[] SessionID
		{
			get
			{
				lock (this)
				{
					return mSessionID;
					IL_0014:
					byte[] result;
					return result;
				}
			}
		}

		public virtual bool IsResumable
		{
			get
			{
				lock (this)
				{
					return mSessionParameters != null;
					IL_001a:
					bool result;
					return result;
				}
			}
		}

		internal TlsSessionImpl(byte[] sessionID, SessionParameters sessionParameters)
		{
			if (sessionID == null)
			{
				throw new ArgumentNullException("sessionID");
			}
			if (sessionID.Length < 1 || sessionID.Length > 32)
			{
				throw new ArgumentException("must have length between 1 and 32 bytes, inclusive", "sessionID");
			}
			mSessionID = Arrays.Clone(sessionID);
			mSessionParameters = sessionParameters;
		}

		public virtual SessionParameters ExportSessionParameters()
		{
			lock (this)
			{
				return (mSessionParameters != null) ? mSessionParameters.Copy() : null;
				IL_002a:
				SessionParameters result;
				return result;
			}
		}

		public virtual void Invalidate()
		{
			lock (this)
			{
				if (mSessionParameters != null)
				{
					mSessionParameters.Clear();
					mSessionParameters = null;
				}
			}
		}
	}
}
