using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class EventType : ConstantClass
	{
		public static readonly EventType ObjectCreatedAll = new EventType("s3:ObjectCreated:*");

		public static readonly EventType ObjectCreatedPut = new EventType("s3:ObjectCreated:Put");

		public static readonly EventType ObjectCreatedPost = new EventType("s3:ObjectCreated:Post");

		public static readonly EventType ObjectCreatedCopy = new EventType("s3:ObjectCreated:Copy");

		public static readonly EventType ObjectCreatedCompleteMultipartUpload = new EventType("s3:ObjectCreated:CompleteMultipartUpload");

		public static readonly EventType ObjectRemovedAll = new EventType("s3:ObjectRemoved:*");

		public static readonly EventType ObjectRemovedDelete = new EventType("s3:ObjectRemoved:Delete");

		public static readonly EventType ObjectRemovedDeleteMarkerCreated = new EventType("s3:ObjectRemoved:DeleteMarkerCreated");

		public static readonly EventType ReducedRedundancyLostObject = new EventType("s3:ReducedRedundancyLostObject");

		public EventType(string value)
			: this(value)
		{
		}

		public static EventType FindValue(string value)
		{
			return ConstantClass.FindValue<EventType>(value);
		}

		public static implicit operator EventType(string value)
		{
			return FindValue(value);
		}
	}
}
