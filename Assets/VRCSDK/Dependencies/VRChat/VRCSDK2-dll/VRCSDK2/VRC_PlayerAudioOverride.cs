using UnityEngine;

namespace VRCSDK2
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class VRC_PlayerAudioOverride : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_PlayerAudioOverride obj);

		public delegate void RegionDelegate(Collider collider);

		[Tooltip("Loudness increase, 0-24 (decibels).")]
		public float VoiceGain = 15f;

		[Tooltip("Distance where loudness falls off to zero (meters).")]
		public float VoiceFar = 25f;

		[Tooltip("Distance where loudness begins to falloff (meters).")]
		public float VoiceNear;

		[Tooltip("Radius of a spherical shaped sound source (meters).")]
		public float VoiceVolumetricRadius;

		[Tooltip("Disable the lowpass distance filter. May be useful for non-standard ranges.")]
		public bool VoiceDisableLowpass;

		[Tooltip("Limit for avatar audio gain (0-24 decibels, negative to mute).")]
		public float AvatarGainLimit = 10f;

		[Tooltip("Limit for avatar audio max range (meters, 0 to mute).")]
		public float AvatarFarLimit = 40f;

		[Tooltip("Limit for avatar min range (meters).")]
		public float AvatarNearLimit = 40f;

		[Tooltip("Limit for avatar volumetric audio (meters).")]
		public float AvatarVolumetricRadiusLimit = 40f;

		[Tooltip("Force spatial avatar audio.")]
		public bool AvatarForceSpatial;

		[Tooltip("Allow avatar audio to use AudioSource Volume Curve for falloff. Otherwise, Inverse Square falloff is enforced.")]
		public bool AvatarAllowCustomCurve = true;

		[Tooltip("If checked, these settings are global. Otherwise, settings only affect players who enter the trigger region.")]
		public bool global;

		[Tooltip("When a player enters this region, their settings are changed.")]
		public Collider region;

		[Tooltip("Higher number means higher priority, can be negative.")]
		public float regionPriority;

		public static InitializationDelegate Initialize;

		public RegionDelegate onRegionEnter;

		public RegionDelegate onRegionExit;

		public VRC_PlayerAudioOverride()
			: this()
		{
		}

		private void Awake()
		{
			region = this.get_gameObject().GetComponent<Collider>();
			if (region != null)
			{
				region.set_isTrigger(true);
			}
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (onRegionEnter != null)
			{
				onRegionEnter(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (onRegionExit != null)
			{
				onRegionExit(other);
			}
		}
	}
}
