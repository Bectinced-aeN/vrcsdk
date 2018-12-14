using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SceneSmoothShift : MonoBehaviour, IVRCEventReceiver
	{
		public AnimationCurve ShiftInterpolationCurve = new AnimationCurve((Keyframe[])new Keyframe[2]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		public float ShiftSpeed = 1f;

		public Transform ShiftStart;

		public Transform ShiftEnd;

		private float TargetPosition;

		private float ShiftPosition;

		public VRC_SceneSmoothShift()
			: this()
		{
		}//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown


		private void Start()
		{
			TargetPosition = ShiftPosition;
		}

		private void OnValidate()
		{
			TargetPosition = ShiftPosition;
			Update();
		}

		private void Update()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (ShiftPosition != TargetPosition)
			{
				ShiftPosition += ShiftSpeed * Mathf.Sign(TargetPosition - ShiftPosition) * Time.get_deltaTime();
			}
			ShiftPosition = Mathf.Clamp01(ShiftPosition);
			float num = ShiftInterpolationCurve.Evaluate(ShiftPosition);
			Vector3 position = Vector3.Lerp(ShiftStart.get_position(), ShiftEnd.get_position(), num);
			Quaternion rotation = Quaternion.Lerp(ShiftStart.get_rotation(), ShiftEnd.get_rotation(), num);
			Vector3 localScale = Vector3.Lerp(ShiftStart.get_localScale(), ShiftEnd.get_localScale(), num);
			this.get_transform().set_position(position);
			this.get_transform().set_rotation(rotation);
			this.get_transform().set_localScale(localScale);
		}

		[RPC(new VRC_EventHandler.VrcTargetType[]
		{
			VRC_EventHandler.VrcTargetType.All
		})]
		private void Shift()
		{
			if (TargetPosition == 0f)
			{
				TargetPosition = 1f;
			}
			else
			{
				TargetPosition = 0f;
			}
		}
	}
}
