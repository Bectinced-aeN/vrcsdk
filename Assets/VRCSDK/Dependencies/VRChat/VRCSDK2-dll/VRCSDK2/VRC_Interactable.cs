using UnityEngine;

namespace VRCSDK2
{
	public abstract class VRC_Interactable : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_Interactable obj);

		public delegate bool ValidDelegate(VRC_Interactable obj, VRC_PlayerApi player);

		public Transform interactTextPlacement;

		public string interactText = "Use";

		[HideInInspector]
		public GameObject interactTextGO;

		[Range(0f, 100f)]
		public float proximity = 3.40282347E+38f;

		public static InitializationDelegate Initialize;

		public static ValidDelegate CheckValid;

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

		public virtual bool IsInteractiveForPlayer(VRC_PlayerApi player)
		{
			if (CheckValid != null)
			{
				return CheckValid(this, player);
			}
			return true;
		}
	}
}
