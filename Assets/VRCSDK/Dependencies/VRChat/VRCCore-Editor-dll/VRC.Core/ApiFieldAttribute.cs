using System;

namespace VRC.Core
{
	public class ApiFieldAttribute : Attribute
	{
		public bool Required = true;

		public string Name;

		public bool IsAdminWritableOnly;

		public bool IsApiWritableOnly;
	}
}
