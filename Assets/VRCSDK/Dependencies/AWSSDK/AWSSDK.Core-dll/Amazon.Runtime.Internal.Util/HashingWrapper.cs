using System;
using System.IO;
using System.Security.Cryptography;
using ThirdParty.MD5;

namespace Amazon.Runtime.Internal.Util
{
	public class HashingWrapper : IHashingWrapper, IDisposable
	{
		private static string MD5ManagedName = typeof(MD5Managed).FullName;

		private HashAlgorithm _algorithm;

		public HashingWrapper(string algorithmName)
		{
			if (string.IsNullOrEmpty(algorithmName))
			{
				throw new ArgumentNullException("algorithmName");
			}
			Init(algorithmName);
		}

		public void AppendBlock(byte[] buffer)
		{
			AppendBlock(buffer, 0, buffer.Length);
		}

		public byte[] AppendLastBlock(byte[] buffer)
		{
			return AppendLastBlock(buffer, 0, buffer.Length);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Init(string algorithmName)
		{
			if (string.Equals(MD5ManagedName, algorithmName, StringComparison.Ordinal))
			{
				_algorithm = new MD5Managed();
				return;
			}
			throw new ArgumentOutOfRangeException(algorithmName, "Unsupported hashing algorithm");
		}

		public void Clear()
		{
			_algorithm.Initialize();
		}

		public byte[] ComputeHash(byte[] buffer)
		{
			return _algorithm.ComputeHash(buffer);
		}

		public byte[] ComputeHash(Stream stream)
		{
			return _algorithm.ComputeHash(stream);
		}

		public void AppendBlock(byte[] buffer, int offset, int count)
		{
			_algorithm.TransformBlock(buffer, offset, count, null, 0);
		}

		public byte[] AppendLastBlock(byte[] buffer, int offset, int count)
		{
			_algorithm.TransformFinalBlock(buffer, offset, count);
			return _algorithm.Hash;
		}

		protected virtual void Dispose(bool disposing)
		{
			IDisposable algorithm = _algorithm;
			if (disposing && algorithm != null)
			{
				algorithm.Dispose();
				_algorithm = null;
			}
		}
	}
}
