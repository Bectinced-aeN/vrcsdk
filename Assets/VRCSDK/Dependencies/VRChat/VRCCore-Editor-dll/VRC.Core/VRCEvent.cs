using System.Collections.Generic;

namespace VRC.Core
{
	public class VRCEvent : ApiModel
	{
		[ApiField]
		public string name
		{
			get;
			private set;
		}

		[ApiField]
		public string description
		{
			get;
			private set;
		}

		[ApiField]
		public string imageUrl
		{
			get;
			private set;
		}

		[ApiField]
		public string authorName
		{
			get;
			private set;
		}

		[ApiField]
		public string authorId
		{
			get;
			private set;
		}

		[ApiField]
		public string startTime
		{
			get;
			private set;
		}

		[ApiField]
		public string venueId
		{
			get;
			private set;
		}

		public VRCEvent()
			: base(null)
		{
		}

		public static List<VRCEvent> MakeEvents(Dictionary<string, object> jsonObjects)
		{
			List<VRCEvent> list = new List<VRCEvent>();
			if (jsonObjects == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, object> jsonObject in jsonObjects)
			{
				VRCEvent vRCEvent = API.CreateFromJson<VRCEvent>(jsonObject.Value as Dictionary<string, object>);
				if (vRCEvent != null)
				{
					list.Add(vRCEvent);
				}
			}
			return list;
		}

		public override string ToString()
		{
			return $"[id: {base.id}; name: {name}; imageUrl: {imageUrl}; authorName: {authorName}; authorId: {authorId}; startTime: {startTime}; description: {description};]";
		}
	}
}
