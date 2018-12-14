using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	[ExecuteInEditMode]
	public class VRC_Water : MonoBehaviour
	{
		public enum WaterMode
		{
			Simple,
			Reflective,
			Refractive
		}

		public WaterMode waterMode = WaterMode.Refractive;

		public bool disablePixelLights = true;

		public int textureSize = 256;

		public float clipPlaneOffset = 0.07f;

		public LayerMask reflectLayers = LayerMask.op_Implicit(-1);

		public LayerMask refractLayers = LayerMask.op_Implicit(-1);

		public bool TurnOffWaterOcclusion = true;

		private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();

		private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>();

		private RenderTexture m_ReflectionTexture;

		private RenderTexture m_RefractionTexture;

		private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;

		private int m_OldReflectionTextureSize;

		private int m_OldRefractionTextureSize;

		private static bool s_InsideWater;

		public VRC_Water()
			: this()
		{
		}//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)


		public void OnWillRenderObject()
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			if (this.get_enabled() && Object.op_Implicit(this.GetComponent<Renderer>()) && Object.op_Implicit(this.GetComponent<Renderer>().get_sharedMaterial()) && this.GetComponent<Renderer>().get_enabled())
			{
				Camera current = Camera.get_current();
				if (Object.op_Implicit(current) && !s_InsideWater)
				{
					s_InsideWater = true;
					m_HardwareWaterSupport = FindHardwareWaterSupport();
					WaterMode waterMode = GetWaterMode();
					CreateWaterObjects(current, out Camera reflectionCamera, out Camera refractionCamera);
					Vector3 position = this.get_transform().get_position();
					Vector3 up = this.get_transform().get_up();
					int pixelLightCount = QualitySettings.get_pixelLightCount();
					if (disablePixelLights)
					{
						QualitySettings.set_pixelLightCount(0);
					}
					UpdateCameraModes(current, reflectionCamera);
					UpdateCameraModes(current, refractionCamera);
					if (waterMode >= WaterMode.Reflective)
					{
						float num = 0f - Vector3.Dot(up, position) - clipPlaneOffset;
						Vector4 plane = default(Vector4);
						plane._002Ector(up.x, up.y, up.z, num);
						Matrix4x4 reflectionMat = Matrix4x4.get_zero();
						CalculateReflectionMatrix(ref reflectionMat, plane);
						Vector3 position2 = current.get_transform().get_position();
						Vector3 position3 = reflectionMat.MultiplyPoint(position2);
						reflectionCamera.set_worldToCameraMatrix(current.get_worldToCameraMatrix() * reflectionMat);
						Vector4 val = CameraSpacePlane(reflectionCamera, position, up, 1f);
						reflectionCamera.set_projectionMatrix(current.CalculateObliqueMatrix(val));
						reflectionCamera.set_cullingMask(-17 & reflectLayers.get_value());
						reflectionCamera.set_targetTexture(m_ReflectionTexture);
						bool invertCulling = GL.get_invertCulling();
						GL.set_invertCulling(!invertCulling);
						reflectionCamera.get_transform().set_position(position3);
						Vector3 eulerAngles = current.get_transform().get_eulerAngles();
						reflectionCamera.get_transform().set_eulerAngles(new Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z));
						reflectionCamera.Render();
						reflectionCamera.get_transform().set_position(position2);
						GL.set_invertCulling(invertCulling);
						this.GetComponent<Renderer>().get_sharedMaterial().SetTexture("_ReflectionTex", m_ReflectionTexture);
					}
					if (waterMode >= WaterMode.Refractive)
					{
						refractionCamera.set_worldToCameraMatrix(current.get_worldToCameraMatrix());
						Vector4 val2 = CameraSpacePlane(refractionCamera, position, up, -1f);
						refractionCamera.set_projectionMatrix(current.CalculateObliqueMatrix(val2));
						refractionCamera.set_cullingMask(-17 & refractLayers.get_value());
						refractionCamera.set_targetTexture(m_RefractionTexture);
						refractionCamera.get_transform().set_position(current.get_transform().get_position());
						refractionCamera.get_transform().set_rotation(current.get_transform().get_rotation());
						refractionCamera.Render();
						this.GetComponent<Renderer>().get_sharedMaterial().SetTexture("_RefractionTex", m_RefractionTexture);
					}
					if (disablePixelLights)
					{
						QualitySettings.set_pixelLightCount(pixelLightCount);
					}
					switch (waterMode)
					{
					case WaterMode.Simple:
						Shader.EnableKeyword("WATER_SIMPLE");
						Shader.DisableKeyword("WATER_REFLECTIVE");
						Shader.DisableKeyword("WATER_REFRACTIVE");
						break;
					case WaterMode.Reflective:
						Shader.DisableKeyword("WATER_SIMPLE");
						Shader.EnableKeyword("WATER_REFLECTIVE");
						Shader.DisableKeyword("WATER_REFRACTIVE");
						break;
					case WaterMode.Refractive:
						Shader.DisableKeyword("WATER_SIMPLE");
						Shader.DisableKeyword("WATER_REFLECTIVE");
						Shader.EnableKeyword("WATER_REFRACTIVE");
						break;
					}
					s_InsideWater = false;
				}
			}
		}

		private void OnDisable()
		{
			if (Object.op_Implicit(m_ReflectionTexture))
			{
				Object.DestroyImmediate(m_ReflectionTexture);
				m_ReflectionTexture = null;
			}
			if (Object.op_Implicit(m_RefractionTexture))
			{
				Object.DestroyImmediate(m_RefractionTexture);
				m_RefractionTexture = null;
			}
			foreach (KeyValuePair<Camera, Camera> reflectionCamera in m_ReflectionCameras)
			{
				Object.DestroyImmediate(reflectionCamera.Value.get_gameObject());
			}
			m_ReflectionCameras.Clear();
			foreach (KeyValuePair<Camera, Camera> refractionCamera in m_RefractionCameras)
			{
				Object.DestroyImmediate(refractionCamera.Value.get_gameObject());
			}
			m_RefractionCameras.Clear();
		}

		private void Update()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit(this.GetComponent<Renderer>()))
			{
				Material sharedMaterial = this.GetComponent<Renderer>().get_sharedMaterial();
				if (Object.op_Implicit(sharedMaterial))
				{
					Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
					float @float = sharedMaterial.GetFloat("_WaveScale");
					Vector4 val = default(Vector4);
					val._002Ector(@float, @float, @float * 0.4f, @float * 0.45f);
					double num = (double)Time.get_timeSinceLevelLoad() / 20.0;
					Vector4 val2 = default(Vector4);
					val2._002Ector((float)Math.IEEERemainder((double)(vector.x * val.x) * num, 1.0), (float)Math.IEEERemainder((double)(vector.y * val.y) * num, 1.0), (float)Math.IEEERemainder((double)(vector.z * val.z) * num, 1.0), (float)Math.IEEERemainder((double)(vector.w * val.w) * num, 1.0));
					sharedMaterial.SetVector("_WaveOffset", val2);
					sharedMaterial.SetVector("_WaveScale4", val);
				}
			}
		}

		private void UpdateCameraModes(Camera src, Camera dest)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			if (!(dest == null))
			{
				dest.set_clearFlags(src.get_clearFlags());
				dest.set_backgroundColor(src.get_backgroundColor());
				if ((int)src.get_clearFlags() == 1)
				{
					Skybox component = src.GetComponent<Skybox>();
					Skybox component2 = dest.GetComponent<Skybox>();
					if (!Object.op_Implicit(component) || !Object.op_Implicit(component.get_material()))
					{
						component2.set_enabled(false);
					}
					else
					{
						component2.set_enabled(true);
						component2.set_material(component.get_material());
					}
				}
				dest.set_farClipPlane(src.get_farClipPlane());
				dest.set_nearClipPlane(src.get_nearClipPlane());
				dest.set_orthographic(src.get_orthographic());
				dest.set_fieldOfView(src.get_fieldOfView());
				dest.set_aspect(src.get_aspect());
				dest.set_orthographicSize(src.get_orthographicSize());
			}
		}

		private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Expected O, but got Unknown
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Expected O, but got Unknown
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			WaterMode waterMode = GetWaterMode();
			reflectionCamera = null;
			refractionCamera = null;
			if (waterMode >= WaterMode.Reflective)
			{
				if (!Object.op_Implicit(m_ReflectionTexture) || m_OldReflectionTextureSize != textureSize)
				{
					if (Object.op_Implicit(m_ReflectionTexture))
					{
						Object.DestroyImmediate(m_ReflectionTexture);
					}
					m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16);
					m_ReflectionTexture.set_name("__WaterReflection" + this.GetInstanceID());
					m_ReflectionTexture.set_isPowerOfTwo(true);
					m_ReflectionTexture.set_hideFlags(52);
					m_OldReflectionTextureSize = textureSize;
				}
				m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
				if (!Object.op_Implicit(reflectionCamera))
				{
					GameObject val = new GameObject("Water Refl Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(), new Type[2]
					{
						typeof(Camera),
						typeof(Skybox)
					});
					reflectionCamera = val.GetComponent<Camera>();
					reflectionCamera.set_enabled(false);
					reflectionCamera.get_transform().set_position(this.get_transform().get_position());
					reflectionCamera.get_transform().set_rotation(this.get_transform().get_rotation());
					reflectionCamera.get_gameObject().AddComponent<FlareLayer>();
					val.set_hideFlags(61);
					m_ReflectionCameras[currentCamera] = reflectionCamera;
					if (TurnOffWaterOcclusion)
					{
						reflectionCamera.set_useOcclusionCulling(false);
					}
				}
			}
			if (waterMode >= WaterMode.Refractive)
			{
				if (!Object.op_Implicit(m_RefractionTexture) || m_OldRefractionTextureSize != textureSize)
				{
					if (Object.op_Implicit(m_RefractionTexture))
					{
						Object.DestroyImmediate(m_RefractionTexture);
					}
					m_RefractionTexture = new RenderTexture(textureSize, textureSize, 16);
					m_RefractionTexture.set_name("__WaterRefraction" + this.GetInstanceID());
					m_RefractionTexture.set_isPowerOfTwo(true);
					m_RefractionTexture.set_hideFlags(52);
					m_OldRefractionTextureSize = textureSize;
				}
				m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
				if (!Object.op_Implicit(refractionCamera))
				{
					GameObject val2 = new GameObject("Water Refr Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(), new Type[2]
					{
						typeof(Camera),
						typeof(Skybox)
					});
					refractionCamera = val2.GetComponent<Camera>();
					refractionCamera.set_enabled(false);
					refractionCamera.get_transform().set_position(this.get_transform().get_position());
					refractionCamera.get_transform().set_rotation(this.get_transform().get_rotation());
					refractionCamera.get_gameObject().AddComponent<FlareLayer>();
					val2.set_hideFlags(61);
					m_RefractionCameras[currentCamera] = refractionCamera;
					if (TurnOffWaterOcclusion)
					{
						reflectionCamera.set_useOcclusionCulling(false);
					}
				}
			}
		}

		private WaterMode GetWaterMode()
		{
			if (m_HardwareWaterSupport < waterMode)
			{
				return m_HardwareWaterSupport;
			}
			return waterMode;
		}

		private WaterMode FindHardwareWaterSupport()
		{
			if (!SystemInfo.get_supportsRenderTextures() || !Object.op_Implicit(this.GetComponent<Renderer>()))
			{
				return WaterMode.Simple;
			}
			Material sharedMaterial = this.GetComponent<Renderer>().get_sharedMaterial();
			if (!Object.op_Implicit(sharedMaterial))
			{
				return WaterMode.Simple;
			}
			string tag = sharedMaterial.GetTag("WATERMODE", false);
			if (tag == "Refractive")
			{
				return WaterMode.Refractive;
			}
			if (tag == "Reflective")
			{
				return WaterMode.Reflective;
			}
			return WaterMode.Simple;
		}

		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = pos + normal * clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.get_worldToCameraMatrix();
			Vector3 val2 = worldToCameraMatrix.MultiplyPoint(val);
			Vector3 val3 = worldToCameraMatrix.MultiplyVector(normal);
			Vector3 val4 = val3.get_normalized() * sideSign;
			return new Vector4(val4.x, val4.y, val4.z, 0f - Vector3.Dot(val2, val4));
		}

		private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane.get_Item(0) * plane.get_Item(0);
			reflectionMat.m01 = -2f * plane.get_Item(0) * plane.get_Item(1);
			reflectionMat.m02 = -2f * plane.get_Item(0) * plane.get_Item(2);
			reflectionMat.m03 = -2f * plane.get_Item(3) * plane.get_Item(0);
			reflectionMat.m10 = -2f * plane.get_Item(1) * plane.get_Item(0);
			reflectionMat.m11 = 1f - 2f * plane.get_Item(1) * plane.get_Item(1);
			reflectionMat.m12 = -2f * plane.get_Item(1) * plane.get_Item(2);
			reflectionMat.m13 = -2f * plane.get_Item(3) * plane.get_Item(1);
			reflectionMat.m20 = -2f * plane.get_Item(2) * plane.get_Item(0);
			reflectionMat.m21 = -2f * plane.get_Item(2) * plane.get_Item(1);
			reflectionMat.m22 = 1f - 2f * plane.get_Item(2) * plane.get_Item(2);
			reflectionMat.m23 = -2f * plane.get_Item(3) * plane.get_Item(2);
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
		}
	}
}
