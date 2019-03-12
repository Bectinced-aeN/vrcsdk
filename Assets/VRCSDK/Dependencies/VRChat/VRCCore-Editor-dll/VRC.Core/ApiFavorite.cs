using System.Collections.Generic;

namespace VRC.Core
{
	public class ApiFavorite : ApiModel
	{
		[ApiField]
		public new string id
		{
			get;
			set;
		}

		[ApiField]
		public string type
		{
			get;
			set;
		}

		[ApiField]
		public string favoriteId
		{
			get;
			set;
		}

		[ApiField]
		public string favoriteReleaseStatus
		{
			get;
			set;
		}

		[ApiField]
		public List<string> tags
		{
			get;
			set;
		}
	}
}
