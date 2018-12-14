using System;
using System.Collections.Generic;
using System.Linq;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiDictContainer : ApiContainer
	{
		private string[] requiredFields
		{
			get;
			set;
		}

		public Dictionary<string, object> ResponseDictionary
		{
			get
			{
				if (!(base.Data is Dictionary<string, object>))
				{
					return new Dictionary<string, object>();
				}
				return base.Data as Dictionary<string, object>;
			}
		}

		public ApiDictContainer(Enum requiredFields)
		{
			this.requiredFields = Enum.GetNames(requiredFields.GetType());
		}

		public ApiDictContainer(params string[] requiredFields)
		{
			this.requiredFields = requiredFields;
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			base.Data = Json.Decode(readTextData());
			if (!(base.Data is Dictionary<string, object>))
			{
				base.Error = "Could not decode dictionary.";
				return false;
			}
			string[] array = (from f in requiredFields
			where !ResponseDictionary.ContainsKey(f)
			select f).ToArray();
			if (array.Length > 0)
			{
				base.Error = string.Format("Response was missing {0}.", string.Join(", ", array));
				return false;
			}
			return true;
		}
	}
}
