namespace LitJson
{
	internal delegate void ExporterFunc(object obj, JsonWriter writer);
	internal delegate void ExporterFunc<T>(T obj, JsonWriter writer);
}
