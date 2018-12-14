namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class KinesisActionIdentifiers
	{
		public static readonly ActionIdentifier AllKinesisActions = new ActionIdentifier("kinesis:*");

		public static readonly ActionIdentifier CreateStream = new ActionIdentifier("kinesis:CreateStream");

		public static readonly ActionIdentifier DeleteStream = new ActionIdentifier("kinesis:DeleteStream");

		public static readonly ActionIdentifier DescribeStream = new ActionIdentifier("kinesis:DescribeStream");

		public static readonly ActionIdentifier ListStreams = new ActionIdentifier("kinesis:ListStreams");

		public static readonly ActionIdentifier PutRecord = new ActionIdentifier("kinesis:PutRecord");

		public static readonly ActionIdentifier GetShardIterator = new ActionIdentifier("kinesis:GetShardIterator");

		public static readonly ActionIdentifier GetRecords = new ActionIdentifier("kinesis:GetRecords");

		public static readonly ActionIdentifier MergeShards = new ActionIdentifier("kinesis:MergeShards");

		public static readonly ActionIdentifier SplitShard = new ActionIdentifier("kinesis:SplitShard");
	}
}
