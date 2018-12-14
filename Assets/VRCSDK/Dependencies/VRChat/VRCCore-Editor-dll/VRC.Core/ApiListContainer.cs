using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiListContainer : ApiContainer
	{
		public List<object> ResponseList
		{
			get
			{
				if (!(base.Data is List<object>))
				{
					return new List<object>();
				}
				return base.Data as List<object>;
			}
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			string text = readTextData();
			base.Data = (Json.Decode(text) as List<object>);
			if (!(base.Data is List<object>))
			{
				base.Error = "Could not decode list from:\n" + text;
				return false;
			}
			return true;
		}
	}
}
