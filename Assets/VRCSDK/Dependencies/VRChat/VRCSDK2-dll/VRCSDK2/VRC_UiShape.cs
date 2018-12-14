using UnityEngine;

namespace VRCSDK2
{
	public class VRC_UiShape : MonoBehaviour
	{
		public delegate Camera GetEventCameraDelegate();

		private Canvas uiCanvas;

		public static GetEventCameraDelegate GetEventCamera;

		public VRC_UiShape()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			uiCanvas = this.GetComponent<Canvas>();
			if (Object.op_Implicit(uiCanvas))
			{
				RectTransform val = this.get_transform();
				Rect rect = val.get_rect();
				Vector2 size = rect.get_size();
				BoxCollider val2 = this.get_gameObject().GetComponent<BoxCollider>();
				if (val2 == null)
				{
					val2 = this.get_gameObject().AddComponent<BoxCollider>();
				}
				val2.set_size(new Vector3(size.x, size.y, 1f));
				Vector2 val3 = Vector2.op_Implicit(Vector3.get_zero());
				Vector2 pivot = val.get_pivot();
				float num = 0.5f - pivot.x;
				Vector3 size2 = val2.get_size();
				val3.x = num * size2.x;
				Vector2 pivot2 = val.get_pivot();
				float num2 = 0.5f - pivot2.y;
				Vector3 size3 = val2.get_size();
				val3.y = num2 * size3.y;
				val2.set_center(Vector2.op_Implicit(val3));
			}
		}

		private void Start()
		{
			uiCanvas.set_worldCamera(GetEventCamera());
		}
	}
}
