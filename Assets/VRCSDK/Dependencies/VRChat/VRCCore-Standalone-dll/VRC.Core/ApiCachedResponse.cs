namespace VRC.Core
{
	public class ApiCachedResponse
	{
		public string Data;

		public float Timestamp;

		public float Lifetime;

		public ApiCachedResponse(string data, float timestamp, float lifetime)
		{
			Data = data;
			Timestamp = timestamp;
			Lifetime = lifetime;
		}
	}
}
