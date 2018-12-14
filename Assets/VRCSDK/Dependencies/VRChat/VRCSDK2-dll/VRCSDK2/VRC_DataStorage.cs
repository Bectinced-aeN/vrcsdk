using System;
using UnityEngine;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_ObjectSync))]
	public class VRC_DataStorage : MonoBehaviour
	{
		public enum VrcDataMirror
		{
			None,
			Rain,
			Animator,
			SerializeComponent
		}

		public enum VrcDataType
		{
			None,
			Bool,
			Int,
			Float,
			String,
			SerializeBytes,
			SerializeObject
		}

		[Serializable]
		public class VrcDataElement
		{
			public string name;

			public VrcDataMirror mirror;

			public VrcDataType type;

			public VRC_SerializableBehaviour serializeComponent;

			public bool valueBool;

			public int valueInt;

			public float valueFloat;

			public string valueString;

			public byte[] valueSerializedBytes;

			public bool modified;

			public bool added;

			public bool Serialize<T>(T objectToSerialize)
			{
				try
				{
					return VRC_DataStorage.Serialize(this, objectToSerialize);
					IL_0017:
					bool result;
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("Could not serialize " + typeof(T).Name + ": " + ex.Message + "\n" + ex.StackTrace));
					return false;
					IL_0070:
					bool result;
					return result;
				}
			}

			public bool Deserialize<T>(out T objectToDeserialize)
			{
				objectToDeserialize = default(T);
				try
				{
					if (VRC_DataStorage.Deserialize(this, out object obj) && obj is T)
					{
						objectToDeserialize = (T)obj;
						return true;
					}
					return false;
					IL_0046:
					bool result;
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("Could not deserialize " + typeof(T).Name + ": " + ex.Message + "\n" + ex.StackTrace));
					return false;
					IL_009f:
					bool result;
					return result;
				}
			}
		}

		public delegate void InitializationDelegate(VRC_DataStorage obj);

		public delegate bool SerializationDelegate(VrcDataElement ds, object obj);

		public delegate bool DeserializationDelegate(VrcDataElement ds, out object obj);

		public delegate void ResizeDelegate(VRC_DataStorage obj, int size);

		public delegate void DataElementDelegate(VRC_DataStorage obj, int idx);

		public VrcDataElement[] data;

		public static InitializationDelegate Initialize;

		public static SerializationDelegate Serialize;

		public static DeserializationDelegate Deserialize;

		public static ResizeDelegate _Resize;

		public static Func<VRC_DataStorage, string, int> _GetElementIndex;

		public event DataElementDelegate ElementChanged;

		public event DataElementDelegate ElementAdded;

		public event DataElementDelegate ElementRemoved;

		public VRC_DataStorage()
			: this()
		{
		}

		public int GetElementIndex(string name)
		{
			return _GetElementIndex(this, name);
		}

		public VrcDataElement GetElement(string name)
		{
			int elementIndex = GetElementIndex(name);
			VrcDataElement result = null;
			if (data != null && data.Length > elementIndex)
			{
				result = data[elementIndex];
			}
			return result;
		}

		public void OnDataElementChanged(int idx)
		{
			if (this.ElementChanged != null)
			{
				this.ElementChanged(this, idx);
			}
		}

		public void OnDataElementAdded(int idx)
		{
			if (this.ElementAdded != null)
			{
				this.ElementAdded(this, idx);
			}
		}

		public void OnDataElementRemoved(int idx)
		{
			if (this.ElementRemoved != null)
			{
				this.ElementRemoved(this, idx);
			}
		}

		public void Resize(int size)
		{
			if (_Resize != null)
			{
				_Resize(this, size);
			}
		}

		private void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}
	}
}
