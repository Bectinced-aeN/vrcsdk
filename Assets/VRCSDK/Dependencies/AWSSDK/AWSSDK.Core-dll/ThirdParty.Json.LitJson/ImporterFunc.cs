namespace ThirdParty.Json.LitJson
{
	internal delegate object ImporterFunc(object input);
	public delegate TValue ImporterFunc<TJson, TValue>(TJson input);
}
