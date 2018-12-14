namespace VRC.Core
{
	public interface ApiCacheObject
	{
		bool ShouldCache();

		bool ShouldClearOnLevelLoad();

		float GetLifeSpan();

		ApiCacheObject Clone();
	}
}
