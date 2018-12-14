using System.Collections.Generic;

namespace VRC.Core.BestHTTP.Extensions
{
	internal class KeyValuePairList
	{
		public List<KeyValuePair> Values
		{
			get;
			protected set;
		}

		public bool TryGet(string value, out KeyValuePair param)
		{
			param = null;
			for (int i = 0; i < Values.Count; i++)
			{
				if (string.CompareOrdinal(Values[i].Key, value) == 0)
				{
					param = Values[i];
					return true;
				}
			}
			return false;
		}

		public bool HasAny(string val1, string val2 = "")
		{
			for (int i = 0; i < Values.Count; i++)
			{
				if (string.CompareOrdinal(Values[i].Key, val1) == 0 || string.CompareOrdinal(Values[i].Key, val2) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
