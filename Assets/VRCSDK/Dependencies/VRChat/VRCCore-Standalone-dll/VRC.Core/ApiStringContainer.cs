using System;

namespace VRC.Core
{
	public class ApiStringContainer : ApiContainer
	{
		public string ResponseString
		{
			get
			{
				if (!(base.Data is string))
				{
					return null;
				}
				return base.Data as string;
			}
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			base.Data = readTextData();
			if (!(base.Data is string))
			{
				base.Error = "Could not decode string.";
				return false;
			}
			return true;
		}
	}
}
