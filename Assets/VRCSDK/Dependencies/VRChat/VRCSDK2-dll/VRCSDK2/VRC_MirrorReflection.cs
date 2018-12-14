using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRCSDK2
{
	[ExecuteInEditMode]
	public class VRC_MirrorReflection : MonoBehaviour
	{
		private class ReflectionData
		{
			public RenderTexture[] texture = (RenderTexture[])new RenderTexture[2];

			public MaterialPropertyBlock propertyBlock;
		}

		private const int MaxResolutionWidth = 2048;

		private const int MaxResolutionHeight = 2048;

		public bool m_DisablePixelLights = true;

		[Tooltip("Mirror will use up to the anti-aliasing level specified in QualitySettings, without exceeding this value (1, 2, 4, or 8)")]
		public int m_MaxAntiAliasing = 1;

		public bool TurnOffMirrorOcclusion = true;

		public LayerMask m_ReflectLayers = LayerMask.op_Implicit(-1);

		private Dictionary<Camera, ReflectionData> m_Reflections = new Dictionary<Camera, ReflectionData>();

		private Camera mirrorCamera;

		private Skybox mirrorSkybox;

		private Matrix4x4 parentTransform;

		private Quaternion parentRotation;

		private int _playerLocalLayer = 10;

		private static bool s_InsideRendering = false;

		private static int[] TexturePropertyID = new int[2];

		public VRC_MirrorReflection()
			: this()
		{
		}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)


		private void OnValidate()
		{
			s_InsideRendering = false;
			if (m_MaxAntiAliasing != 1 && m_MaxAntiAliasing != 2 && m_MaxAntiAliasing != 4 && m_MaxAntiAliasing != 8)
			{
				m_MaxAntiAliasing = 1;
			}
		}

		private void Start()
		{
			Renderer component = this.GetComponent<Renderer>();
			Material sharedMaterial = component.get_sharedMaterial();
			sharedMaterial.set_shader(Shader.Find("FX/MirrorReflection"));
			TexturePropertyID[0] = Shader.PropertyToID("_ReflectionTex0");
			TexturePropertyID[1] = Shader.PropertyToID("_ReflectionTex1");
			_playerLocalLayer = LayerMask.NameToLayer("PlayerLocal");
		}

		public void OnWillRenderObject()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			Renderer component = this.GetComponent<Renderer>();
			if (this.get_enabled() && Object.op_Implicit(component) && component.get_enabled())
			{
				Camera current = Camera.get_current();
				if (Object.op_Implicit(current) && !(current == mirrorCamera) && !s_InsideRendering)
				{
					s_InsideRendering = true;
					ReflectionData reflectionData = GetReflectionData(current);
					int pixelLightCount = QualitySettings.get_pixelLightCount();
					if (m_DisablePixelLights)
					{
						QualitySettings.set_pixelLightCount(0);
					}
					UpdateCameraModes(current);
					UpdateParentTransform(current);
					if (current.get_stereoEnabled())
					{
						if (ShouldRenderLeftEye(current))
						{
							Vector3 worldEyePos = GetWorldEyePos(current, 0);
							Quaternion worldEyeRot = GetWorldEyeRot(current, 0);
							Matrix4x4 eyeProjectionMatrix = GetEyeProjectionMatrix(current, 0);
							RenderMirror(reflectionData.texture[0], worldEyePos, worldEyeRot, eyeProjectionMatrix);
						}
						if (ShouldRenderRightEye(current))
						{
							Vector3 worldEyePos2 = GetWorldEyePos(current, 1);
							Quaternion worldEyeRot2 = GetWorldEyeRot(current, 1);
							Matrix4x4 eyeProjectionMatrix2 = GetEyeProjectionMatrix(current, 1);
							RenderMirror(reflectionData.texture[1], worldEyePos2, worldEyeRot2, eyeProjectionMatrix2);
						}
					}
					else if (ShouldRenderMonoscopic(current))
					{
						RenderMirror(reflectionData.texture[0], current.get_transform().get_position(), current.get_transform().get_rotation(), current.get_projectionMatrix());
					}
					component.SetPropertyBlock(reflectionData.propertyBlock);
					if (m_DisablePixelLights)
					{
						QualitySettings.set_pixelLightCount(pixelLightCount);
					}
					s_InsideRendering = false;
				}
			}
		}

		private bool ShouldRenderLeftEye(Camera cam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (int)cam.get_stereoTargetEye() == 3 || (int)cam.get_stereoTargetEye() == 1;
			if (flag && Vector3.Dot(GetWorldEyePos(cam, 0) - this.get_transform().get_position(), GetNormalDirection()) <= 0f)
			{
				flag = false;
			}
			return flag;
		}

		private bool ShouldRenderRightEye(Camera cam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (int)cam.get_stereoTargetEye() == 3 || (int)cam.get_stereoTargetEye() == 2;
			if (flag && Vector3.Dot(GetWorldEyePos(cam, 1) - this.get_transform().get_position(), GetNormalDirection()) <= 0f)
			{
				flag = false;
			}
			return flag;
		}

		private bool ShouldRenderMonoscopic(Camera cam)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Dot(cam.get_transform().get_position() - this.get_transform().get_position(), GetNormalDirection()) > 0f;
		}

		private Vector3 GetWorldEyePos(Camera cam, XRNode eye)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localPosition = InputTracking.GetLocalPosition(eye);
			return parentTransform.MultiplyPoint3x4(localPosition);
		}

		private Quaternion GetWorldEyeRot(Camera cam, XRNode eye)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Quaternion localRotation = InputTracking.GetLocalRotation(eye);
			return parentRotation * localRotation;
		}

		private Matrix4x4 GetEyeProjectionMatrix(Camera cam, XRNode eye)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Invalid comparison between Unknown and I4
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return cam.GetStereoProjectionMatrix(((int)eye == 1) ? 1 : 0);
		}

		private Vector3 GetNormalDirection()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return -this.get_transform().get_forward();
		}

		private void RenderMirror(RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			mirrorCamera.ResetWorldToCameraMatrix();
			mirrorCamera.get_transform().set_position(camPosition);
			mirrorCamera.get_transform().set_rotation(camRotation);
			mirrorCamera.set_projectionMatrix(camProjectionMatrix);
			int num = -17;
			num &= ~(1 << (_playerLocalLayer & 0x1F));
			mirrorCamera.set_cullingMask(num & m_ReflectLayers.get_value());
			mirrorCamera.set_targetTexture(targetTexture);
			Vector3 position = this.get_transform().get_position();
			Vector3 normalDirection = GetNormalDirection();
			Vector4 plane = Plane(position, normalDirection);
			Camera obj = mirrorCamera;
			obj.set_worldToCameraMatrix(obj.get_worldToCameraMatrix() * CalculateReflectionMatrix(plane));
			Vector4 val = CameraSpacePlane(mirrorCamera, position, normalDirection);
			mirrorCamera.set_projectionMatrix(mirrorCamera.CalculateObliqueMatrix(val));
			mirrorCamera.get_transform().set_position(GetPosition(mirrorCamera.get_cameraToWorldMatrix()));
			mirrorCamera.get_transform().set_rotation(GetRotation(mirrorCamera.get_cameraToWorldMatrix()));
			bool invertCulling = GL.get_invertCulling();
			GL.set_invertCulling(!invertCulling);
			mirrorCamera.Render();
			GL.set_invertCulling(invertCulling);
		}

		private void OnDisable()
		{
			if (Object.op_Implicit(mirrorCamera))
			{
				Object.DestroyImmediate(mirrorCamera.get_gameObject());
				mirrorCamera = null;
				mirrorSkybox = null;
			}
			foreach (ReflectionData value in m_Reflections.Values)
			{
				Object.DestroyImmediate(value.texture[0]);
				Object.DestroyImmediate(value.texture[1]);
			}
			m_Reflections.Clear();
		}

		private void UpdateCameraModes(Camera src)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Invalid comparison between Unknown and I4
			if (!Object.op_Implicit(mirrorCamera))
			{
				GameObject val = new GameObject("MirrorCam" + this.get_gameObject().get_name(), new Type[4]
				{
					typeof(Camera),
					typeof(VRC_MirrorCamera),
					typeof(Skybox),
					typeof(FlareLayer)
				});
				val.set_hideFlags(61);
				mirrorSkybox = val.GetComponent<Skybox>();
				mirrorCamera = val.GetComponent<Camera>();
				mirrorCamera.set_enabled(false);
			}
			mirrorCamera.set_clearFlags(src.get_clearFlags());
			mirrorCamera.set_backgroundColor(src.get_backgroundColor());
			if ((int)src.get_clearFlags() == 1)
			{
				Skybox val2 = src.GetComponent(typeof(Skybox)) as Skybox;
				if (!Object.op_Implicit(val2) || !Object.op_Implicit(val2.get_material()))
				{
					mirrorSkybox.set_enabled(false);
				}
				else
				{
					mirrorSkybox.set_enabled(true);
					mirrorSkybox.set_material(val2.get_material());
				}
			}
			mirrorCamera.set_farClipPlane(src.get_farClipPlane());
			mirrorCamera.set_nearClipPlane(src.get_nearClipPlane());
			mirrorCamera.set_orthographic(src.get_orthographic());
			mirrorCamera.set_fieldOfView(src.get_fieldOfView());
			mirrorCamera.set_aspect(src.get_aspect());
			mirrorCamera.set_orthographicSize(src.get_orthographicSize());
			mirrorCamera.set_useOcclusionCulling(!TurnOffMirrorOcclusion);
		}

		private void UpdateParentTransform(Camera cam)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if (cam.get_transform().get_parent() != null)
			{
				parentTransform = cam.get_transform().get_parent().get_localToWorldMatrix();
				parentRotation = cam.get_transform().get_parent().get_rotation();
			}
			else
			{
				Quaternion localRotation = InputTracking.GetLocalRotation(3);
				Matrix4x4 val = Matrix4x4.TRS(InputTracking.GetLocalPosition(3), localRotation, Vector3.get_one());
				parentTransform = cam.get_transform().get_localToWorldMatrix() * val.get_inverse();
				parentRotation = cam.get_transform().get_rotation() * Quaternion.Inverse(localRotation);
			}
		}

		private ReflectionData GetReflectionData(Camera currentCamera)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected O, but got Unknown
			ReflectionData value = null;
			if (m_Reflections == null)
			{
				m_Reflections = new Dictionary<Camera, ReflectionData>();
			}
			if (!m_Reflections.TryGetValue(currentCamera, out value))
			{
				value = new ReflectionData();
				value.propertyBlock = new MaterialPropertyBlock();
				m_Reflections[currentCamera] = value;
			}
			int num = Mathf.Min(currentCamera.get_pixelWidth(), 2048);
			int num2 = Mathf.Min(currentCamera.get_pixelHeight(), 2048);
			int num3 = Mathf.Min(QualitySettings.get_antiAliasing(), m_MaxAntiAliasing);
			num3 = Mathf.Max(1, num3);
			for (int i = 0; i < 2 && (i <= 0 || currentCamera.get_stereoEnabled()); i++)
			{
				if (!Object.op_Implicit(value.texture[i]) || value.texture[i].get_width() != num || value.texture[i].get_height() != num2 || value.texture[i].get_antiAliasing() != num3)
				{
					if (Object.op_Implicit(value.texture[i]))
					{
						Object.DestroyImmediate(value.texture[i]);
					}
					value.texture[i] = new RenderTexture(num, num2, 24);
					value.texture[i].set_antiAliasing(num3);
					value.texture[i].set_hideFlags(52);
					value.propertyBlock.SetTexture(TexturePropertyID[i], value.texture[i]);
				}
			}
			return value;
		}

		private static Vector4 Plane(Vector3 pos, Vector3 normal)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			return new Vector4(normal.x, normal.y, normal.z, 0f - Vector3.Dot(pos, normal));
		}

		private static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 worldToCameraMatrix = cam.get_worldToCameraMatrix();
			Vector3 pos2 = worldToCameraMatrix.MultiplyPoint(pos);
			Vector3 val = worldToCameraMatrix.MultiplyVector(normal);
			Vector3 normalized = val.get_normalized();
			return Plane(pos2, normalized);
		}

		private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 identity = Matrix4x4.get_identity();
			identity.m00 = 1f - 2f * plane.get_Item(0) * plane.get_Item(0);
			identity.m01 = -2f * plane.get_Item(0) * plane.get_Item(1);
			identity.m02 = -2f * plane.get_Item(0) * plane.get_Item(2);
			identity.m03 = -2f * plane.get_Item(3) * plane.get_Item(0);
			identity.m10 = -2f * plane.get_Item(1) * plane.get_Item(0);
			identity.m11 = 1f - 2f * plane.get_Item(1) * plane.get_Item(1);
			identity.m12 = -2f * plane.get_Item(1) * plane.get_Item(2);
			identity.m13 = -2f * plane.get_Item(3) * plane.get_Item(1);
			identity.m20 = -2f * plane.get_Item(2) * plane.get_Item(0);
			identity.m21 = -2f * plane.get_Item(2) * plane.get_Item(1);
			identity.m22 = 1f - 2f * plane.get_Item(2) * plane.get_Item(2);
			identity.m23 = -2f * plane.get_Item(3) * plane.get_Item(2);
			identity.m30 = 0f;
			identity.m31 = 0f;
			identity.m32 = 0f;
			identity.m33 = 1f;
			return identity;
		}

		private static float _copysign(float sizeval, float signval)
		{
			return (Mathf.Sign(signval) != 1f) ? (0f - Mathf.Abs(sizeval)) : Mathf.Abs(sizeval);
		}

		private static Quaternion GetRotation(Matrix4x4 matrix)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
			result.x = _copysign(result.x, matrix.m21 - matrix.m12);
			result.y = _copysign(result.y, matrix.m02 - matrix.m20);
			result.z = _copysign(result.z, matrix.m10 - matrix.m01);
			return result;
		}

		private static Vector3 GetPosition(Matrix4x4 matrix)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}
	}
}
