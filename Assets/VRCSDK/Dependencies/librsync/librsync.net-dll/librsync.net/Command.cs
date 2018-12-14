namespace librsync.net
{
	public struct Command
	{
		public CommandKind Kind;

		public long Parameter1;

		public long Parameter2;

		public long Length
		{
			get
			{
				if (Kind == CommandKind.Literal)
				{
					return Parameter1;
				}
				if (Kind == CommandKind.Copy)
				{
					return Parameter2;
				}
				return 0L;
			}
		}
	}
}
