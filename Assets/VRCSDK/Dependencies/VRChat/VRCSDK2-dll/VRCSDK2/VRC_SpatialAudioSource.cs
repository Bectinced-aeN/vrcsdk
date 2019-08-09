using UnityEngine;

namespace VRCSDK2
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	[ExecuteInEditMode]
	public class VRC_SpatialAudioSource : MonoBehaviour
	{
		public delegate void InitializationDelegate(VRC_SpatialAudioSource obj);

		[Tooltip("Loudness increase in decibels, can be negative.")]
		public float Gain = 10f;

		[Tooltip("Distance where loudness falls off to zero, in meters.")]
		public float Far = 40f;

		[Tooltip("Distance where loudness begins to falloff, in meters. Default of 0 ensures accurate spatialization.")]
		public float Near;

		[Tooltip("Radius in meters of a spherical shaped sound source. Default of 0 simulates a point-source (recommended).")]
		public float VolumetricRadius;

		[Tooltip("Enable Spatialization. Uncheck only for directionless audio.")]
		public bool EnableSpatialization = true;

		[Tooltip("Use the AudioSource '3D Sound Settings' volume curve. If unchecked, use Inverse Square falloff.")]
		public bool UseAudioSourceVolumeCurve;

		public static InitializationDelegate Initialize;

		private AudioSource _source;

		public VRC_SpatialAudioSource()
			: this()
		{
		}

		private void Awake()
		{
			_source = this.GetComponent<AudioSource>();
			if (_source == null)
			{
				Debug.LogErrorFormat("[{0}:VRC_SpatialAudioSource without an AudioSource component!", new object[1]
				{
					this.get_gameObject().get_name()
				});
			}
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		private void OnDrawGizmosSelected()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			if (!UseAudioSourceVolumeCurve)
			{
				Color color = default(Color);
				color.r = 1f;
				color.g = 0.5f;
				color.b = 0f;
				color.a = 1f;
				Gizmos.set_color(color);
				Gizmos.DrawWireSphere(this.get_transform().get_position(), Near);
				color.a = 0.1f;
				Gizmos.set_color(color);
				Gizmos.DrawSphere(this.get_transform().get_position(), Near);
				color.r = 1f;
				color.g = 0f;
				color.b = 0f;
				color.a = 1f;
				Gizmos.set_color(Color.get_red());
				Gizmos.DrawWireSphere(this.get_transform().get_position(), Far);
				color.a = 0.1f;
				Gizmos.set_color(color);
				Gizmos.DrawSphere(this.get_transform().get_position(), Far);
				color.r = 1f;
				color.g = 0f;
				color.b = 1f;
				color.a = 1f;
				Gizmos.set_color(color);
				Gizmos.DrawWireSphere(this.get_transform().get_position(), VolumetricRadius);
				color.a = 0.1f;
				Gizmos.set_color(color);
				Gizmos.DrawSphere(this.get_transform().get_position(), VolumetricRadius);
			}
		}
	}
}
