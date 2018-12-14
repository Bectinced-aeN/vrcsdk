using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VRCSDK2
{
	public static class VRC_Serialization
	{
		private class NetworkSurrogateSelector : ISurrogateSelector
		{
			private ISurrogateSelector _next;

			public void ChainSelector(ISurrogateSelector selector)
			{
				if (selector == this)
				{
					throw new ArgumentException("Cannot allow Cyclical Surrogates");
				}
				if (_next == null)
				{
					_next = selector;
				}
				else
				{
					_next.ChainSelector(selector);
				}
			}

			public ISurrogateSelector GetNextSelector()
			{
				return _next;
			}

			public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
			{
				selector = this;
				if (type == typeof(Vector2))
				{
					return new Vector2Surrogate();
				}
				if (type == typeof(Vector3))
				{
					return new Vector3Surrogate();
				}
				if (type == typeof(Vector4))
				{
					return new Vector4Surrogate();
				}
				if (type == typeof(Quaternion))
				{
					return new QuaternionSurrogate();
				}
				if (type == typeof(Color))
				{
					return new ColorSurrogate();
				}
				if (type == typeof(Color32))
				{
					return new Color32Surrogate();
				}
				if (type == typeof(GameObject))
				{
					return new GameObjectSurrogate();
				}
				if (type == typeof(Transform))
				{
					return new TransformSurrogate();
				}
				if (type.IsSubclassOf(typeof(Object)))
				{
					return new ObjectSurrogate();
				}
				selector = null;
				if (_next != null)
				{
					return _next.GetSurrogate(type, context, out selector);
				}
				return null;
			}
		}

		private class Vector2Surrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Vector2 val = (Vector2)obj;
				info.AddValue("x", val.x);
				info.AddValue("y", val.y);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				Vector2 val = (Vector2)obj;
				val.x = info.GetSingle("x");
				val.y = info.GetSingle("y");
				return val;
			}
		}

		private class Vector3Surrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val = (Vector3)obj;
				info.AddValue("x", val.x);
				info.AddValue("y", val.y);
				info.AddValue("z", val.z);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val = (Vector3)obj;
				val.x = info.GetSingle("x");
				val.y = info.GetSingle("y");
				val.z = info.GetSingle("z");
				return val;
			}
		}

		private class Vector4Surrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Vector4 val = (Vector4)obj;
				info.AddValue("x", val.x);
				info.AddValue("y", val.y);
				info.AddValue("z", val.z);
				info.AddValue("w", val.w);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				Vector4 val = (Vector4)obj;
				val.x = info.GetSingle("x");
				val.y = info.GetSingle("y");
				val.z = info.GetSingle("z");
				val.w = info.GetSingle("w");
				return val;
			}
		}

		private class QuaternionSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Quaternion val = (Quaternion)obj;
				info.AddValue("x", val.x);
				info.AddValue("y", val.y);
				info.AddValue("z", val.z);
				info.AddValue("w", val.w);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				Quaternion val = (Quaternion)obj;
				val.x = info.GetSingle("x");
				val.y = info.GetSingle("y");
				val.z = info.GetSingle("z");
				val.w = info.GetSingle("w");
				return val;
			}
		}

		private class TransformSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				Transform val = obj;
				if (val.get_gameObject() == null)
				{
					throw new ArgumentException("Transform must have an associated gameObject.");
				}
				string gameObjectPath = GetGameObjectPath(val.get_gameObject());
				info.AddValue("path", gameObjectPath);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				string @string = info.GetString("path");
				GameObject val = FindGameObject(@string);
				if (val == null)
				{
					throw new KeyNotFoundException("Could not locate Transform at path " + @string);
				}
				return val.get_transform();
			}
		}

		private class ColorSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Color val = (Color)obj;
				info.AddValue("a", val.a);
				info.AddValue("r", val.r);
				info.AddValue("g", val.g);
				info.AddValue("b", val.b);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				Color val = (Color)obj;
				val.a = info.GetSingle("a");
				val.r = info.GetSingle("r");
				val.g = info.GetSingle("g");
				val.b = info.GetSingle("b");
				return val;
			}
		}

		private class Color32Surrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Color32 val = (Color32)obj;
				info.AddValue("a", val.a);
				info.AddValue("r", val.r);
				info.AddValue("g", val.g);
				info.AddValue("b", val.b);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				Color32 val = (Color32)obj;
				val.a = info.GetByte("a");
				val.r = info.GetByte("r");
				val.g = info.GetByte("g");
				val.b = info.GetByte("b");
				return val;
			}
		}

		private class GameObjectSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				string gameObjectPath = GetGameObjectPath(obj as GameObject);
				info.AddValue("path", gameObjectPath);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				string @string = info.GetString("path");
				GameObject val = FindGameObject(@string);
				if (val == null)
				{
					throw new KeyNotFoundException("Could not locate Game Object at path " + @string);
				}
				return val;
			}
		}

		private class ObjectSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				Type type = obj.GetType();
				MemberInfo[] array = (from m in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where m.MemberType == MemberTypes.Field
				select m).ToArray();
				object[] array2 = new object[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					MemberInfo memberInfo = array[i];
					if (memberInfo.GetCustomAttributes(inherit: true).Any((object attr) => attr is NonSerializedAttribute))
					{
						array2[i] = null;
					}
					else
					{
						array2[i] = ((FieldInfo)memberInfo).GetValue(obj);
					}
				}
				info.AddValue("type", type.AssemblyQualifiedName);
				info.AddValue("members", ParameterEncoder(array2));
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				string @string = info.GetString("type");
				object[] array = ParameterDecoder((byte[])info.GetValue("members", typeof(byte[])));
				Type type = Type.GetType(@string, throwOnError: false);
				if (type == null)
				{
					throw new KeyNotFoundException("Could not load type " + @string);
				}
				MemberInfo[] array2 = (from m in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where m.MemberType == MemberTypes.Field
				select m).ToArray();
				ScriptableObject val = ScriptableObject.CreateInstance(type);
				if (val == null)
				{
					throw new NullReferenceException("Could not instantiate Scriptable Object of type " + type.Name);
				}
				obj = val;
				if (array2.Length != array.Length)
				{
					throw new ArgumentException("Received members does not match the number of valid members present for " + @string);
				}
				for (int i = 0; i < array.Length; i++)
				{
					MemberInfo memberInfo = array2[i];
					if (!memberInfo.GetCustomAttributes(inherit: true).Any((object attr) => attr is NonSerializedAttribute))
					{
						((FieldInfo)memberInfo).SetValue(val, array[i]);
					}
				}
				return val;
			}
		}

		private class ArraySurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				Type type = obj.GetType();
				Array array = obj as Array;
				info.AddValue("type", type.AssemblyQualifiedName);
				int arrayRank = type.GetArrayRank();
				if (arrayRank <= 0 || arrayRank > 4)
				{
					info.AddValue("rank", 0);
					Debug.LogError((object)"ArrayRank out of range (1-4) in serialized array - not transmitted");
				}
				else
				{
					info.AddValue("rank", arrayRank);
					int num = 1;
					int[] array2 = new int[4]
					{
						1,
						1,
						1,
						1
					};
					for (int i = 0; i < arrayRank; i++)
					{
						array2[i] = array.GetLength(i);
						num *= array2[i];
					}
					if (num > 65536)
					{
						Debug.LogError((object)"Too many elements in serialized array - not transmitted");
						for (int j = 0; j < arrayRank; j++)
						{
							info.AddValue("dim_" + j.ToString(), 0);
						}
					}
					else
					{
						for (int k = 0; k < arrayRank; k++)
						{
							info.AddValue("dim_" + k.ToString(), array2[k]);
						}
						int[] array3 = new int[arrayRank];
						for (int l = 0; l < array2[3]; l++)
						{
							if (arrayRank > 3)
							{
								array3[3] = l;
							}
							for (int m = 0; m < array2[2]; m++)
							{
								if (arrayRank > 2)
								{
									array3[2] = m;
								}
								for (int n = 0; n < array2[1]; n++)
								{
									if (arrayRank > 1)
									{
										array3[1] = n;
									}
									for (int num2 = 0; num2 < array2[0]; num2++)
									{
										array3[0] = num2;
										byte[] value = ParameterEncoder(array.GetValue(array3));
										info.AddValue("elem_" + num2 + "." + n + "." + m + "." + l, value);
									}
								}
							}
						}
					}
				}
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				string @string = info.GetString("type");
				Type type = Type.GetType(@string, throwOnError: false);
				if (type == null)
				{
					throw new KeyNotFoundException("Could not load type " + @string);
				}
				int num = (int)info.GetValue("rank", typeof(int));
				if (num <= 0 || num > 4)
				{
					Debug.LogError((object)"ArrayRank out of range (1-4) in serialized array - not recieved");
					return null;
				}
				int num2 = 1;
				int[] array = new int[4]
				{
					1,
					1,
					1,
					1
				};
				for (int i = 0; i < num; i++)
				{
					array[i] = (int)info.GetValue("dim_" + i.ToString(), typeof(int));
					num2 *= array[i];
				}
				if (num2 > 65536)
				{
					Debug.LogError((object)"Too many elements in serialized array - not received");
					return null;
				}
				Array array2 = Array.CreateInstance(type.GetElementType(), array);
				int[] array3 = new int[num];
				for (int j = 0; j < array[3]; j++)
				{
					if (num > 3)
					{
						array3[3] = j;
					}
					for (int k = 0; k < array[2]; k++)
					{
						if (num > 2)
						{
							array3[2] = k;
						}
						for (int l = 0; l < array[1]; l++)
						{
							if (num > 1)
							{
								array3[1] = l;
							}
							for (int m = 0; m < array[0]; m++)
							{
								array3[0] = m;
								byte[] dataParameters = (byte[])info.GetValue("elem_" + m + "." + l + "." + k + "." + j, typeof(byte[]));
								object value = ParameterDecoder(dataParameters);
								array2.SetValue(Convert.ChangeType(value, type.GetElementType()), array3);
							}
						}
					}
				}
				return array2;
			}
		}

		private class GenericSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				Type type = obj.GetType();
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					List<object> list = ((IList)obj).Cast<object>().ToList();
					short num = (short)list.Count;
					info.AddValue("type", type.AssemblyQualifiedName);
					info.AddValue("length", num);
					for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
					{
						byte[] value = ParameterEncoder(list[num2]);
						info.AddValue(num2.ToString(), value);
					}
				}
				else if (type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
				{
					List<object> list2 = ((IDictionary)obj).Keys.Cast<object>().ToList();
					List<object> list3 = ((IDictionary)obj).Values.Cast<object>().ToList();
					short num3 = (short)((IDictionary)obj).Count;
					info.AddValue("type", type.AssemblyQualifiedName);
					info.AddValue("length", num3);
					for (short num4 = 0; num4 < num3; num4 = (short)(num4 + 1))
					{
						byte[] value = ParameterEncoder(list2[num4]);
						info.AddValue("key_" + num4.ToString(), value);
						value = ParameterEncoder(list3[num4]);
						info.AddValue("value_" + num4.ToString(), value);
					}
				}
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				string @string = info.GetString("type");
				Type type = Type.GetType(@string, throwOnError: false);
				if (type == null)
				{
					throw new KeyNotFoundException("Could not load type " + @string);
				}
				object result = null;
				Type[] genericArguments = type.GetGenericArguments();
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArguments));
					short @int = info.GetInt16("length");
					for (short num = 0; num < @int; num = (short)(num + 1))
					{
						byte[] dataParameters = (byte[])info.GetValue(num.ToString(), typeof(byte[]));
						object[] array = ParameterDecoder(dataParameters);
						list.Add(array[0]);
					}
					result = list;
				}
				else if (type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
				{
					IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(genericArguments));
					short int2 = info.GetInt16("length");
					for (short num2 = 0; num2 < int2; num2 = (short)(num2 + 1))
					{
						byte[] dataParameters2 = (byte[])info.GetValue("key_" + num2.ToString(), typeof(byte[]));
						object[] array2 = ParameterDecoder(dataParameters2);
						byte[] dataParameters3 = (byte[])info.GetValue("value_" + num2.ToString(), typeof(byte[]));
						object[] array3 = ParameterDecoder(dataParameters3);
						dictionary.Add(array2[0], array3[0]);
					}
					result = dictionary;
				}
				return result;
			}
		}

		private static ISurrogateSelector _networkSurrogateSelector;

		private static ISurrogateSelector SurrogateSelector
		{
			get
			{
				if (_networkSurrogateSelector == null)
				{
					_networkSurrogateSelector = new NetworkSurrogateSelector();
				}
				return _networkSurrogateSelector;
			}
		}

		public static VRC_EventDispatcher Dispatcher
		{
			get
			{
				GameObject val = GameObject.Find("/VRC_OBJECTS/Dispatcher");
				if (val == null)
				{
					return null;
				}
				return val.GetComponent<VRC_EventDispatcher>();
			}
		}

		public static byte[] ParameterEncoder(params object[] parameters)
		{
			if (parameters == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			IFormatter formatter = new BinaryFormatter();
			formatter.SurrogateSelector = SurrogateSelector;
			try
			{
				formatter.Serialize(memoryStream, parameters);
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Something went wrong serializing RPC parameters: \n" + ex.Message + "\n" + ex.StackTrace));
				return null;
				IL_0054:;
			}
			memoryStream.Flush();
			return memoryStream.ToArray();
		}

		public static object[] ParameterDecoder(byte[] dataParameters, bool rethrow = false)
		{
			if (dataParameters != null && dataParameters.Length > 0)
			{
				MemoryStream serializationStream = new MemoryStream(dataParameters);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.SurrogateSelector = SurrogateSelector;
				try
				{
					return (object[])binaryFormatter.Deserialize(serializationStream);
				}
				catch (Exception ex)
				{
					if (rethrow)
					{
						throw ex;
					}
					Debug.LogError((object)("Error decoding parameters: " + ex.Message + "\n" + ex.StackTrace));
					return null;
					IL_006a:
					object[] result;
					return result;
				}
			}
			return new object[0];
		}

		private static string GetGameObjectPathFallback(GameObject go)
		{
			string text = string.Empty;
			while (go != null)
			{
				text = ((!(text == string.Empty)) ? (go.get_name() + "/" + text) : go.get_name());
				if (go.get_transform().get_parent() == null)
				{
					text = "/" + text;
					break;
				}
				go = go.get_transform().get_parent().get_gameObject();
			}
			return text;
		}

		private static string GetGameObjectPath(GameObject go)
		{
			VRC_EventDispatcher dispatcher = Dispatcher;
			if (dispatcher != null)
			{
				return dispatcher.GetGameObjectPath(go);
			}
			return GetGameObjectPathFallback(go);
		}

		private static GameObject FindGameObject(string path)
		{
			VRC_EventDispatcher dispatcher = Dispatcher;
			if (dispatcher != null)
			{
				return dispatcher.FindGameObject(path, suppressErrors: true);
			}
			return GameObject.Find(path);
		}
	}
}
