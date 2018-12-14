using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC;
using VRC.Core;
using VRCSDK2;

public class VRCSdkControlPanel : EditorWindow
{
	private static VRCSdkControlPanel window;

	public static Action _EnableSpatialization;

	private string username;

	private string password;

	private int errorCount;

	private int warningCount;

	private VRC_SceneDescriptor[] scenes;

	private VRC_AvatarDescriptor[] avatars;

	private Vector2 scrollPos;

	private bool showLayerHelp;

	private int numClients = 1;

	private bool futureProof;

	private string contentId = string.Empty;

	public VRCSdkControlPanel()
		: this()
	{
	}

	[MenuItem("VRChat SDK/Show Build Control Panel")]
	private static void Init()
	{
		window = (VRCSdkControlPanel)EditorWindow.GetWindow(typeof(VRCSdkControlPanel));
		window.get_titleContent().set_text("VRChat");
		window.Show();
	}

	private bool OnGUIUserInfo()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Invalid comparison between Unknown and I4
		if (!RemoteConfig.IsInitialized())
		{
			RemoteConfig.Init(true, (Action)null, (Action)null);
		}
		if (!APIUser.get_IsLoggedInWithCredentials() && APIUser.get_IsCached())
		{
			APIUser.CachedLogin((Action<APIUser>)null, (Action<string>)null, false);
		}
		GUILayout.Label("Account Info", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
		if (APIUser.get_IsLoggedInWithCredentials())
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Logged in as " + APIUser.get_CurrentUser().get_displayName());
			if (GUILayout.Button("Logout", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				APIUser.Logout();
				GUILayout.EndHorizontal();
				return false;
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Developer Status: " + APIUser.get_CurrentUser().get_developerType(), (GUILayoutOption[])new GUILayoutOption[0]);
			if ((int)APIUser.get_CurrentUser().get_developerType() == 2)
			{
				string str = (!(ApiModel.GetApiUrl() == "https://dev-api.vrchat.cloud/api/1/")) ? "Release" : "Dev";
				GUILayout.Label("API: " + str, (GUILayoutOption[])new GUILayoutOption[0]);
			}
			return true;
		}
		ApiModel.ResetApi();
		username = EditorGUILayout.TextField("Username", username, (GUILayoutOption[])new GUILayoutOption[0]);
		password = EditorGUILayout.PasswordField("Password", password, (GUILayoutOption[])new GUILayoutOption[0]);
		if (GUILayout.Button("Sign In", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			APIUser.Login(username, password, (Action<APIUser>)delegate
			{
				EditorUtility.ClearProgressBar();
			}, (Action<string>)delegate(string message)
			{
				Logger.Log("Error logging in - " + message, 0);
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayDialog("Error logging in", message, "Okay");
			}, true);
			EditorUtility.DisplayProgressBar("Logging in!", "Hang tight...", 0.5f);
		}
		if (GUILayout.Button("Sign up", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			Application.OpenURL("http://www.vrchat.net/auth/register");
		}
		return false;
	}

	private void OnGUIError(string output)
	{
		OnGUIError((GameObject[])new GameObject[0], output);
	}

	private void OnGUIError(GameObject subject, string output)
	{
		OnGUIError((GameObject[])new GameObject[1]
		{
			subject
		}, output);
	}

	private void OnGUIError(GameObject[] subject, string output)
	{
		errorCount++;
		EditorGUILayout.HelpBox(output, 3);
	}

	private void OnGUIWarning(string output)
	{
		OnGUIWarning((GameObject[])new GameObject[0], output);
	}

	private void OnGUIWarning(GameObject subject, string output)
	{
		OnGUIWarning((GameObject[])new GameObject[1]
		{
			subject
		}, output);
	}

	private void OnGUIWarning(GameObject[] subject, string output)
	{
		warningCount++;
		EditorGUILayout.HelpBox(output, 2);
	}

	private void OnGUIInformation(GameObject[] subject, string output)
	{
		EditorGUILayout.HelpBox(output, 1);
	}

	private void OnGUI()
	{
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		if (window == null)
		{
			window = (VRCSdkControlPanel)EditorWindow.GetWindow(typeof(VRCSdkControlPanel));
		}
		if (Application.get_isPlaying())
		{
			EditorGUILayout.LabelField("You cannot edit your VRChat data while the Unity Application is running", (GUILayoutOption[])new GUILayoutOption[0]);
		}
		else if (OnGUIUserInfo())
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("General", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
			futureProof = EditorGUILayout.ToggleLeft("Future Proof Publish (Requires extra upload)", futureProof, (GUILayoutOption[])new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Client Version Date", SDKClientUtilities.GetTestClientVersionDate(), (GUILayoutOption[])new GUILayoutOption[0]);
			EditorGUILayout.LabelField("SDK Version Date", SDKClientUtilities.GetSDKVersionDate(), (GUILayoutOption[])new GUILayoutOption[0]);
			if (!SDKClientUtilities.IsClientNewerThanSDK())
			{
				OnGUIWarning("Your SDK is newer than the VRChat client you're testing with. Some SDK features may not work as expected. You can change VRC clients in VRChat SDK/Settings.");
			}
			if (RemoteConfig.IsInitialized())
			{
				string @string = RemoteConfig.GetString("sdkUnityVersion");
				if (Application.get_unityVersion() != @string)
				{
					OnGUIWarning("You are not using the recommended Unity version for the VRChat SDK. Content built with this version may not work correctly. Please use Unity " + @string);
				}
			}
			errorCount = (warningCount = 0);
			scenes = Tools.FindSceneObjectsOfTypeAll<VRC_SceneDescriptor>();
			avatars = Tools.FindSceneObjectsOfTypeAll<VRC_AvatarDescriptor>();
			if (scenes.Length > 0 && avatars.Length > 0)
			{
				GameObject[] array = (GameObject[])new GameObject[avatars.Length];
				for (int i = 0; i < avatars.Length; i++)
				{
					array[i] = avatars[i].get_gameObject();
				}
				OnGUIError(array, "a unity scene containing a VRChat Scene Descriptor should not also contain avatars.");
			}
			else if (scenes.Length > 1)
			{
				GameObject[] array2 = (GameObject[])new GameObject[scenes.Length];
				for (int j = 0; j < scenes.Length; j++)
				{
					array2[j] = scenes[j].get_gameObject();
				}
				OnGUIError(array2, "a unity scene containing a VRChat Scene Descriptor should only contain one scene descriptor.");
			}
			else if (scenes.Length == 1)
			{
				GUILayout.Label("Scene Options", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, (GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.Space();
				try
				{
					OnGUIScene(scenes[0]);
				}
				catch (Exception)
				{
				}
				EditorGUILayout.EndScrollView();
			}
			else if (avatars.Length > 0)
			{
				GUILayout.Label("Avatar Options", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, (GUILayoutOption[])new GUILayoutOption[0]);
				VRC_AvatarDescriptor[] array3 = avatars;
				foreach (VRC_AvatarDescriptor avatar in array3)
				{
					EditorGUILayout.Space();
					OnGUIAvatar(avatar);
				}
				EditorGUILayout.EndScrollView();
			}
			else
			{
				OnGUIError("Please add a scene descriptor or avatar descriptor to your project.");
			}
			window.Repaint();
		}
	}

	private void CheckUploadChanges(VRC_SceneDescriptor scene)
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (EditorPrefs.HasKey("VRCSDK2_scene_changed") && EditorPrefs.GetBool("VRCSDK2_scene_changed"))
		{
			EditorPrefs.DeleteKey("VRCSDK2_scene_changed");
			if (EditorPrefs.HasKey("VRCSDK2_capacity"))
			{
				scene.capacity = EditorPrefs.GetInt("VRCSDK2_capacity");
				EditorPrefs.DeleteKey("VRCSDK2_capacity");
			}
			if (EditorPrefs.HasKey("VRCSDK2_content_sex"))
			{
				scene.contentSex = EditorPrefs.GetBool("VRCSDK2_content_sex");
				EditorPrefs.DeleteKey("VRCSDK2_content_sex");
			}
			if (EditorPrefs.HasKey("VRCSDK2_content_violence"))
			{
				scene.contentViolence = EditorPrefs.GetBool("VRCSDK2_content_violence");
				EditorPrefs.DeleteKey("VRCSDK2_content_violence");
			}
			if (EditorPrefs.HasKey("VRCSDK2_content_gore"))
			{
				scene.contentGore = EditorPrefs.GetBool("VRCSDK2_content_gore");
				EditorPrefs.DeleteKey("VRCSDK2_content_gore");
			}
			if (EditorPrefs.HasKey("VRCSDK2_content_other"))
			{
				scene.contentOther = EditorPrefs.GetBool("VRCSDK2_content_other");
				EditorPrefs.DeleteKey("VRCSDK2_content_other");
			}
			if (EditorPrefs.HasKey("VRCSDK2_release_public"))
			{
				scene.releasePublic = EditorPrefs.GetBool("VRCSDK2_release_public");
				EditorPrefs.DeleteKey("VRCSDK2_release_public");
			}
			EditorUtility.SetDirty(scene);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}

	private void OnGUIScene(VRC_SceneDescriptor scene)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Invalid comparison between Unknown and I4
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Expected O, but got Unknown
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		CheckUploadChanges(scene);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.InspectorTitlebar(true, scene.get_gameObject());
		errorCount = (warningCount = 0);
		if (APIUser.get_CurrentUser() != null && APIUser.get_CurrentUser().get_hasScriptingAccess() && !CustomDLLMaker.DoesScriptDirExist())
		{
			CustomDLLMaker.CreateDirectories();
		}
		Vector3 gravity = Physics.get_gravity();
		if (gravity.x != 0f || gravity.z != 0f)
		{
			OnGUIWarning("Gravity vector is not straight down. Though we support different gravity, player orientation is always 'upwards' so things don't always behave as you intend.");
		}
		if (gravity.y > 0f)
		{
			OnGUIWarning("Gravity vector is not straight down, inverted or zero gravity will make walking extremely difficult.");
		}
		if (gravity.y == 0f)
		{
			OnGUIWarning("Zero gravity will make walking extremely difficult, though we support different gravity, player orientation is always 'upwards' so this may not have the effect you're looking for.");
		}
		scene.useAssignedLayers = true;
		if (scene.useAssignedLayers)
		{
			if (!UpdateLayers.AreLayersSetup() && GUILayout.Button("Setup Layers", (GUILayoutOption[])new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Setup Layers for VRChat", "This adds all VRChat layers to your project and pushes any custom layers down the layer list. If you have custom layers assigned to gameObjects, you'll need to reassign them. Are you sure you want to continue?", "Do it!", "Don't do it"))
			{
				UpdateLayers.SetupEditorLayers();
			}
			if (!UpdateLayers.AreLayersSetup())
			{
				OnGUIWarning("Layers are not setup properly. Please press the button above.");
			}
			if (UpdateLayers.AreLayersSetup() && !UpdateLayers.IsCollisionLayerMatrixSetup() && GUILayout.Button("Setup Collision Layer Matrix", (GUILayoutOption[])new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Setup Collision Layer Matrix for VRChat", "This will setup the correct physics collisions in the PhysicsManager for VRChat layers. Are you sure you want to continue?", "Do it!", "Don't do it"))
			{
				UpdateLayers.SetupCollisionLayerMatrix();
			}
			if (UpdateLayers.AreLayersSetup() && !UpdateLayers.IsCollisionLayerMatrixSetup())
			{
				OnGUIWarning("Physics Collision Layer Matrix is not setup correctly. Please press the button above.");
			}
		}
		scene.autoSpatializeAudioSources = EditorGUILayout.ToggleLeft("Apply 3D spatialization to AudioSources automatically at runtime (override settings by adding an ONSPAudioSource component to game object)", scene.autoSpatializeAudioSources, (GUILayoutOption[])new GUILayoutOption[0]);
		if (GUILayout.Button("Enable 3D spatialization on all 3D AudioSources in scene now", (GUILayoutOption[])new GUILayoutOption[0]) && EditorUtility.DisplayDialog("Enable Spatialization", "This will add an ONSPAudioSource script to every 3D AudioSource in the current scene, and enable default settings for spatialization.  Are you sure you want to continue?", "Do it!", "Don't do it"))
		{
			if (_EnableSpatialization != null)
			{
				_EnableSpatialization();
			}
			else
			{
				Debug.LogError((object)"VrcSdkControlPanel: EnableSpatialization callback not found!");
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(scene);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		if (APIUser.get_CurrentUser() != null && APIUser.get_CurrentUser().get_hasScriptingAccess() && !CustomDLLMaker.DoesScriptDirExist())
		{
			CustomDLLMaker.CreateDirectories();
		}
		if (APIUser.get_CurrentUser() != null && !APIUser.get_CurrentUser().get_hasScriptingAccess() && CustomDLLMaker.DoesScriptDirExist())
		{
			OnGUIWarning("Your account does not have permissions to upload custom scripts. You can test locally but need to contact VRChat to publish your world with scripts.");
		}
		string lastUrl = VRCSdkBuilder.GetLastUrl();
		bool flag = lastUrl != null;
		EditorGUILayout.LabelField("Errors: " + errorCount + " Warnings: " + warningCount, (GUILayoutOption[])new GUILayoutOption[0]);
		string str = "-1";
		PipelineManager[] array = Tools.FindSceneObjectsOfTypeAll<PipelineManager>();
		if (array.Length == 1)
		{
			if (scene.apiWorld == null)
			{
				scene.apiWorld = ScriptableObject.CreateInstance<ApiWorld>();
				ApiWorld.Fetch(array[0].blueprintId, (Action<ApiWorld>)delegate(ApiWorld world)
				{
					scene.apiWorld = world;
				}, (Action<string>)delegate
				{
				});
			}
			str = (scene.apiWorld as ApiWorld).get_version().ToString();
		}
		EditorGUILayout.LabelField("World Version: " + str, (GUILayoutOption[])new GUILayoutOption[0]);
		GUI.set_enabled(errorCount == 0);
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		EditorGUILayout.LabelField("Test", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
		numClients = EditorGUILayout.IntField("Number of Clients", numClients, (GUILayoutOption[])new GUILayoutOption[0]);
		if (!flag)
		{
			GUI.set_enabled(false);
		}
		if (GUILayout.Button("Last Build", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			VRCSdkBuilder.shouldBuildUnityPackage = futureProof;
			VRCSdkBuilder.numClientsToLaunch = numClients;
			VRCSdkBuilder.RunLastExportedSceneResource();
		}
		if ((int)APIUser.get_CurrentUser().get_developerType() == 2 && GUILayout.Button("Copy Test URL", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			TextEditor val = new TextEditor();
			val.set_text(lastUrl);
			val.SelectAll();
			val.Copy();
		}
		if (!flag)
		{
			GUI.set_enabled(true);
		}
		if (GUILayout.Button("New Build", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			VRCSdkBuilder.shouldBuildUnityPackage = futureProof;
			VRCSdkBuilder.numClientsToLaunch = numClients;
			VRCSdkBuilder.ExportSceneResourceAndRun();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[0]);
		EditorGUILayout.LabelField("Publish", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
		if (!flag)
		{
			GUI.set_enabled(false);
		}
		if (GUILayout.Button("Last Build", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			VRCSdkBuilder.shouldBuildUnityPackage = futureProof;
			VRCSdkBuilder.UploadLastExportedSceneBlueprint();
		}
		if (!flag)
		{
			GUI.set_enabled(true);
		}
		if (GUILayout.Button("New Build", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			VRCSdkBuilder.shouldBuildUnityPackage = futureProof;
			VRCSdkBuilder.ExportAndUploadSceneBlueprint();
		}
		EditorGUILayout.EndVertical();
		GUI.set_enabled(true);
	}

	private void OnGUISceneLayer(int layer, string name, string description)
	{
		if (LayerMask.LayerToName(layer) != name)
		{
			OnGUIError("Layer " + layer + " must be renamed to '" + name + "'");
		}
		if (showLayerHelp)
		{
			EditorGUILayout.HelpBox("Layer " + layer + " " + name + "\n" + description, 0);
		}
	}

	private int CountPolygons(Renderer r)
	{
		int num = 0;
		SkinnedMeshRenderer val = r as SkinnedMeshRenderer;
		if (val != null)
		{
			if (val.get_sharedMesh() == null)
			{
				return 0;
			}
			for (int i = 0; i < val.get_sharedMesh().get_subMeshCount(); i++)
			{
				num += val.get_sharedMesh().GetTriangles(i).Length / 3;
			}
		}
		ParticleSystemRenderer val2 = r as ParticleSystemRenderer;
		if (val2 != null)
		{
			num += val2.GetComponent<ParticleSystem>().get_maxParticles();
		}
		MeshRenderer val3 = r as MeshRenderer;
		if (val3 != null)
		{
			MeshFilter component = val3.GetComponent<MeshFilter>();
			if (component == null || component.get_sharedMesh() == null)
			{
				return 0;
			}
			for (int j = 0; j < component.get_sharedMesh().get_subMeshCount(); j++)
			{
				num += component.get_sharedMesh().GetTriangles(j).Length / 3;
			}
		}
		return num;
	}

	private void AnalyzeGeometry(GameObject go, out Bounds bounds, out int polycount)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		polycount = 0;
		bounds._002Ector(go.get_transform().get_position(), Vector3.get_zero());
		List<Renderer> list = new List<Renderer>();
		LODGroup[] componentsInChildren = go.GetComponentsInChildren<LODGroup>();
		LODGroup[] array = componentsInChildren;
		foreach (LODGroup val in array)
		{
			LOD[] lODs = val.GetLODs();
			int num = 0;
			LOD[] array2 = lODs;
			for (int j = 0; j < array2.Length; j++)
			{
				LOD val2 = array2[j];
				int num2 = 0;
				Renderer[] renderers = val2.renderers;
				foreach (Renderer val3 in renderers)
				{
					list.Add(val3);
					num2 += CountPolygons(val3);
				}
				if (num2 > num)
				{
					num = num2;
				}
			}
			polycount += num;
		}
		Renderer[] componentsInChildren2 = go.GetComponentsInChildren<Renderer>();
		Renderer[] array3 = componentsInChildren2;
		foreach (Renderer val4 in array3)
		{
			if (val4 as ParticleSystemRenderer == null)
			{
				bounds.Encapsulate(val4.get_bounds());
			}
			if (!list.Contains(val4))
			{
				polycount += CountPolygons(val4);
			}
		}
		bounds.set_center(bounds.get_center() - go.get_transform().get_position());
	}

	private void OnGUIAvatar(VRC_AvatarDescriptor avatar)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Invalid comparison between Unknown and I4
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		EditorGUI.BeginChangeCheck();
		errorCount = (warningCount = 0);
		EditorGUILayout.InspectorTitlebar(true, avatar.get_gameObject());
		AnalyzeGeometry(avatar.get_gameObject(), out Bounds bounds, out int polycount);
		if (polycount < 10000)
		{
			OnGUIInformation((GameObject[])new GameObject[1]
			{
				avatar.get_gameObject()
			}, "Polygons: " + polycount);
		}
		else if (polycount < 15000)
		{
			OnGUIWarning(avatar.get_gameObject(), "Polygons: " + polycount + " - Please try to reduce your avatar poly count to less thatn 10k.");
		}
		else if (polycount < 20000)
		{
			OnGUIWarning(avatar.get_gameObject(), "Polygons: " + polycount + " - This avatar will not perform well on many systems.");
		}
		else
		{
			OnGUIError((GameObject[])new GameObject[1]
			{
				avatar.get_gameObject()
			}, "Polygons: " + polycount + " - This avatar has too many polygons. It must have less than 20k and should have less than 10k.");
		}
		Vector3 size = bounds.get_size();
		if (!(size.x > 5f))
		{
			Vector3 size2 = bounds.get_size();
			if (!(size2.y > 6f))
			{
				Vector3 size3 = bounds.get_size();
				if (!(size3.z > 5f))
				{
					goto IL_0169;
				}
			}
		}
		GameObject gameObject = avatar.get_gameObject();
		Vector3 size4 = bounds.get_size();
		OnGUIError(gameObject, "This avatar measures too large on at least one axis. It must be <5m on a side but it's bounds are " + size4.ToString());
		goto IL_0169;
		IL_0169:
		VRC_EventHandler componentInChildren = avatar.GetComponentInChildren<VRC_EventHandler>();
		if (componentInChildren != null)
		{
			OnGUIError(avatar.get_gameObject(), "This avatar contains an EventHandler, which is not currently supported in VRChat.");
		}
		Animator component = avatar.GetComponent<Animator>();
		if (component == null)
		{
			OnGUIWarning(avatar.get_gameObject(), "This avatar does not contain an animator, and will not animate in VRChat.");
		}
		else if (!component.get_isHuman())
		{
			OnGUIWarning(avatar.get_gameObject(), "This avatar is not imported as a humanoid rig and will not play VRChat's provided animation set.");
		}
		else if (!avatar.get_gameObject().get_activeInHierarchy())
		{
			OnGUIError(avatar.get_gameObject(), "Your avatar is disabled in the scene heirarchy!");
		}
		else
		{
			Transform boneTransform = component.GetBoneTransform(5);
			Transform boneTransform2 = component.GetBoneTransform(13);
			if (boneTransform == null)
			{
				OnGUIError(avatar.get_gameObject(), "Your avatar is humanoid, but it's feet aren't specified!");
			}
			if (boneTransform2 == null)
			{
				OnGUIError(avatar.get_gameObject(), "Your avatar is humanoid, but it's upper arms aren't specified!");
			}
			if (boneTransform != null && boneTransform2 != null)
			{
				Vector3 val = boneTransform.get_position() - avatar.get_transform().get_position();
				if (val.y < 0f)
				{
					OnGUIWarning(avatar.get_gameObject(), "Avatar feet are beneath the avatar's origin (the floor). That's probably not what you want.");
				}
				Vector3 val2 = boneTransform2.get_position() - avatar.get_transform().get_position();
				if (val2.y < 0.2f)
				{
					OnGUIError(avatar.get_gameObject(), "This avatar is too short. The minimum is 20cm shoulder height.");
				}
				else if (val2.y < 1f)
				{
					OnGUIWarning(avatar.get_gameObject(), "This avatar is short. This is probably shorter than you want.");
				}
				else if (val2.y > 5f)
				{
					OnGUIWarning(avatar.get_gameObject(), "This avatar is too tall. The maximum is 5m shoulder height.");
				}
				else if (val2.y > 2.5f)
				{
					OnGUIWarning(avatar.get_gameObject(), "This avatar is tall. This is probably taller than you want.");
				}
			}
		}
		EditorGUILayout.LabelField("Errors: " + errorCount + " Warnings: " + warningCount, (GUILayoutOption[])new GUILayoutOption[0]);
		GUI.set_enabled(errorCount == 0 || (int)APIUser.get_CurrentUser().get_developerType() == 2);
		EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
		if (GUILayout.Button("Build & Publish", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			VRCSdkBuilder.shouldBuildUnityPackage = futureProof;
			VRCSdkBuilder.ExportAndUploadAvatarBlueprint(avatar.get_gameObject());
		}
		EditorGUILayout.EndHorizontal();
		GUI.set_enabled(true);
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(avatar);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}
}
