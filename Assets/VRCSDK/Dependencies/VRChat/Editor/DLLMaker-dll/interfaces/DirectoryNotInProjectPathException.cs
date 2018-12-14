using System;

namespace interfaces
{
	public class DirectoryNotInProjectPathException : Exception
	{
		public DirectoryNotInProjectPathException()
		{
		}

		public DirectoryNotInProjectPathException(string description)
			: base(description)
		{
		}
	}
}
