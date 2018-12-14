using SQLite;

namespace AmplitudeSDKWrapper
{
	internal class Event
	{
		[PrimaryKey]
		[AutoIncrement]
		public int Id
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}
	}
}
