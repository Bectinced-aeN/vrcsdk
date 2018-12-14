namespace Amazon.Util.Internal
{
	public abstract class ConfigurationElement
	{
		public ElementInformation ElementInformation
		{
			get;
			set;
		}

		public ConfigurationElement()
		{
			ElementInformation = new ElementInformation(isPresent: true);
		}
	}
}
