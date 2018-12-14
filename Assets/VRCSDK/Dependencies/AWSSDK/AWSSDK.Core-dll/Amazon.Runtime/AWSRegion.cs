namespace Amazon.Runtime
{
	public abstract class AWSRegion
	{
		public RegionEndpoint Region
		{
			get;
			protected set;
		}

		protected void SetRegionFromName(string regionSystemName)
		{
			Region = RegionEndpoint.GetBySystemName(regionSystemName);
		}
	}
}
