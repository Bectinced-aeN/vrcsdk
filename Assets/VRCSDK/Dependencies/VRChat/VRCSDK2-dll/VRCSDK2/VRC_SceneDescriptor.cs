using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRCSDK2
{
	[RequireComponent(typeof(VRC_EventHandler))]
	public class VRC_SceneDescriptor : VRC_Behaviour
	{
		public enum SpawnOrder
		{
			First,
			Sequential,
			Random,
			Demo
		}

		public enum RespawnHeightBehaviour
		{
			Respawn,
			Destroy
		}

		public delegate void IntializationDelegate(VRC_SceneDescriptor sceneDescriptor);

		public Transform[] spawns;

		public SpawnOrder spawnOrder = SpawnOrder.Random;

		public GameObject ReferenceCamera;

		public float RespawnHeightY = -100f;

		public RespawnHeightBehaviour ObjectBehaviourAtRespawnHeight = RespawnHeightBehaviour.Destroy;

		[HideInInspector]
		public bool useAssignedLayers;

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

		[Obsolete("Property is not used.")]
		[HideInInspector]
		public string Name;

		[HideInInspector]
		[Obsolete("Property is not used.")]
		public bool NSFW;

		[HideInInspector]
		public Vector3 SpawnPosition = new Vector3(0f, 0f, 0f);

		[HideInInspector]
		public Transform SpawnLocation;

		[HideInInspector]
		public float DrawDistance;

		public List<GameObject> DynamicPrefabs = new List<GameObject>();

		private static Dictionary<string, GameObject> sDynamicPrefabs;

		public List<Material> DynamicMaterials = new List<Material>();

		private static Dictionary<string, Material> sDynamicMaterials;

		[Range(1f, 50f)]
		public int UpdateTimeInMS = 10;

		[HideInInspector]
		public Texture2D[] LightMapsNear;

		[HideInInspector]
		public Texture2D[] LightMapsFar;

		[HideInInspector]
		public LightmapsMode LightMode;

		[HideInInspector]
		public bool LoadRenderSettings;

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
	}
}
