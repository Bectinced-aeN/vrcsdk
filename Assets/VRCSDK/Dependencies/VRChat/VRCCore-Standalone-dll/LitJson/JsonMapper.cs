using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LitJson
{
	internal class JsonMapper
	{
		private static int max_nesting_depth;

		private static IFormatProvider datetime_format;

		private static IDictionary<Type, ExporterFunc> base_exporters_table;

		private static IDictionary<Type, ExporterFunc> custom_exporters_table;

		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;

		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

		private static IDictionary<Type, ArrayMetadata> array_metadata;

		private static readonly object array_metadata_lock;

		private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;

		private static readonly object conv_ops_lock;

		private static IDictionary<Type, ObjectMetadata> object_metadata;

		private static readonly object object_metadata_lock;

		private static IDictionary<Type, IList<PropertyMetadata>> type_properties;

		private static readonly object type_properties_lock;

		private static JsonWriter static_writer;

		private static readonly object static_writer_lock;

		static JsonMapper()
		{
			array_metadata_lock = new object();
			conv_ops_lock = new object();
			object_metadata_lock = new object();
			type_properties_lock = new object();
			static_writer_lock = new object();
			max_nesting_depth = 100;
			array_metadata = new Dictionary<Type, ArrayMetadata>();
			conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
			object_metadata = new Dictionary<Type, ObjectMetadata>();
			type_properties = new Dictionary<Type, IList<PropertyMetadata>>();
			static_writer = new JsonWriter();
			datetime_format = DateTimeFormatInfo.InvariantInfo;
			base_exporters_table = new Dictionary<Type, ExporterFunc>();
			custom_exporters_table = new Dictionary<Type, ExporterFunc>();
			base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			RegisterBaseExporters();
			RegisterBaseImporters();
		}

		private static bool HasInterface(Type type, string name)
		{
			return type.GetInterface(name, ignoreCase: true) != null;
		}

		public static PropertyInfo[] GetPublicInstanceProperties(Type type)
		{
			return type.GetProperties();
		}

		private static void AddArrayMetadata(Type type)
		{
			if (!array_metadata.ContainsKey(type))
			{
				ArrayMetadata value = default(ArrayMetadata);
				value.IsArray = type.IsArray;
				if (HasInterface(type, "System.Collections.IList"))
				{
					value.IsList = true;
				}
				PropertyInfo[] publicInstanceProperties = GetPublicInstanceProperties(type);
				foreach (PropertyInfo propertyInfo in publicInstanceProperties)
				{
					if (!(propertyInfo.Name != "Item"))
					{
						ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
						if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
						{
							value.ElementType = propertyInfo.PropertyType;
						}
					}
				}
				lock (array_metadata_lock)
				{
					try
					{
						array_metadata.Add(type, value);
					}
					catch (ArgumentException)
					{
						goto end_IL_00d5;
						IL_00db:
						end_IL_00d5:;
					}
				}
			}
		}

		private static void AddObjectMetadata(Type type)
		{
			if (!object_metadata.ContainsKey(type))
			{
				ObjectMetadata value = default(ObjectMetadata);
				if (HasInterface(type, "System.Collections.IDictionary"))
				{
					value.IsDictionary = true;
				}
				value.Properties = new Dictionary<string, PropertyMetadata>();
				PropertyInfo[] publicInstanceProperties = GetPublicInstanceProperties(type);
				foreach (PropertyInfo propertyInfo in publicInstanceProperties)
				{
					if (propertyInfo.Name == "Item")
					{
						ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
						if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(string))
						{
							value.ElementType = propertyInfo.PropertyType;
						}
					}
					else
					{
						PropertyMetadata value2 = default(PropertyMetadata);
						value2.Info = propertyInfo;
						value2.Type = propertyInfo.PropertyType;
						value.Properties.Add(propertyInfo.Name, value2);
					}
				}
				FieldInfo[] fields = type.GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					PropertyMetadata value3 = default(PropertyMetadata);
					value3.Info = fieldInfo;
					value3.IsField = true;
					value3.Type = fieldInfo.FieldType;
					value.Properties.Add(fieldInfo.Name, value3);
				}
				lock (object_metadata_lock)
				{
					try
					{
						object_metadata.Add(type, value);
					}
					catch (ArgumentException)
					{
						goto end_IL_0169;
						IL_016f:
						end_IL_0169:;
					}
				}
			}
		}

		private static void AddTypeProperties(Type type)
		{
			if (!type_properties.ContainsKey(type))
			{
				IList<PropertyMetadata> list = new List<PropertyMetadata>();
				PropertyInfo[] publicInstanceProperties = GetPublicInstanceProperties(type);
				foreach (PropertyInfo propertyInfo in publicInstanceProperties)
				{
					if (!(propertyInfo.Name == "Item"))
					{
						PropertyMetadata item = default(PropertyMetadata);
						item.Info = propertyInfo;
						item.IsField = false;
						list.Add(item);
					}
				}
				FieldInfo[] fields = type.GetFields();
				foreach (FieldInfo info in fields)
				{
					PropertyMetadata item2 = default(PropertyMetadata);
					item2.Info = info;
					item2.IsField = true;
					list.Add(item2);
				}
				lock (type_properties_lock)
				{
					try
					{
						type_properties.Add(type, list);
					}
					catch (ArgumentException)
					{
						goto end_IL_00db;
						IL_00e1:
						end_IL_00db:;
					}
				}
			}
		}

		private static MethodInfo GetConvOp(Type t1, Type t2)
		{
			lock (conv_ops_lock)
			{
				if (!conv_ops.ContainsKey(t1))
				{
					conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
				}
			}
			if (conv_ops[t1].ContainsKey(t2))
			{
				return conv_ops[t1][t2];
			}
			MethodInfo method = t1.GetMethod("op_Implicit", new Type[1]
			{
				t2
			});
			lock (conv_ops_lock)
			{
				try
				{
					conv_ops[t1].Add(t2, method);
					return method;
				}
				catch (ArgumentException)
				{
					return conv_ops[t1][t2];
					IL_00b1:
					return method;
				}
			}
		}

		private static object ReadValue(Type inst_type, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd)
			{
				return null;
			}
			if (reader.Token == JsonToken.Null)
			{
				if (!inst_type.IsClass)
				{
					throw new JsonException($"Can't assign null to an instance of type {inst_type}");
				}
				return null;
			}
			if (reader.Token == JsonToken.Double || reader.Token == JsonToken.Int || reader.Token == JsonToken.Long || reader.Token == JsonToken.String || reader.Token == JsonToken.Boolean)
			{
				Type type = reader.Value.GetType();
				if (inst_type.IsAssignableFrom(type))
				{
					return reader.Value;
				}
				if (custom_importers_table.ContainsKey(type) && custom_importers_table[type].ContainsKey(inst_type))
				{
					ImporterFunc importerFunc = custom_importers_table[type][inst_type];
					return importerFunc(reader.Value);
				}
				if (base_importers_table.ContainsKey(type) && base_importers_table[type].ContainsKey(inst_type))
				{
					ImporterFunc importerFunc2 = base_importers_table[type][inst_type];
					return importerFunc2(reader.Value);
				}
				if (inst_type.IsEnum)
				{
					return Enum.ToObject(inst_type, reader.Value);
				}
				MethodInfo convOp = GetConvOp(inst_type, type);
				if (convOp != null)
				{
					return convOp.Invoke(null, new object[1]
					{
						reader.Value
					});
				}
				throw new JsonException($"Can't assign value '{reader.Value}' (type {type}) to type {inst_type}");
			}
			object obj = null;
			if (reader.Token == JsonToken.ArrayStart)
			{
				if (inst_type.FullName == "System.Object")
				{
					inst_type = typeof(object[]);
				}
				AddArrayMetadata(inst_type);
				ArrayMetadata arrayMetadata = array_metadata[inst_type];
				if (!arrayMetadata.IsArray && !arrayMetadata.IsList)
				{
					throw new JsonException($"Type {inst_type} can't act as an array");
				}
				IList list;
				Type elementType;
				if (!arrayMetadata.IsArray)
				{
					list = (IList)Activator.CreateInstance(inst_type);
					elementType = arrayMetadata.ElementType;
				}
				else
				{
					list = new ArrayList();
					elementType = inst_type.GetElementType();
				}
				while (true)
				{
					object obj2 = ReadValue(elementType, reader);
					if (obj2 == null && reader.Token == JsonToken.ArrayEnd)
					{
						break;
					}
					list.Add(obj2);
				}
				if (arrayMetadata.IsArray)
				{
					int count = list.Count;
					obj = Array.CreateInstance(elementType, count);
					for (int i = 0; i < count; i++)
					{
						((Array)obj).SetValue(list[i], i);
					}
				}
				else
				{
					obj = list;
				}
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				if (inst_type == typeof(object))
				{
					inst_type = typeof(Dictionary<string, object>);
				}
				AddObjectMetadata(inst_type);
				ObjectMetadata objectMetadata = object_metadata[inst_type];
				obj = Activator.CreateInstance(inst_type);
				while (true)
				{
					reader.Read();
					if (reader.Token == JsonToken.ObjectEnd)
					{
						break;
					}
					string text = (string)reader.Value;
					if (objectMetadata.Properties.ContainsKey(text))
					{
						PropertyMetadata propertyMetadata = objectMetadata.Properties[text];
						if (propertyMetadata.IsField)
						{
							((FieldInfo)propertyMetadata.Info).SetValue(obj, ReadValue(propertyMetadata.Type, reader));
						}
						else
						{
							PropertyInfo propertyInfo = (PropertyInfo)propertyMetadata.Info;
							if (propertyInfo.CanWrite)
							{
								propertyInfo.SetValue(obj, ReadValue(propertyMetadata.Type, reader), null);
							}
							else
							{
								ReadValue(propertyMetadata.Type, reader);
							}
						}
					}
					else if (!objectMetadata.IsDictionary)
					{
						if (!reader.SkipNonMembers)
						{
							throw new JsonException($"The type {inst_type} doesn't have the property '{text}'");
						}
						ReadSkip(reader);
					}
					else
					{
						((IDictionary)obj).Add(text, ReadValue(objectMetadata.ElementType, reader));
					}
				}
			}
			return obj;
		}

		private static IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
			{
				return null;
			}
			IJsonWrapper jsonWrapper = factory();
			if (reader.Token == JsonToken.String)
			{
				jsonWrapper.SetString((string)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Double)
			{
				jsonWrapper.SetDouble((double)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Int)
			{
				jsonWrapper.SetInt((int)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Long)
			{
				jsonWrapper.SetLong((long)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Boolean)
			{
				jsonWrapper.SetBoolean((bool)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.ArrayStart)
			{
				jsonWrapper.SetJsonType(JsonType.Array);
				while (true)
				{
					IJsonWrapper jsonWrapper2 = ReadValue(factory, reader);
					if (jsonWrapper2 == null && reader.Token == JsonToken.ArrayEnd)
					{
						break;
					}
					jsonWrapper.Add(jsonWrapper2);
				}
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				jsonWrapper.SetJsonType(JsonType.Object);
				while (true)
				{
					reader.Read();
					if (reader.Token == JsonToken.ObjectEnd)
					{
						break;
					}
					string key = (string)reader.Value;
					jsonWrapper[key] = ReadValue(factory, reader);
				}
			}
			return jsonWrapper;
		}

		private static void ReadSkip(JsonReader reader)
		{
			ToWrapper(() => new JsonMockWrapper(), reader);
		}

		private static void RegisterBaseExporters()
		{
			base_exporters_table[typeof(byte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((byte)obj));
			};
			base_exporters_table[typeof(char)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((char)obj));
			};
			base_exporters_table[typeof(DateTime)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((DateTime)obj, datetime_format));
			};
			base_exporters_table[typeof(decimal)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((decimal)obj);
			};
			base_exporters_table[typeof(sbyte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((sbyte)obj));
			};
			base_exporters_table[typeof(short)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((short)obj));
			};
			base_exporters_table[typeof(ushort)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((ushort)obj));
			};
			base_exporters_table[typeof(uint)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToUInt64((uint)obj));
			};
			base_exporters_table[typeof(ulong)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((ulong)obj);
			};
		}

		private static void RegisterBaseImporters()
		{
			ImporterFunc importer = (object input) => Convert.ToByte((int)input);
			RegisterImporter(base_importers_table, typeof(int), typeof(byte), importer);
			importer = ((object input) => Convert.ToUInt64((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(ulong), importer);
			importer = ((object input) => Convert.ToSByte((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(sbyte), importer);
			importer = ((object input) => Convert.ToInt16((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(short), importer);
			importer = ((object input) => Convert.ToUInt16((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(ushort), importer);
			importer = ((object input) => Convert.ToUInt32((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(uint), importer);
			importer = ((object input) => Convert.ToSingle((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(float), importer);
			importer = ((object input) => Convert.ToDouble((int)input));
			RegisterImporter(base_importers_table, typeof(int), typeof(double), importer);
			importer = ((object input) => Convert.ToDecimal((double)input));
			RegisterImporter(base_importers_table, typeof(double), typeof(decimal), importer);
			importer = ((object input) => Convert.ToUInt32((long)input));
			RegisterImporter(base_importers_table, typeof(long), typeof(uint), importer);
			importer = ((object input) => Convert.ToChar((string)input));
			RegisterImporter(base_importers_table, typeof(string), typeof(char), importer);
			importer = ((object input) => Convert.ToDateTime((string)input, datetime_format));
			RegisterImporter(base_importers_table, typeof(string), typeof(DateTime), importer);
		}

		private static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
		{
			if (!table.ContainsKey(json_type))
			{
				table.Add(json_type, new Dictionary<Type, ImporterFunc>());
			}
			table[json_type][value_type] = importer;
		}

		private static void WriteValue(object obj, JsonWriter writer, bool writer_is_private, int depth)
		{
			if (depth > max_nesting_depth)
			{
				throw new JsonException($"Max allowed object depth reached while trying to export from type {obj.GetType()}");
			}
			if (obj == null)
			{
				writer.Write(null);
			}
			else if (obj is IJsonWrapper)
			{
				if (writer_is_private)
				{
					writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
				}
				else
				{
					((IJsonWrapper)obj).ToJson(writer);
				}
			}
			else if (obj is string)
			{
				writer.Write((string)obj);
			}
			else if (obj is double)
			{
				writer.Write((double)obj);
			}
			else if (obj is int)
			{
				writer.Write((int)obj);
			}
			else if (obj is bool)
			{
				writer.Write((bool)obj);
			}
			else if (obj is long)
			{
				writer.Write((long)obj);
			}
			else if (obj is Array)
			{
				writer.WriteArrayStart();
				foreach (object item in (Array)obj)
				{
					WriteValue(item, writer, writer_is_private, depth + 1);
				}
				writer.WriteArrayEnd();
			}
			else if (obj is IList)
			{
				writer.WriteArrayStart();
				foreach (object item2 in (IList)obj)
				{
					WriteValue(item2, writer, writer_is_private, depth + 1);
				}
				writer.WriteArrayEnd();
			}
			else if (obj is IDictionary)
			{
				writer.WriteObjectStart();
				IDictionaryEnumerator enumerator3 = ((IDictionary)obj).GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator3.Current;
						writer.WritePropertyName((string)dictionaryEntry.Key);
						WriteValue(dictionaryEntry.Value, writer, writer_is_private, depth + 1);
					}
				}
				finally
				{
					(enumerator3 as IDisposable)?.Dispose();
				}
				writer.WriteObjectEnd();
			}
			else
			{
				Type type = obj.GetType();
				if (custom_exporters_table.ContainsKey(type))
				{
					ExporterFunc exporterFunc = custom_exporters_table[type];
					exporterFunc(obj, writer);
				}
				else if (base_exporters_table.ContainsKey(type))
				{
					ExporterFunc exporterFunc2 = base_exporters_table[type];
					exporterFunc2(obj, writer);
				}
				else if (obj is Enum)
				{
					Type underlyingType = Enum.GetUnderlyingType(type);
					if (underlyingType == typeof(long) || underlyingType == typeof(uint) || underlyingType == typeof(ulong))
					{
						writer.Write((ulong)obj);
					}
					else
					{
						writer.Write((int)obj);
					}
				}
				else
				{
					AddTypeProperties(type);
					IList<PropertyMetadata> list = type_properties[type];
					writer.WriteObjectStart();
					foreach (PropertyMetadata item3 in list)
					{
						PropertyMetadata current3 = item3;
						if (current3.IsField)
						{
							writer.WritePropertyName(current3.Info.Name);
							WriteValue(((FieldInfo)current3.Info).GetValue(obj), writer, writer_is_private, depth + 1);
						}
						else
						{
							PropertyInfo propertyInfo = (PropertyInfo)current3.Info;
							if (propertyInfo.CanRead)
							{
								writer.WritePropertyName(current3.Info.Name);
								WriteValue(propertyInfo.GetValue(obj, null), writer, writer_is_private, depth + 1);
							}
						}
					}
					writer.WriteObjectEnd();
				}
			}
		}

		public static string ToJson(object obj)
		{
			lock (static_writer_lock)
			{
				static_writer.Reset();
				WriteValue(obj, static_writer, writer_is_private: true, 0);
				return static_writer.ToString();
				IL_0033:
				string result;
				return result;
			}
		}

		public static void ToJson(object obj, JsonWriter writer)
		{
			WriteValue(obj, writer, writer_is_private: false, 0);
		}

		public static JsonData ToObject(JsonReader reader)
		{
			return (JsonData)ToWrapper(() => new JsonData(), reader);
		}

		public static JsonData ToObject(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (JsonData)ToWrapper(() => new JsonData(), reader2);
		}

		public static JsonData ToObject(string json)
		{
			return (JsonData)ToWrapper(() => new JsonData(), json);
		}

		public static T ToObject<T>(JsonReader reader)
		{
			return (T)ReadValue(typeof(T), reader);
		}

		public static T ToObject<T>(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (T)ReadValue(typeof(T), reader2);
		}

		public static T ToObject<T>(string json)
		{
			JsonReader reader = new JsonReader(json);
			return (T)ReadValue(typeof(T), reader);
		}

		public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
		{
			return ReadValue(factory, reader);
		}

		public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
		{
			JsonReader reader = new JsonReader(json);
			return ReadValue(factory, reader);
		}

		public static void RegisterExporter<T>(ExporterFunc<T> exporter)
		{
			ExporterFunc value = delegate(object obj, JsonWriter writer)
			{
				exporter((T)obj, writer);
			};
			custom_exporters_table[typeof(T)] = value;
		}

		public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
		{
			ImporterFunc importer2 = (object input) => importer((TJson)input);
			RegisterImporter(custom_importers_table, typeof(TJson), typeof(TValue), importer2);
		}

		public static void UnregisterExporters()
		{
			custom_exporters_table.Clear();
		}

		public static void UnregisterImporters()
		{
			custom_importers_table.Clear();
		}
	}
}
