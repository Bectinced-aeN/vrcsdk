using UnityEngine;

namespace VRCSDK2
{
	public abstract class VRC_Interactable : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_Interactable obj);

		public Transform interactTextPlacement;

		public string interactText = "Use";

		[HideInInspector]
		public GameObject interactTextGO;

		public float proximity = 0.1f;

		public static InitializationDelegate Initialize;

		protected VRC_Interactable()
			: this()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void Awake()
		{
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public abstract void Interact();
	}
}
