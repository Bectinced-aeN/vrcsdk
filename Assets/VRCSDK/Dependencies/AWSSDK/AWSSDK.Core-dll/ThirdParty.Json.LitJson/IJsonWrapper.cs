using System.Collections;
using System.Collections.Specialized;

namespace ThirdParty.Json.LitJson
{
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		bool IsArray
		{
			get;
		}

		bool IsBoolean
		{
			get;
		}

		bool IsDouble
		{
			get;
		}

		bool IsInt
		{
			get;
		}

		bool IsUInt
		{
			get;
		}

		bool IsLong
		{
			get;
		}

		bool IsULong
		{
			get;
		}

		bool IsObject
		{
			get;
		}

		bool IsString
		{
			get;
		}

		bool GetBoolean();

		double GetDouble();

		int GetInt();

		uint GetUInt();

		JsonType GetJsonType();

		long GetLong();

		ulong GetULong();

		string GetString();

		void SetBoolean(bool val);

		void SetDouble(double val);

		void SetInt(int val);

		void SetUInt(uint val);

		void SetJsonType(JsonType type);

		void SetLong(long val);

		void SetULong(ulong val);

		void SetString(string val);

		string ToJson();

		void ToJson(JsonWriter writer);
	}
}
