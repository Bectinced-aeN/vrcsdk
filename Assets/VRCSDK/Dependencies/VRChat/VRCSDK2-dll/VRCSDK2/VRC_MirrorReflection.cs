using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRCSDK2
{
	[ExecuteInEditMode]
	public class VRC_MirrorReflection : MonoBehaviour
	{
		private enum Dimension
		{
			Auto = 0,
			X256 = 0x100,
			X512 = 0x200,
			X1024 = 0x400
		}

		private class ReflectionData
		{
			public readonly RenderTexture[] texture = (RenderTexture[])new RenderTexture[2];

			public MaterialPropertyBlock propertyBlock;
		}

		private const int MAX_AUTO_VR_RESOLUTION_WIDTH = 2048;

		private const int MAX_AUTO_VR_RESOLUTION_HEIGHT = 2048;

		private const int MAX_AUTO_DESKTOP_RESOLUTION_WIDTH = 2048;

		private const int MAX_AUTO_DESKTOP_RESOLUTION_HEIGHT = 2048;

		[Tooltip("Disables real-time pixel shaded point and spot lighting. Pixel shaded lights will fall-back to vertex lighting when this is enabled.")]
		public bool m_DisablePixelLights = true;

		[Tooltip("Disables occlusion culling on the mirror. Enable this if you see objects flickering in the mirror.")]
		public bool TurnOffMirrorOcclusion = true;

		[Tooltip("Only objects on the selected layers will be rendered in the mirror. Objects on the Water layer are never rendered in mirrors.")]
		public LayerMask m_ReflectLayers = LayerMask.op_Implicit(-1);

		[Tooltip("Rendering resolution of the mirror (per eye in VR). Auto renders at the same resolution as the user's HMD or monitor up to the maximum.")]
		[SerializeField]
		private Dimension mirrorResolution;

		[Tooltip("The mirror will use this shader instead of the default shader if one is provided.")]
		[SerializeField]
		private Shader customShader;

		private Dictionary<Camera, ReflectionData> _mReflections = new Dictionary<Camera, ReflectionData>();

		private Camera mirrorCamera;

		private Skybox mirrorSkybox;

		private Matrix4x4 parentTransform;

		private Quaternion parentRotation;

		private int _playerLocalLayer = 10;

		private static bool _sInsideRendering = false;

		private static readonly int[] _texturePropertyId = new int[2];

		public VRC_MirrorReflection()
			: this()
		{
		}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)


		private void OnValidate()
		{
			_sInsideRendering = false;
			Renderer component = this.GetComponent<Renderer>();
			Material sharedMaterial = component.get_sharedMaterial();
			if (customShader != null)
			{
				sharedMaterial.set_shader(customShader);
			}
			else
			{
				sharedMaterial.set_shader(Shader.Find("FX/MirrorReflection"));
			}
		}

		private unsafe void Start()
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			Renderer component = this.GetComponent<Renderer>();
			Material sharedMaterial = component.get_sharedMaterial();
			if (customShader != null)
			{
				sharedMaterial.set_shader(customShader);
			}
			else
			{
				sharedMaterial.set_shader(Shader.Find("FX/MirrorReflection"));
			}
			_texturePropertyId[0] = Shader.PropertyToID("_ReflectionTex0");
			_texturePropertyId[1] = Shader.PropertyToID("_ReflectionTex1");
			_playerLocalLayer = LayerMask.NameToLayer("PlayerLocal");
			Camera.onPostRender = Delegate.Combine((Delegate)Camera.onPostRender, (Delegate)new CameraCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
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
				if (Object.op_Implicit(current) && !(current == mirrorCamera) && !_sInsideRendering)
				{
					_sInsideRendering = true;
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
					_sInsideRendering = false;
				}
			}
		}

		public void CameraPostRender(Camera cam)
		{
			if (_mReflections.ContainsKey(cam))
			{
				ReflectionData reflectionData = _mReflections[cam];
				if (reflectionData.texture[0] != null)
				{
					RenderTexture.ReleaseTemporary(reflectionData.texture[0]);
					reflectionData.texture[0] = null;
				}
				if (reflectionData.texture[1] != null)
				{
					RenderTexture.ReleaseTemporary(reflectionData.texture[1]);
					reflectionData.texture[1] = null;
				}
			}
		}

		private void OnDisable()
		{
			foreach (ReflectionData value in _mReflections.Values)
			{
				if (value.texture[0] != null)
				{
					RenderTexture.ReleaseTemporary(value.texture[0]);
					value.texture[0] = null;
				}
				if (value.texture[1] != null)
				{
					RenderTexture.ReleaseTemporary(value.texture[1]);
					value.texture[1] = null;
				}
			}
			_mReflections.Clear();
		}

		private void OnDestroy()
		{
			if (mirrorCamera != null)
			{
				if (!Application.get_isEditor())
				{
					Object.Destroy(mirrorCamera.get_gameObject());
				}
				else
				{
					Object.DestroyImmediate(mirrorCamera.get_gameObject());
				}
				mirrorCamera = null;
				mirrorSkybox = null;
			}
		}

		private bool ShouldRenderLeftEye(Camera cam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			StereoTargetEyeMask stereoTargetEye = cam.get_stereoTargetEye();
			bool flag = (int)stereoTargetEye == 3 || (int)stereoTargetEye == 1;
			if (!flag)
			{
				return false;
			}
			if (Vector3.Dot(GetWorldEyePos(cam, 0) - this.get_transform().get_position(), GetNormalDirection()) <= 0f)
			{
				flag = false;
			}
			return flag;
		}

		private bool ShouldRenderRightEye(Camera cam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			StereoTargetEyeMask stereoTargetEye = cam.get_stereoTargetEye();
			bool flag = (int)stereoTargetEye == 3 || (int)stereoTargetEye == 2;
			if (!flag)
			{
				return false;
			}
			if (Vector3.Dot(GetWorldEyePos(cam, 1) - this.get_transform().get_position(), GetNormalDirection()) <= 0f)
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

		private void UpdateCameraModes(Camera src)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Invalid comparison between Unknown and I4
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
				GameObject val2 = val;
				mirrorSkybox = val2.GetComponent<Skybox>();
				mirrorCamera = val2.GetComponent<Camera>();
				mirrorCamera.set_enabled(false);
			}
			mirrorCamera.set_clearFlags(src.get_clearFlags());
			mirrorCamera.set_backgroundColor(src.get_backgroundColor());
			if ((int)src.get_clearFlags() == 1)
			{
				Skybox val3 = src.GetComponent(typeof(Skybox)) as Skybox;
				if (!Object.op_Implicit(val3) || !Object.op_Implicit(val3.get_material()))
				{
					mirrorSkybox.set_enabled(false);
				}
				else
				{
					mirrorSkybox.set_enabled(true);
					mirrorSkybox.set_material(val3.get_material());
				}
			}
			mirrorCamera.set_farClipPlane(src.get_farClipPlane());
			mirrorCamera.set_nearClipPlane(src.get_nearClipPlane());
			mirrorCamera.set_orthographic(src.get_orthographic());
			mirrorCamera.set_fieldOfView(src.get_fieldOfView());
			mirrorCamera.set_aspect(src.get_aspect());
			mirrorCamera.set_orthographicSize(src.get_orthographicSize());
			mirrorCamera.set_useOcclusionCulling(!TurnOffMirrorOcclusion);
			mirrorCamera.set_allowMSAA(src.get_allowMSAA());
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
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			ReflectionData value = null;
			if (_mReflections == null)
			{
				_mReflections = new Dictionary<Camera, ReflectionData>();
			}
			if (!_mReflections.TryGetValue(currentCamera, out value))
			{
				ReflectionData reflectionData = new ReflectionData();
				reflectionData.propertyBlock = new MaterialPropertyBlock();
				value = reflectionData;
				_mReflections[currentCamera] = value;
			}
			int width;
			int height;
			if (mirrorResolution == Dimension.Auto)
			{
				GetAutoResolution(currentCamera, out width, out height);
			}
			else
			{
				width = (int)mirrorResolution;
				height = (int)mirrorResolution;
			}
			int antiAliasing = QualitySettings.get_antiAliasing();
			antiAliasing = Mathf.Max(1, antiAliasing);
			for (int i = 0; i < 2 && (i <= 0 || currentCamera.get_stereoEnabled()); i++)
			{
				if (Object.op_Implicit(value.texture[i]))
				{
					RenderTexture.ReleaseTemporary(value.texture[i]);
				}
				value.texture[i] = RenderTexture.GetTemporary(width, height, 24, 2, 0, antiAliasing);
				value.propertyBlock.SetTexture(_texturePropertyId[i], value.texture[i]);
			}
			return value;
		}

		private static void GetAutoResolution(Camera currentCamera, out int width, out int height)
		{
			if (currentCamera.get_stereoEnabled())
			{
				width = Mathf.Min(currentCamera.get_pixelWidth(), 2048);
				height = Mathf.Min(currentCamera.get_pixelHeight(), 2048);
			}
			else
			{
				width = Mathf.Min(currentCamera.get_pixelWidth(), 2048);
				height = Mathf.Min(currentCamera.get_pixelHeight(), 2048);
			}
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
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = default(Quaternion);
			Quaternion val2 = val;
			val2.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
			val2.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
			val2.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
			val2.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
			val = val2;
			val.x = _copysign(val.x, matrix.m21 - matrix.m12);
			val.y = _copysign(val.y, matrix.m02 - matrix.m20);
			val.z = _copysign(val.z, matrix.m10 - matrix.m01);
			return val;
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
