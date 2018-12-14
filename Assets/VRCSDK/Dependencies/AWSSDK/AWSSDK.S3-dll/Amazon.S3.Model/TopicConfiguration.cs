using Amazon.Runtime;
using System;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class TopicConfiguration : NotificationConfiguration
	{
		public string Id
		{
			get;
			set;
		}

		[Obsolete("The Event property is now obsolete in favor the Events property.")]
		public string Event
		{
			get
			{
				if (!IsSetEvents())
				{
					return null;
				}
				return ConstantClass.op_Implicit(base.Events[0]);
			}
			set
			{
				if (base.Events == null)
				{
					base.Events = new List<EventType>();
				}
				if (base.Events.Count == 0)
				{
					base.Events.Add(value);
				}
				else
				{
					base.Events[0] = value;
				}
			}
		}

		public string Topic
		{
			get;
			set;
		}

		internal bool IsSetId()
		{
			return Id != null;
		}

		internal bool IsSetTopic()
		{
			return Topic != null;
		}
	}
}
