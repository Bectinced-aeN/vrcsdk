using System;
using System.Text;
using UnityEngine;

namespace VRC.Core.BestHTTP.Forms
{
	internal sealed class HTTPUrlEncodedForm : HTTPFormBase
	{
		private byte[] CachedData;

		public override void PrepareRequest(HTTPRequest request)
		{
			request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
		}

		public override byte[] GetData()
		{
			if (CachedData != null && !base.IsChanged)
			{
				return CachedData;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < base.Fields.Count; i++)
			{
				HTTPFieldData hTTPFieldData = base.Fields[i];
				if (i > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.Append(EscapeDataStringHelper(hTTPFieldData.Name));
				stringBuilder.Append("=");
				try
				{
					if (!string.IsNullOrEmpty(hTTPFieldData.Text) || hTTPFieldData.Binary == null)
					{
						stringBuilder.Append(EscapeDataStringHelper(hTTPFieldData.Text));
					}
					else
					{
						stringBuilder.Append(EscapeDataStringHelper(Encoding.UTF8.GetString(hTTPFieldData.Binary, 0, hTTPFieldData.Binary.Length)));
					}
				}
				catch (FormatException ex)
				{
					Debug.LogErrorFormat("Could not escape field {0} using data {1}", new object[2]
					{
						hTTPFieldData.Name,
						hTTPFieldData.Text
					});
					Debug.LogException((Exception)ex);
				}
			}
			base.IsChanged = false;
			return CachedData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}

		private string EscapeDataStringHelper(string text)
		{
			int num = 32000;
			if (text.Length <= num)
			{
				return Uri.EscapeDataString(text);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i += num)
			{
				stringBuilder.Append(Uri.EscapeDataString(text.Substring(i, Math.Min(num, text.Length - i))));
			}
			return stringBuilder.ToString();
		}
	}
}
