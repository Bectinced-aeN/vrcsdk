namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class CloudWatchActionIdentifiers
	{
		public static readonly ActionIdentifier AllCloudWatchActions = new ActionIdentifier("cloudwatch:*");

		public static readonly ActionIdentifier DeleteAlarms = new ActionIdentifier("cloudwatch:DeleteAlarms");

		public static readonly ActionIdentifier DescribeAlarmHistory = new ActionIdentifier("cloudwatch:DescribeAlarmHistory");

		public static readonly ActionIdentifier DescribeAlarms = new ActionIdentifier("cloudwatch:DescribeAlarms");

		public static readonly ActionIdentifier DescribeAlarmsForMetric = new ActionIdentifier("cloudwatch:DescribeAlarmsForMetric");

		public static readonly ActionIdentifier DisableAlarmActions = new ActionIdentifier("cloudwatch:DisableAlarmActions");

		public static readonly ActionIdentifier EnableAlarmActions = new ActionIdentifier("cloudwatch:EnableAlarmActions");

		public static readonly ActionIdentifier GetMetricStatistics = new ActionIdentifier("cloudwatch:GetMetricStatistics");

		public static readonly ActionIdentifier ListMetrics = new ActionIdentifier("cloudwatch:ListMetrics");

		public static readonly ActionIdentifier PutMetricAlarm = new ActionIdentifier("cloudwatch:PutMetricAlarm");

		public static readonly ActionIdentifier PutMetricData = new ActionIdentifier("cloudwatch:PutMetricData");

		public static readonly ActionIdentifier SetAlarmState = new ActionIdentifier("cloudwatch:SetAlarmState");
	}
}
