using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SerializableBehaviour : MonoBehaviour
	{
		public VRC_SerializableBehaviour()
			: this()
		{
		}

		public virtual byte[] GetBytes()
		{
			return new byte[0];
		}

		public virtual void SetBytes(byte[] stream)
		{
		}
	}
}
