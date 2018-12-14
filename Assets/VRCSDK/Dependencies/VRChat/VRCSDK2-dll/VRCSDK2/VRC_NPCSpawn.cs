using UnityEngine;

namespace VRCSDK2
{
	public class VRC_NPCSpawn : MonoBehaviour
	{
		public delegate void InstantiationDelegate(VRC_NPCSpawn obj);

		public string npcName = string.Empty;

		public string blueprintId = string.Empty;

		public RuntimeAnimatorController customAnimation;

		public float scale = 1f;

		public static InstantiationDelegate Initialize;

		public GameObject npcGameObject;

		public VRC_NPCSpawn()
			: this()
		{
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
