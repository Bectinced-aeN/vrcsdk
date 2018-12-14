using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DESEncryption : IEncryption
{
	private const int Iterations = 1000;

	public string Encrypt(string plainText, string password)
	{
		if (plainText == null)
		{
			throw new ArgumentNullException("plainText");
		}
		if (string.IsNullOrEmpty(password))
		{
			throw new ArgumentNullException("password");
		}
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.GenerateIV();
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, dESCryptoServiceProvider.IV, 1000);
		byte[] bytes = rfc2898DeriveBytes.GetBytes(8);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, dESCryptoServiceProvider.IV), CryptoStreamMode.Write))
			{
				memoryStream.Write(dESCryptoServiceProvider.IV, 0, dESCryptoServiceProvider.IV.Length);
				byte[] bytes2 = Encoding.UTF8.GetBytes(plainText);
				cryptoStream.Write(bytes2, 0, bytes2.Length);
				cryptoStream.FlushFinalBlock();
				return Convert.ToBase64String(memoryStream.ToArray());
				IL_00b2:
				string result;
				return result;
			}
		}
	}

	public bool TryDecrypt(string cipherText, string password, out string plainText)
	{
		if (!string.IsNullOrEmpty(cipherText) && !string.IsNullOrEmpty(password))
		{
			try
			{
				byte[] buffer = Convert.FromBase64String(cipherText);
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
					byte[] array = new byte[8];
					memoryStream.Read(array, 0, array.Length);
					Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, array, 1000);
					byte[] bytes = rfc2898DeriveBytes.GetBytes(8);
					using (CryptoStream stream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(bytes, array), CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader(stream))
						{
							plainText = streamReader.ReadToEnd();
							return true;
							IL_008a:
							bool result;
							return result;
						}
					}
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				plainText = string.Empty;
				return false;
				IL_00e1:
				bool result;
				return result;
			}
		}
		plainText = string.Empty;
		return false;
	}
}
