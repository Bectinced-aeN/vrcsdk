using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SceneResetPosition : MonoBehaviour, IVRCEventReceiver
	{
		public Transform Position;

		public bool RemoveVelocity = true;

		private Rigidbody rigidbody;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private Vector3 initialScale;

		public VRC_SceneResetPosition()
			: this()
		{
		}

		private void Start()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (Position == null)
			{
				initialPosition = this.get_transform().get_position();
				initialRotation = this.get_transform().get_rotation();
				initialScale = this.get_transform().get_localScale();
			}
		}

		public void ResetPosition()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			if (rigidbody == null)
			{
				rigidbody = this.GetComponent<Rigidbody>();
			}
			if (Position != null)
			{
				this.get_transform().set_position(Position.get_position());
				this.get_transform().set_rotation(Position.get_rotation());
				this.get_transform().set_localScale(Position.get_localScale());
			}
			else
			{
				this.get_transform().set_position(initialPosition);
				this.get_transform().set_rotation(initialRotation);
				this.get_transform().set_localScale(initialScale);
			}
			if (rigidbody != null && RemoveVelocity)
			{
				rigidbody.set_velocity(new Vector3(0f, 0f, 0f));
				rigidbody.set_angularVelocity(new Vector3(0f, 0f, 0f));
			}
		}
	}
}
