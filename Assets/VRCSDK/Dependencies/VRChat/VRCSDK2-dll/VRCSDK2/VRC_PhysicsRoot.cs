using UnityEngine;

namespace VRCSDK2
{
	public class VRC_PhysicsRoot : MonoBehaviour
	{
		private GameObject PhysicsRoot;

		private GameObject[] PhysicsObjects;

		public VRC_PhysicsRoot()
			: this()
		{
		}

		private void Start()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			PhysicsRoot = GameObject.Find("_PhysicsRoot");
			if (PhysicsRoot == null)
			{
				PhysicsRoot = new GameObject();
				PhysicsRoot.set_name("_PhysicsRoot");
			}
			PhysicsObjects = (GameObject[])new GameObject[this.get_transform().get_childCount()];
			for (int i = 0; i < this.get_transform().get_childCount(); i++)
			{
				PhysicsObjects[i] = this.get_transform().GetChild(i).get_gameObject();
			}
			GameObject[] physicsObjects = PhysicsObjects;
			foreach (GameObject val in physicsObjects)
			{
				val.get_transform().set_parent(PhysicsRoot.get_transform());
			}
		}

		private void OnDestroy()
		{
			GameObject[] physicsObjects = PhysicsObjects;
			foreach (GameObject val in physicsObjects)
			{
				Object.Destroy(val);
			}
		}
	}
}
