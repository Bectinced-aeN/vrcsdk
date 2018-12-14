using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal sealed class SessionParameters
	{
		internal sealed class Builder
		{
			private int mCipherSuite = -1;

			private short mCompressionAlgorithm = -1;

			private byte[] mMasterSecret;

			private Certificate mPeerCertificate;

			private byte[] mPskIdentity;

			private byte[] mSrpIdentity;

			private byte[] mEncodedServerExtensions;

			public SessionParameters Build()
			{
				Validate(mCipherSuite >= 0, "cipherSuite");
				Validate(mCompressionAlgorithm >= 0, "compressionAlgorithm");
				Validate(mMasterSecret != null, "masterSecret");
				return new SessionParameters(mCipherSuite, (byte)mCompressionAlgorithm, mMasterSecret, mPeerCertificate, mPskIdentity, mSrpIdentity, mEncodedServerExtensions);
			}

			public Builder SetCipherSuite(int cipherSuite)
			{
				mCipherSuite = cipherSuite;
				return this;
			}

			public Builder SetCompressionAlgorithm(byte compressionAlgorithm)
			{
				mCompressionAlgorithm = compressionAlgorithm;
				return this;
			}

			public Builder SetMasterSecret(byte[] masterSecret)
			{
				mMasterSecret = masterSecret;
				return this;
			}

			public Builder SetPeerCertificate(Certificate peerCertificate)
			{
				mPeerCertificate = peerCertificate;
				return this;
			}

			public Builder SetPskIdentity(byte[] pskIdentity)
			{
				mPskIdentity = pskIdentity;
				return this;
			}

			public Builder SetSrpIdentity(byte[] srpIdentity)
			{
				mSrpIdentity = srpIdentity;
				return this;
			}

			public Builder SetServerExtensions(IDictionary serverExtensions)
			{
				if (serverExtensions == null)
				{
					mEncodedServerExtensions = null;
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					TlsProtocol.WriteExtensions(memoryStream, serverExtensions);
					mEncodedServerExtensions = memoryStream.ToArray();
				}
				return this;
			}

			private void Validate(bool condition, string parameter)
			{
				if (!condition)
				{
					throw new InvalidOperationException("Required session parameter '" + parameter + "' not configured");
				}
			}
		}

		private int mCipherSuite;

		private byte mCompressionAlgorithm;

		private byte[] mMasterSecret;

		private Certificate mPeerCertificate;

		private byte[] mPskIdentity;

		private byte[] mSrpIdentity;

		private byte[] mEncodedServerExtensions;

		public int CipherSuite => mCipherSuite;

		public byte CompressionAlgorithm => mCompressionAlgorithm;

		public byte[] MasterSecret => mMasterSecret;

		public Certificate PeerCertificate => mPeerCertificate;

		public byte[] PskIdentity => mPskIdentity;

		public byte[] SrpIdentity => mSrpIdentity;

		private SessionParameters(int cipherSuite, byte compressionAlgorithm, byte[] masterSecret, Certificate peerCertificate, byte[] pskIdentity, byte[] srpIdentity, byte[] encodedServerExtensions)
		{
			mCipherSuite = cipherSuite;
			mCompressionAlgorithm = compressionAlgorithm;
			mMasterSecret = Arrays.Clone(masterSecret);
			mPeerCertificate = peerCertificate;
			mPskIdentity = Arrays.Clone(pskIdentity);
			mSrpIdentity = Arrays.Clone(srpIdentity);
			mEncodedServerExtensions = encodedServerExtensions;
		}

		public void Clear()
		{
			if (mMasterSecret != null)
			{
				Arrays.Fill(mMasterSecret, 0);
			}
		}

		public SessionParameters Copy()
		{
			return new SessionParameters(mCipherSuite, mCompressionAlgorithm, mMasterSecret, mPeerCertificate, mPskIdentity, mSrpIdentity, mEncodedServerExtensions);
		}

		public IDictionary ReadServerExtensions()
		{
			if (mEncodedServerExtensions == null)
			{
				return null;
			}
			MemoryStream input = new MemoryStream(mEncodedServerExtensions, writable: false);
			return TlsProtocol.ReadExtensions(input);
		}
	}
}
