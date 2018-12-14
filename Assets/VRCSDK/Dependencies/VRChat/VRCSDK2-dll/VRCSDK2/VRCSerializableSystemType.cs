using System;
using UnityEngine;

namespace VRCSDK2
{
	[Serializable]
	public class VRCSerializableSystemType
	{
		[SerializeField]
		private string m_Name;

		[SerializeField]
		private string m_AssemblyQualifiedName;

		[SerializeField]
		private string m_AssemblyName;

		private Type m_SystemType;

		public string Name => m_Name;

		public string AssemblyQualifiedName => m_AssemblyQualifiedName;

		public string AssemblyName => m_AssemblyName;

		public Type SystemType
		{
			get
			{
				if (m_SystemType == null)
				{
					GetSystemType();
				}
				return m_SystemType;
			}
		}

		public VRCSerializableSystemType(Type _SystemType)
		{
			m_SystemType = _SystemType;
			m_Name = _SystemType.Name;
			m_AssemblyQualifiedName = _SystemType.AssemblyQualifiedName;
			m_AssemblyName = _SystemType.Assembly.FullName;
		}

		private void GetSystemType()
		{
			m_SystemType = Type.GetType(m_AssemblyQualifiedName);
		}

		public override bool Equals(object obj)
		{
			VRCSerializableSystemType vRCSerializableSystemType = obj as VRCSerializableSystemType;
			if ((object)vRCSerializableSystemType == null)
			{
				return false;
			}
			return Equals(vRCSerializableSystemType);
		}

		public bool Equals(VRCSerializableSystemType _Object)
		{
			return _Object.SystemType.Equals(SystemType);
		}

		public override int GetHashCode()
		{
			return SystemType.GetHashCode() * 17;
		}

		public static bool operator ==(VRCSerializableSystemType a, VRCSerializableSystemType b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(VRCSerializableSystemType a, VRCSerializableSystemType b)
		{
			return !(a == b);
		}
	}
}
