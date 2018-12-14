namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class ElasticBeanstalkActionIdentifiers
	{
		public static readonly ActionIdentifier AllElasticBeanstalkActions = new ActionIdentifier("elasticbeanstalk:*");

		public static readonly ActionIdentifier CheckDNSAvailability = new ActionIdentifier("elasticbeanstalk:CheckDNSAvailability");

		public static readonly ActionIdentifier CreateApplication = new ActionIdentifier("elasticbeanstalk:CreateApplication");

		public static readonly ActionIdentifier CreateApplicationVersion = new ActionIdentifier("elasticbeanstalk:CreateApplicationVersion");

		public static readonly ActionIdentifier CreateConfigurationTemplate = new ActionIdentifier("elasticbeanstalk:CreateConfigurationTemplate");

		public static readonly ActionIdentifier CreateEnvironment = new ActionIdentifier("elasticbeanstalk:CreateEnvironment");

		public static readonly ActionIdentifier CreateStorageLocation = new ActionIdentifier("elasticbeanstalk:CreateStorageLocation");

		public static readonly ActionIdentifier DeleteApplication = new ActionIdentifier("elasticbeanstalk:DeleteApplication");

		public static readonly ActionIdentifier DeleteApplicationVersion = new ActionIdentifier("elasticbeanstalk:DeleteApplicationVersion");

		public static readonly ActionIdentifier DeleteConfigurationTemplate = new ActionIdentifier("elasticbeanstalk:DeleteConfigurationTemplate");

		public static readonly ActionIdentifier DeleteEnvironmentConfiguration = new ActionIdentifier("elasticbeanstalk:DeleteEnvironmentConfiguration");

		public static readonly ActionIdentifier DescribeApplicationVersions = new ActionIdentifier("elasticbeanstalk:DescribeApplicationVersions");

		public static readonly ActionIdentifier DescribeApplications = new ActionIdentifier("elasticbeanstalk:DescribeApplications");

		public static readonly ActionIdentifier DescribeConfigurationOptions = new ActionIdentifier("elasticbeanstalk:DescribeConfigurationOptions");

		public static readonly ActionIdentifier DescribeConfigurationSettings = new ActionIdentifier("elasticbeanstalk:DescribeConfigurationSettings");

		public static readonly ActionIdentifier DescribeEnvironmentResources = new ActionIdentifier("elasticbeanstalk:DescribeEnvironmentResources");

		public static readonly ActionIdentifier DescribeEnvironments = new ActionIdentifier("elasticbeanstalk:DescribeEnvironments");

		public static readonly ActionIdentifier DescribeEvents = new ActionIdentifier("elasticbeanstalk:DescribeEvents");

		public static readonly ActionIdentifier ListAvailableSolutionStacks = new ActionIdentifier("elasticbeanstalk:ListAvailableSolutionStacks");

		public static readonly ActionIdentifier RebuildEnvironment = new ActionIdentifier("elasticbeanstalk:RebuildEnvironment");

		public static readonly ActionIdentifier RequestEnvironmentInfo = new ActionIdentifier("elasticbeanstalk:RequestEnvironmentInfo");

		public static readonly ActionIdentifier RestartAppServer = new ActionIdentifier("elasticbeanstalk:RestartAppServer");

		public static readonly ActionIdentifier RetrieveEnvironmentInfo = new ActionIdentifier("elasticbeanstalk:RetrieveEnvironmentInfo");

		public static readonly ActionIdentifier SwapEnvironmentCNAMEs = new ActionIdentifier("elasticbeanstalk:SwapEnvironmentCNAMEs");

		public static readonly ActionIdentifier TerminateEnvironment = new ActionIdentifier("elasticbeanstalk:TerminateEnvironment");

		public static readonly ActionIdentifier UpdateApplication = new ActionIdentifier("elasticbeanstalk:UpdateApplication");

		public static readonly ActionIdentifier UpdateApplicationVersion = new ActionIdentifier("elasticbeanstalk:UpdateApplicationVersion");

		public static readonly ActionIdentifier UpdateConfigurationTemplate = new ActionIdentifier("elasticbeanstalk:UpdateConfigurationTemplate");

		public static readonly ActionIdentifier UpdateEnvironment = new ActionIdentifier("elasticbeanstalk:UpdateEnvironment");

		public static readonly ActionIdentifier ValidateConfigurationSettings = new ActionIdentifier("elasticbeanstalk:ValidateConfigurationSettings");
	}
}
