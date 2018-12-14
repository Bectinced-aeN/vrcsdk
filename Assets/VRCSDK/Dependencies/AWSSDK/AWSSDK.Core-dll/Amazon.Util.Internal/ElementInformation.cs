namespace Amazon.Util.Internal
{
	public class ElementInformation
	{
		public bool IsPresent
		{
			get;
			private set;
		}

		public ElementInformation(bool isPresent)
		{
			IsPresent = isPresent;
		}
	}
}
