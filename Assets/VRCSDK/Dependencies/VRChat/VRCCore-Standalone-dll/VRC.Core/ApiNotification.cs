using System;
using System.Collections.Generic;
using System.Threading;

namespace VRC.Core
{
	public class ApiNotification : ApiModel
	{
		public enum NotificationType
		{
			All,
			Message,
			Friendrequest,
			Invite,
			Requestinvite,
			VoteToKick,
			Broadcast
		}

		[ApiField]
		public string senderUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string receiverUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string senderUsername
		{
			get;
			set;
		}

		[ApiField(Name = "type")]
		public NotificationType notificationType
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string message
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime created_at
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public Dictionary<string, object> details
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool seen
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string jobName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string jobColor
		{
			get;
			set;
		}

		public ApiNotification()
			: base(null)
		{
		}

		public override bool ShouldCache()
		{
			return false;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "type":
				data = notificationType.ToString().ToLower();
				return true;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "type":
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((data as string).ToLower());
				notificationType = (NotificationType)(int)Enum.Parse(typeof(NotificationType), value);
				return true;
			}
			default:
				return base.WriteField(fieldName, data);
			}
		}
	}
}
