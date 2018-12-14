namespace LitJson
{
	internal delegate object ImporterFunc(object input);
	internal delegate TValue ImporterFunc<TJson, TValue>(TJson input);
}
