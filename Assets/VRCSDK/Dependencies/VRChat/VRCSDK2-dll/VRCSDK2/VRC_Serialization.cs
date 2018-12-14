using System;
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
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				Transform val = obj;
				if (val.get_gameObject() == null)
				{
					throw new ArgumentException("Transform must have an associated gameObject.");
				}
				object[] parameters = new object[9]
				{
					val.get_position(),
					val.get_rotation(),
					val.get_localPosition(),
					val.get_localRotation(),
					val.get_localScale(),
					val.get_forward(),
					val.get_right(),
					val.get_up(),
					val.get_name()
				};
				string gameObjectPath = GetGameObjectPath(val.get_gameObject());
				info.AddValue("path", gameObjectPath);
				info.AddValue("data", ParameterEncoder(parameters));
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				string @string = info.GetString("path");
				GameObject val = FindGameObject(@string);
				if (val == null)
				{
					throw new KeyNotFoundException("Could not locate Transform at path " + @string);
				}
				Transform transform = val.get_transform();
				byte[] dataParameters = (byte[])info.GetValue("data", typeof(byte[]));
				object[] array = ParameterDecoder(dataParameters);
				transform.set_position((Vector3)array[0]);
				transform.set_rotation((Quaternion)array[1]);
				transform.set_localPosition((Vector3)array[2]);
				transform.set_localRotation((Quaternion)array[3]);
				transform.set_localScale((Vector3)array[4]);
				transform.set_forward((Vector3)array[5]);
				transform.set_right((Vector3)array[6]);
				transform.set_up((Vector3)array[7]);
				transform.set_name((string)array[8]);
				return transform;
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
