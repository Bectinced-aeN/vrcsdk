using System.Collections.Generic;

namespace VRC.Core
{
	public class VRCEvent : ApiModel
	{
		private string mName;

		private string mDescription;

		private string mImageUrl;

		private string mAuthorName;

		private string mAuthorId;

		private string mStartTime;

		private string mVenueId;

		public string name => mName;

		public string description => mDescription;

		public string imageUrl => mImageUrl;

		public string authorName => mAuthorName;

		public string authorId => mAuthorId;

		public string startTime => mStartTime;

		public string venueId => mVenueId;

		public VRCEvent(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			mName = (jsonObject["name"] as string);
			mDescription = (jsonObject["description"] as string);
			mImageUrl = (jsonObject["imageUrl"] as string);
			mAuthorName = (jsonObject["authorName"] as string);
			mAuthorId = (jsonObject["authorId"] as string);
			mStartTime = (jsonObject["startTime"] as string);
		}

		public static List<VRCEvent> VRCEvents(Dictionary<string, object> jsonObjects)
		{
			List<VRCEvent> list = new List<VRCEvent>();
			if (jsonObjects != null)
			{
				foreach (KeyValuePair<string, object> jsonObject in jsonObjects)
				{
					VRCEvent item = new VRCEvent(jsonObject.Value as Dictionary<string, object>);
					list.Add(item);
				}
				return list;
			}
			return list;
		}

		public override string ToString()
		{
			return $"[id: {base.id}; name: {name}; imageUrl: {imageUrl}; authorName: {authorName}; authorId: {authorId}; startTime: {startTime}; description: {description};]";
		}
	}
}
