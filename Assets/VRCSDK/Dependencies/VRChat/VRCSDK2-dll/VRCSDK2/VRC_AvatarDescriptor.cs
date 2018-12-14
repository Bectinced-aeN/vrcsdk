using UnityEngine;

namespace VRCSDK2
{
	public class VRC_AvatarDescriptor : VRC_Behaviour
	{
		public enum AnimationSet
		{
			Male,
			Female,
			None
		}

		public enum LipSyncStyle
		{
			Default,
			JawFlapBone,
			JawFlapBlendShape,
			VisemeBlendShape
		}

		public enum Viseme
		{
			sil,
			PP,
			FF,
			TH,
			DD,
			kk,
			CH,
			SS,
			nn,
			RR,
			aa,
			E,
			ih,
			oh,
			ou,
			Count
		}

		public string Name;

		public Vector3 ViewPosition = new Vector3(0f, 1.6f, 0.2f);

		public AnimationSet Animations;

		public AnimatorOverrideController CustomStandingAnims;

		public AnimatorOverrideController CustomSittingAnims;

		public bool ScaleIPD = true;

		public LipSyncStyle lipSync;

		public Transform lipSyncJawBone;

		public SkinnedMeshRenderer VisemeSkinnedMesh;

		public string MouthOpenBlendShapeName = "Facial_Blends.Jaw_Down";

		public string[] VisemeBlendShapes;

		[HideInInspector]
		public object apiAvatar;

		public string unityVersion;

		public Vector3 portraitCameraPositionOffset = new Vector3(0f, 0f, 0f);

		public Quaternion portraitCameraRotationOffset = Quaternion.AngleAxis(180f, Vector3.get_up());

		private void OnDrawGizmosSelected()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			float num = ViewPosition.get_magnitude() / 1.6f;
			if (num < 1f)
			{
				num = 1f;
			}
			Vector3 val = this.get_transform().get_position() + ViewPosition;
			Gizmos.DrawRay(this.get_transform().get_position(), Vector3.get_forward());
			Gizmos.DrawSphere(val, 0.01f * num);
		}

		public void PositionPortraitCamera(Transform cam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (portraitCameraPositionOffset == Vector3.get_zero())
			{
				float y = ViewPosition.y;
				float num = y * 0.8f;
				float num2 = y * 0.7f;
				portraitCameraPositionOffset = new Vector3(0f, num, num2);
			}
			cam.set_position(this.get_transform().TransformPoint(portraitCameraPositionOffset));
			cam.set_rotation(this.get_transform().get_rotation() * portraitCameraRotationOffset);
		}
	}
}
