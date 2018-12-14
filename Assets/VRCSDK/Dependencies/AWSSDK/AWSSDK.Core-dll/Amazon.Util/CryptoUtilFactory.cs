using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ThirdParty.MD5;

namespace Amazon.Util
{
	public static class CryptoUtilFactory
	{
		private class CryptoUtil : ICryptoUtil
		{
			[ThreadStatic]
			private static HashAlgorithm _hashAlgorithm;

			private static HashAlgorithm SHA256HashAlgorithmInstance
			{
				get
				{
					if (_hashAlgorithm == null)
					{
						_hashAlgorithm = SHA256.Create();
					}
					return _hashAlgorithm;
				}
			}

			internal CryptoUtil()
			{
			}

			public string HMACSign(string data, string key, SigningAlgorithm algorithmName)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				return HMACSign(bytes, key, algorithmName);
			}

			private KeyedHashAlgorithm ThreadSafeCreateKeyedHashedAlgorithm(SigningAlgorithm algorithmName)
			{
				string text = algorithmName.ToString().ToUpper(CultureInfo.InvariantCulture);
				KeyedHashAlgorithm result = null;
				bool flag = true;
				lock (_keyedHashAlgorithmCreationLock)
				{
					flag = !_initializedAlgorithmNames.Contains(text);
					if (flag)
					{
						result = KeyedHashAlgorithm.Create(text);
						_initializedAlgorithmNames.Add(text);
					}
				}
				if (!flag)
				{
					result = KeyedHashAlgorithm.Create(text);
				}
				return result;
			}

			public string HMACSign(byte[] data, string key, SigningAlgorithm algorithmName)
			{
				if (string.IsNullOrEmpty(key))
				{
					throw new ArgumentNullException("key", "Please specify a Secret Signing Key.");
				}
				if (data == null || data.Length == 0)
				{
					throw new ArgumentNullException("data", "Please specify data to sign.");
				}
				KeyedHashAlgorithm keyedHashAlgorithm = ThreadSafeCreateKeyedHashedAlgorithm(algorithmName);
				if (keyedHashAlgorithm != null)
				{
					try
					{
						keyedHashAlgorithm.Key = Encoding.UTF8.GetBytes(key);
						return Convert.ToBase64String(keyedHashAlgorithm.ComputeHash(data));
					}
					finally
					{
						keyedHashAlgorithm.Clear();
					}
				}
				throw new InvalidOperationException("Please specify a KeyedHashAlgorithm to use.");
			}

			public byte[] ComputeSHA256Hash(byte[] data)
			{
				return SHA256HashAlgorithmInstance.ComputeHash(data);
			}

			public byte[] ComputeSHA256Hash(Stream steam)
			{
				return SHA256HashAlgorithmInstance.ComputeHash(steam);
			}

			public byte[] ComputeMD5Hash(byte[] data)
			{
				return new MD5Managed().ComputeHash(data);
			}

			public byte[] ComputeMD5Hash(Stream steam)
			{
				return new MD5Managed().ComputeHash(steam);
			}

			public byte[] HMACSignBinary(byte[] data, byte[] key, SigningAlgorithm algorithmName)
			{
				if (key == null || key.Length == 0)
				{
					throw new ArgumentNullException("key", "Please specify a Secret Signing Key.");
				}
				if (data == null || data.Length == 0)
				{
					throw new ArgumentNullException("data", "Please specify data to sign.");
				}
				KeyedHashAlgorithm keyedHashAlgorithm = ThreadSafeCreateKeyedHashedAlgorithm(algorithmName);
				if (keyedHashAlgorithm != null)
				{
					try
					{
						keyedHashAlgorithm.Key = key;
						return keyedHashAlgorithm.ComputeHash(data);
					}
					finally
					{
						keyedHashAlgorithm.Clear();
					}
				}
				throw new InvalidOperationException("Please specify a KeyedHashAlgorithm to use.");
			}
		}

		private static CryptoUtil util = new CryptoUtil();

		private static HashSet<string> _initializedAlgorithmNames = new HashSet<string>();

		private static object _keyedHashAlgorithmCreationLock = new object();

		public static ICryptoUtil CryptoInstance => util;
	}
}
