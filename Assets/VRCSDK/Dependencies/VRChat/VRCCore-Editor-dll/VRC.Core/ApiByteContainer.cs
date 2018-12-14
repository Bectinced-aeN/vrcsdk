using System;

namespace VRC.Core
{
	public class ApiByteContainer : ApiContainer
	{
		public byte[] ResponseBytes
		{
			get
			{
				if (!(base.Data is byte[]))
				{
					return new byte[0];
				}
				return base.Data as byte[];
			}
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			base.Data = readData();
			if (!(base.Data is byte[]))
			{
				base.Error = "Could not decode byte array.";
				return false;
			}
			return true;
		}
	}
}
