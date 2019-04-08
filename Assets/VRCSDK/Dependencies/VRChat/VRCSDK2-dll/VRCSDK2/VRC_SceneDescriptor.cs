using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRCSDK2
{
	public class VRC_SceneDescriptor : VRC_Behaviour
	{
		public enum SpawnOrder
		{
			First,
			Sequential,
			Random,
			Demo
		}

		public enum SpawnOrientation
		{
			Default,
			AlignPlayerWithSpawnPoint,
			AlignRoomWithSpawnPoint
		}

		public enum RespawnHeightBehaviour
		{
			Respawn,
			Destroy
		}

		public delegate void IntializationDelegate(VRC_SceneDescriptor sceneDescriptor);

		public Transform[] spawns;

		public SpawnOrder spawnOrder = SpawnOrder.Random;

		public SpawnOrientation spawnOrientation;

		public GameObject ReferenceCamera;

		public float RespawnHeightY = -100f;

		public RespawnHeightBehaviour ObjectBehaviourAtRespawnHeight = RespawnHeightBehaviour.Destroy;

		public bool ForbidUserPortals;

		public bool UseCustomVoiceFalloffRange;

		public float VoiceFalloffRangeNear = 4f;

		public float VoiceFalloffRangeFar = 350f;

		[HideInInspector]
		public bool autoSpatializeAudioSources = true;

		[HideInInspector]
		public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		[HideInInspector]
		public bool[] layerCollisionArr;

		[HideInInspector]
		public int capacity;

		[HideInInspector]
		public bool contentSex;

		[HideInInspector]
		public bool contentViolence;

		[HideInInspector]
		public bool contentGore;

		[HideInInspector]
		public bool contentOther;

		[HideInInspector]
		public bool releasePublic;

		public string unityVersion;

		[HideInInspector]
		[Obsolete("Property is not used.")]
		public string Name;

		[Obsolete("Property is not used.")]
		[HideInInspector]
		public bool NSFW;

		[HideInInspector]
		public Vector3 SpawnPosition = new Vector3(0f, 0f, 0f);

		[HideInInspector]
		public Transform SpawnLocation;

		[HideInInspector]
		public float DrawDistance;

		[HideInInspector]
		public bool useAssignedLayers;

		public List<GameObject> DynamicPrefabs = new List<GameObject>();

		private static Dictionary<string, GameObject> sDynamicPrefabs;

		public List<Material> DynamicMaterials = new List<Material>();

		private static Dictionary<string, Material> sDynamicMaterials;

		[Range(33f, 150f)]
		public int UpdateTimeInMS = 66;

		[HideInInspector]
		public Texture2D[] LightMapsNear;

		[HideInInspector]
		public Texture2D[] LightMapsFar;

		[HideInInspector]
		public LightmapsMode LightMode;

		[HideInInspector]
		public Color RenderAmbientEquatorColor;

		[HideInInspector]
		public Color RenderAmbientGroundColor;

		[HideInInspector]
		public float RenderAmbientIntensity;

		[HideInInspector]
		public Color RenderAmbientLight;

		[HideInInspector]
		public AmbientMode RenderAmbientMode;

		[HideInInspector]
		public SphericalHarmonicsL2 RenderAmbientProbe;

		[HideInInspector]
		public Color RenderAmbientSkyColor;

		[HideInInspector]
		public bool RenderFog;

		[HideInInspector]
		public Color RenderFogColor;

		[HideInInspector]
		public FogMode RenderFogMode;

		[HideInInspector]
		public float RenderFogDensity;

		[HideInInspector]
		public float RenderFogLinearStart;

		[HideInInspector]
		public float RenderFogLinearEnd;

		[HideInInspector]
		public float RenderHaloStrength;

		[HideInInspector]
		public float RenderFlareFadeSpeed;

		[HideInInspector]
		public float RenderFlareStrength;

		[HideInInspector]
		public Cubemap RenderCustomReflection;

		[HideInInspector]
		public DefaultReflectionMode RenderDefaultReflectionMode;

		[HideInInspector]
		public int RenderDefaultReflectionResolution;

		[HideInInspector]
		public int RenderReflectionBounces;

		[HideInInspector]
		public float RenderReflectionIntensity;

		[HideInInspector]
		public Material RenderSkybox;

		public static IntializationDelegate Initialize;

		[HideInInspector]
		public object apiWorld;

		private static VRC_SceneDescriptor _instance;

		public Vector3 portraitCameraPositionOffset = new Vector3(0f, 0f, 0f);

		public Quaternion portraitCameraRotationOffset = Quaternion.AngleAxis(180f, Vector3.get_up());

		public static VRC_SceneDescriptor Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType<VRC_SceneDescriptor>();
				}
				return _instance;
			}
		}

		public static GameObject GetPrefab(string name)
		{
			if (sDynamicPrefabs.ContainsKey(name))
			{
				return sDynamicPrefabs[name];
			}
			if (name.StartsWith("Assets"))
			{
				foreach (string key in sDynamicPrefabs.Keys)
				{
					if (name.Contains(key))
					{
						return sDynamicPrefabs[key];
					}
				}
			}
			return null;
		}

		public static Material GetMaterial(string name)
		{
			if (sDynamicMaterials.ContainsKey(name))
			{
				return sDynamicMaterials[name];
			}
			if (name.StartsWith("Assets"))
			{
				foreach (string key in sDynamicMaterials.Keys)
				{
					if (name.Contains(key))
					{
						return sDynamicMaterials[key];
					}
				}
			}
			return null;
		}

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Debug.LogWarning((object)"Destroying existing Scene Descriptor");
				Object.Destroy(_instance);
			}
			_instance = this;
			sDynamicPrefabs = new Dictionary<string, GameObject>();
			foreach (GameObject dynamicPrefab in DynamicPrefabs)
			{
				if (dynamicPrefab != null && !sDynamicPrefabs.ContainsKey(dynamicPrefab.get_name()))
				{
					sDynamicPrefabs.Add(dynamicPrefab.get_name(), dynamicPrefab);
				}
			}
			sDynamicMaterials = new Dictionary<string, Material>();
			foreach (Material dynamicMaterial in DynamicMaterials)
			{
				if (dynamicMaterial != null && !sDynamicMaterials.ContainsKey(dynamicMaterial.get_name()))
				{
					sDynamicMaterials.Add(dynamicMaterial.get_name(), dynamicMaterial);
				}
			}
			if (Initialize != null)
			{
				Initialize(this);
			}
		}

		public void PositionPortraitCamera(Transform cam)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			cam.set_position(this.get_transform().TransformPoint(portraitCameraPositionOffset));
			cam.set_rotation(this.get_transform().get_rotation() * portraitCameraRotationOffset);
		}
	}
}
