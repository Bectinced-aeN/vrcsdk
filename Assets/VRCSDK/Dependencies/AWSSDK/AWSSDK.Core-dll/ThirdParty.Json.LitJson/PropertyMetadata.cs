using System;
using System.Reflection;

namespace ThirdParty.Json.LitJson
{
	internal struct PropertyMetadata
	{
		public MemberInfo Info;

		public bool IsField;

		public Type Type;
	}
}
