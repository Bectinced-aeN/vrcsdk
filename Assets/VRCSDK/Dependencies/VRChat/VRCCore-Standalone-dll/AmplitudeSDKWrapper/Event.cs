using SQLite;

namespace AmplitudeSDKWrapper
{
	internal class Event
	{
		[AutoIncrement]
		[PrimaryKey]
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
