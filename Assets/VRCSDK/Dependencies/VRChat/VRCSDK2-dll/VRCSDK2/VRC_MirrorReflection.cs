using System;
using System.Collections;
using UnityEngine;

namespace VRCSDK2
{
	[ExecuteInEditMode]
	public class VRC_MirrorReflection : MonoBehaviour
	{
		public bool EnableInEditor;

		public bool m_DisablePixelLights = true;

		public int m_TextureSize = 256;

		public float m_ClipPlaneOffset = 0.07f;

		public bool TurnOffMirrorOcclusion = true;

		public LayerMask m_ReflectLayers = LayerMask.op_Implicit(-1);

		private Hashtable m_ReflectionCameras = new Hashtable();

		private RenderTexture m_ReflectionTexture;

		private int m_OldReflectionTextureSize;

		private int _playerLocalLayer = 10;

		private int _mirrorLayer = 18;

		private static bool s_InsideRendering;

		public VRC_MirrorReflection()
			: this()
		{
		}//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)


		private void Awake()
		{
			_playerLocalLayer = LayerMask.NameToLayer("PlayerLocal");
			_mirrorLayer = LayerMask.NameToLayer("MirrorReflection");
		}

		public void OnWillRenderObject()
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			if (this.get_enabled() && Object.op_Implicit(this.GetComponent<Renderer>()) && Object.op_Implicit(this.GetComponent<Renderer>().get_sharedMaterial()) && this.GetComponent<Renderer>().get_enabled() && (EnableInEditor || !Application.get_isEditor() || Application.get_isPlaying()))
			{
				Camera current = Camera.get_current();
				if (Object.op_Implicit(current) && !s_InsideRendering)
				{
					s_InsideRendering = true;
					CreateMirrorObjects(current, out Camera reflectionCamera);
					Vector3 position = this.get_transform().get_position();
					Vector3 val = -this.get_transform().get_forward();
					int pixelLightCount = QualitySettings.get_pixelLightCount();
					if (m_DisablePixelLights)
					{
						QualitySettings.set_pixelLightCount(0);
					}
					UpdateCameraModes(current, reflectionCamera);
					float num = 0f - Vector3.Dot(val, position) - m_ClipPlaneOffset;
					Vector4 plane = default(Vector4);
					plane._002Ector(val.x, val.y, val.z, num);
					Matrix4x4 reflectionMat = Matrix4x4.get_zero();
					CalculateReflectionMatrix(ref reflectionMat, plane);
					Vector3 position2 = current.get_transform().get_position();
					Vector3 position3 = reflectionMat.MultiplyPoint(position2);
					reflectionCamera.set_worldToCameraMatrix(current.get_worldToCameraMatrix() * reflectionMat);
					Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, val, 1f);
					Matrix4x4 projection = current.get_projectionMatrix();
					CalculateObliqueMatrix(ref projection, clipPlane);
					reflectionCamera.set_projectionMatrix(projection);
					int num2 = -17;
					num2 &= ~(1 << _playerLocalLayer);
					reflectionCamera.set_cullingMask(num2 & m_ReflectLayers.get_value());
					reflectionCamera.set_targetTexture(m_ReflectionTexture);
					GL.SetRevertBackfacing(true);
					reflectionCamera.get_transform().set_position(position3);
					Vector3 eulerAngles = current.get_transform().get_eulerAngles();
					reflectionCamera.get_transform().set_eulerAngles(new Vector3(0f, eulerAngles.y, eulerAngles.z));
					reflectionCamera.Render();
					reflectionCamera.get_transform().set_position(position2);
					GL.SetRevertBackfacing(false);
					Material[] sharedMaterials = this.GetComponent<Renderer>().get_sharedMaterials();
					Material[] array = sharedMaterials;
					foreach (Material val2 in array)
					{
						if (val2.HasProperty("_ReflectionTex"))
						{
							val2.SetTexture("_ReflectionTex", m_ReflectionTexture);
						}
					}
					Matrix4x4 val3 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.get_identity(), new Vector3(0.5f, 0.5f, 0.5f));
					Vector3 lossyScale = this.get_transform().get_lossyScale();
					Matrix4x4 val4 = this.get_transform().get_localToWorldMatrix() * Matrix4x4.Scale(new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z));
					val4 = val3 * current.get_projectionMatrix() * current.get_worldToCameraMatrix() * val4;
					Material[] array2 = sharedMaterials;
					foreach (Material val5 in array2)
					{
						val5.SetMatrix("_ProjMatrix", val4);
					}
					if (m_DisablePixelLights)
					{
						QualitySettings.set_pixelLightCount(pixelLightCount);
					}
					s_InsideRendering = false;
				}
			}
		}

		private void OnDisable()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit(m_ReflectionTexture))
			{
				Object.DestroyImmediate(m_ReflectionTexture);
				m_ReflectionTexture = null;
			}
			IDictionaryEnumerator enumerator = m_ReflectionCameras.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object.DestroyImmediate(((DictionaryEntry)enumerator.Current).Value.get_gameObject());
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
			m_ReflectionCameras.Clear();
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
					Skybox val = src.GetComponent(typeof(Skybox)) as Skybox;
					Skybox val2 = dest.GetComponent(typeof(Skybox)) as Skybox;
					if (!Object.op_Implicit(val) || !Object.op_Implicit(val.get_material()))
					{
						val2.set_enabled(false);
					}
					else
					{
						val2.set_enabled(true);
						val2.set_material(val.get_material());
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

		private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected O, but got Unknown
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			reflectionCamera = null;
			if (!Object.op_Implicit(m_ReflectionTexture) || m_OldReflectionTextureSize != m_TextureSize)
			{
				if (Object.op_Implicit(m_ReflectionTexture))
				{
					Object.DestroyImmediate(m_ReflectionTexture);
				}
				m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
				m_ReflectionTexture.set_name("__MirrorReflection" + this.GetInstanceID());
				m_ReflectionTexture.set_isPowerOfTwo(true);
				m_ReflectionTexture.set_hideFlags(52);
				m_OldReflectionTextureSize = m_TextureSize;
			}
			reflectionCamera = (m_ReflectionCameras[currentCamera] as Camera);
			if (!Object.op_Implicit(reflectionCamera))
			{
				GameObject val = new GameObject("Mirror Refl Camera id" + this.GetInstanceID() + " for " + currentCamera.GetInstanceID(), new Type[3]
				{
					typeof(Camera),
					typeof(VRC_MirrorCamera),
					typeof(Skybox)
				});
				reflectionCamera = val.GetComponent<Camera>();
				reflectionCamera.set_enabled(false);
				reflectionCamera.get_transform().set_position(this.get_transform().get_position());
				reflectionCamera.get_transform().set_rotation(this.get_transform().get_rotation());
				reflectionCamera.get_gameObject().AddComponent<FlareLayer>();
				reflectionCamera.set_renderingPath(1);
				val.set_hideFlags(61);
				m_ReflectionCameras[currentCamera] = reflectionCamera;
				if (TurnOffMirrorOcclusion)
				{
					reflectionCamera.set_useOcclusionCulling(false);
				}
			}
		}

		private static float sgn(float a)
		{
			if (a > 0f)
			{
				return 1f;
			}
			if (a < 0f)
			{
				return -1f;
			}
			return 0f;
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
			Vector3 val = pos + normal * m_ClipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.get_worldToCameraMatrix();
			Vector3 val2 = worldToCameraMatrix.MultiplyPoint(val);
			Vector3 val3 = worldToCameraMatrix.MultiplyVector(normal);
			Vector3 val4 = val3.get_normalized() * sideSign;
			return new Vector4(val4.x, val4.y, val4.z, 0f - Vector3.Dot(val2, val4));
		}

		private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			Vector4 val = projection.get_inverse() * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
			Vector4 val2 = clipPlane * (2f / Vector4.Dot(clipPlane, val));
			projection.set_Item(2, val2.x - projection.get_Item(3));
			projection.set_Item(6, val2.y - projection.get_Item(7));
			projection.set_Item(10, val2.z - projection.get_Item(11));
			projection.set_Item(14, val2.w - projection.get_Item(15));
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
