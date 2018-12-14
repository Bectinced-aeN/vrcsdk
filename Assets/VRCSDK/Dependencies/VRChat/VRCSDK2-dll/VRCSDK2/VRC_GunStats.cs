using UnityEngine;

namespace VRCSDK2
{
	public class VRC_GunStats : MonoBehaviour
	{
		public float rateOfFire = 10f;

		public float damage = 10f;

		public bool fullAuto = true;

		public GameObject muzzleEffect;

		public AudioSource muzzleAudio;

		public AudioClip[] fireAudio;

		public AudioSource reloadAudio;

		public GameObject[] hitEffects;

		public GameObject leftHandContact;

		public bool leftHandPositionOnly;

		public int clipSize;

		public AudioClip EmptyClipFire;

		public VRC_GunStats()
			: this()
		{
		}
	}
}
