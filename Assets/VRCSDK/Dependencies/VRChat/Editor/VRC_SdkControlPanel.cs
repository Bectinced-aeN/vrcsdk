using UnityEngine;
using UnityEditor;
using VRC.Core;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VRC_SdkControlPanel : EditorWindow
{
    static VRC_SdkControlPanel window;
	static bool forceNewFileCreation = false;

    public static System.Action _EnableSpatialization = null;   // assigned in AutoAddONSPAudioSourceComponents
    
    [MenuItem("VRChat SDK/Show Build Control Panel")]
    static void Init()
    {
        window = (VRC_SdkControlPanel)EditorWindow.GetWindow(typeof(VRC_SdkControlPanel));
        window.titleContent.text = "VRChat";
        window.Show();
    	forceNewFileCreation = UnityEditor.EditorPrefs.GetBool("forceNewFileCreation", false);
	}

    bool UseDevApi
    {
        get
        {
            return VRC.Core.ApiModel.GetApiUrl() == ApiModel.devApiUrl;
        }
    }

    int errorCount = 0;
    int warningCount = 0;

    void OnGUIError(string output)
    {
        OnGUIError(new GameObject[] { }, output);
    }
    void OnGUIError(GameObject subject, string output)
    {
        OnGUIError(new GameObject[] { subject }, output);
    }
    void OnGUIError(GameObject[] subject, string output)
    {
        ++errorCount;
        EditorGUILayout.HelpBox(output, MessageType.Error);
    }

    void OnGUIWarning(string output)
    {
        OnGUIWarning(new GameObject[] { }, output);
    }
    void OnGUIWarning(GameObject subject, string output)
    {
        OnGUIWarning(new GameObject[] { subject }, output);
    }
    void OnGUIWarning(GameObject[] subject, string output)
    {
        ++warningCount;
        EditorGUILayout.HelpBox(output, MessageType.Warning);
    }

    void OnGUIInformation(GameObject[] subject, string output)
    {
        EditorGUILayout.HelpBox(output, MessageType.Info);
    }

    VRCSDK2.VRC_SceneDescriptor[] scenes;
    VRCSDK2.VRC_AvatarDescriptor[] avatars;
    Vector2 scrollPos;

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (window == null)
            window = (VRC_SdkControlPanel)EditorWindow.GetWindow(typeof(VRC_SdkControlPanel));

        if (!VRC.AccountEditorWindow.OnShowStatus())
            return;

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("You cannot edit your VRChat data while the Unity Application is running");
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        if (VRC.Core.SDKClientUtilities.IsInternalSDK())
        {
            useFileApi = EditorGUILayout.ToggleLeft("Use New File API", useFileApi); 
        }
        else
        {
            useFileApi = true;
        }

        if (UnityEditor.EditorPrefs.GetBool("useFileApi", true) != useFileApi)
        {
            UnityEditor.EditorPrefs.SetBool("useFileApi", useFileApi);
        }

        if (useFileApi && EditorGUILayout.ToggleLeft("Force Complete File Upload", forceNewFileCreation))
        {
            if (!forceNewFileCreation)
            {
                bool yep = UnityEditor.EditorUtility.DisplayDialog("Are you sure?", "This will disable compression and cause your full file data to be uploaded during next publish, resulting in longer upload times.", "OK", "Cancel");
                if (yep)
                {
                    forceNewFileCreation = true;
                }
            }
        }
        else
        {
            forceNewFileCreation = false;
        }

        if (UnityEditor.EditorPrefs.GetBool("forceNewFileCreation", false) != forceNewFileCreation)
        {
            UnityEditor.EditorPrefs.SetBool("forceNewFileCreation", forceNewFileCreation);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Client Version Date", VRC.Core.SDKClientUtilities.GetTestClientVersionDate());
        EditorGUILayout.LabelField("SDK Version Date", VRC.Core.SDKClientUtilities.GetSDKVersionDate());
        if (!VRC.Core.SDKClientUtilities.IsClientNewerThanSDK())
        {
            OnGUIWarning("Your SDK is newer than the VRChat client you're testing with. Some SDK features may not work as expected. You can change VRC clients in VRChat SDK/Settings.");
        }

        if (VRC.Core.RemoteConfig.IsInitialized())
        {
            string sdkUnityVersion = VRC.Core.RemoteConfig.GetString("sdkUnityVersion");
            if (Application.unityVersion != sdkUnityVersion)
            {
                OnGUIWarning("You are not using the recommended Unity version for the VRChat SDK. Content built with this version may not work correctly. Please use Unity " + sdkUnityVersion);
            }
        }

        errorCount = warningCount = 0;

        scenes = (VRCSDK2.VRC_SceneDescriptor[])VRC.Tools.FindSceneObjectsOfTypeAll<VRCSDK2.VRC_SceneDescriptor>();
        avatars = (VRCSDK2.VRC_AvatarDescriptor[])VRC.Tools.FindSceneObjectsOfTypeAll<VRCSDK2.VRC_AvatarDescriptor>();

        if (scenes.Length > 0 && avatars.Length > 0)
        {
            GameObject[] gos = new GameObject[avatars.Length];
            for (int i = 0; i < avatars.Length; ++i)
                gos[i] = avatars[i].gameObject;
            OnGUIError(gos, "a unity scene containing a VRChat Scene Descriptor should not also contain avatars.");
        }
        else if (scenes.Length > 1)
        {
            GameObject[] gos = new GameObject[scenes.Length];
            for (int i = 0; i < scenes.Length; ++i)
                gos[i] = scenes[i].gameObject;
            OnGUIError(gos, "a unity scene containing a VRChat Scene Descriptor should only contain one scene descriptor.");
        }
        else if (scenes.Length == 1)
        {
            GUILayout.Label("Scene Options", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.Space();
            try
            {
                //TODO: Throws errors on scene export for some reason.
                OnGUIScene(scenes[0]);
            }
            catch (System.Exception)
            {
            }
            EditorGUILayout.EndScrollView();
        }
        else if (avatars.Length > 0)
        {
            GUILayout.Label("Avatar Options", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var av in avatars)
            {
                EditorGUILayout.Space();
                OnGUIAvatar(av);
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            OnGUIError("Please add a scene descriptor or avatar descriptor to your project.");
        }

        window.Repaint();
    }

    bool showLayerHelp = false;
    int numClients = 1;
    bool useFileApi = true;

    void CheckUploadChanges(VRCSDK2.VRC_SceneDescriptor scene)
    {
        if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_scene_changed") &&
            UnityEditor.EditorPrefs.GetBool("VRCSDK2_scene_changed"))
        {
            UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_scene_changed");

            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_capacity"))
            {
                scene.capacity = UnityEditor.EditorPrefs.GetInt("VRCSDK2_capacity");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_capacity");
            }
            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_content_sex"))
            {
                scene.contentSex = UnityEditor.EditorPrefs.GetBool("VRCSDK2_content_sex");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_content_sex");
            }
            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_content_violence"))
            {
                scene.contentViolence = UnityEditor.EditorPrefs.GetBool("VRCSDK2_content_violence");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_content_violence");
            }
            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_content_gore"))
            {
                scene.contentGore = UnityEditor.EditorPrefs.GetBool("VRCSDK2_content_gore");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_content_gore");
            }
            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_content_other"))
            {
                scene.contentOther = UnityEditor.EditorPrefs.GetBool("VRCSDK2_content_other");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_content_other");
            }
            if (UnityEditor.EditorPrefs.HasKey("VRCSDK2_release_public"))
            {
                scene.releasePublic = UnityEditor.EditorPrefs.GetBool("VRCSDK2_release_public");
                UnityEditor.EditorPrefs.DeleteKey("VRCSDK2_release_public");
            }

            EditorUtility.SetDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }

    bool ShouldShowLightmapWarning
    {
        get
        {
            const string GraphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
            SerializedObject graphicsManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsAssetPath)[0]);
            SerializedProperty lightmapStripping = graphicsManager.FindProperty("m_LightmapStripping");
            return lightmapStripping.enumValueIndex == 0;
        }
    }

    bool ShouldShowFogWarning
    {
        get
        {
            const string GraphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
            SerializedObject graphicsManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsAssetPath)[0]);
            SerializedProperty lightmapStripping = graphicsManager.FindProperty("m_FogStripping");
            return lightmapStripping.enumValueIndex == 0;
        }
    }

    void OnGUIScene(VRCSDK2.VRC_SceneDescriptor scene)
    {
        CheckUploadChanges(scene);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.InspectorTitlebar(true, scene.gameObject);

        errorCount = warningCount = 0;

        if (VRC.Core.APIUser.CurrentUser != null && VRC.Core.APIUser.CurrentUser.hasScriptingAccess && !CustomDLLMaker.DoesScriptDirExist())
        {
			CustomDLLMaker.CreateDirectories();
        }

        Vector3 g = Physics.gravity;
        if (g.x != 0.0f || g.z != 0.0f)
            OnGUIWarning("Gravity vector is not straight down. Though we support different gravity, player orientation is always 'upwards' so things don't always behave as you intend.");
        if (g.y > 0)
            OnGUIWarning("Gravity vector is not straight down, inverted or zero gravity will make walking extremely difficult.");
        if (g.y == 0)
            OnGUIWarning("Zero gravity will make walking extremely difficult, though we support different gravity, player orientation is always 'upwards' so this may not have the effect you're looking for.");

        scene.useAssignedLayers = true;
        if (scene.useAssignedLayers)
        {
            if (!UpdateLayers.AreLayersSetup() && GUILayout.Button("Setup Layers"))
            {
                bool doIt = EditorUtility.DisplayDialog("Setup Layers for VRChat", "This adds all VRChat layers to your project and pushes any custom layers down the layer list. If you have custom layers assigned to gameObjects, you'll need to reassign them. Are you sure you want to continue?", "Do it!", "Don't do it");
                if (doIt)
                {
                    UpdateLayers.SetupEditorLayers();
                }
            }

            if (!UpdateLayers.AreLayersSetup())
                OnGUIWarning("Layers are not setup properly. Please press the button above.");

            if (UpdateLayers.AreLayersSetup() && !UpdateLayers.IsCollisionLayerMatrixSetup() && GUILayout.Button("Setup Collision Layer Matrix"))
            {
                bool doIt = EditorUtility.DisplayDialog("Setup Collision Layer Matrix for VRChat", "This will setup the correct physics collisions in the PhysicsManager for VRChat layers. Are you sure you want to continue?", "Do it!", "Don't do it");
                if (doIt)
                {
                    UpdateLayers.SetupCollisionLayerMatrix();
                }
            }

            if (UpdateLayers.AreLayersSetup() && !UpdateLayers.IsCollisionLayerMatrixSetup())
                OnGUIWarning("Physics Collision Layer Matrix is not setup correctly. Please press the button above.");
        }

        scene.autoSpatializeAudioSources = EditorGUILayout.ToggleLeft("Apply 3D spatialization to AudioSources automatically at runtime (override settings by adding an ONSPAudioSource component to game object)", scene.autoSpatializeAudioSources);
        if (GUILayout.Button("Enable 3D spatialization on all 3D AudioSources in scene now"))
        {
            bool doIt = EditorUtility.DisplayDialog("Enable Spatialization", "This will add an ONSPAudioSource script to every 3D AudioSource in the current scene, and enable default settings for spatialization.  Are you sure you want to continue?", "Do it!", "Don't do it");
            if (doIt)
            {
				if (_EnableSpatialization != null)
					_EnableSpatialization();
				else
					Debug.LogError("VrcSdkControlPanel: EnableSpatialization callback not found!");
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        // auto create VRCScript dir for those with access
        if(VRC.Core.APIUser.CurrentUser != null && VRC.Core.APIUser.CurrentUser.hasScriptingAccess && !CustomDLLMaker.DoesScriptDirExist())
        {
			CustomDLLMaker.CreateDirectories();
        }

        // warn those without scripting access if they choose to script locally
		if(VRC.Core.APIUser.CurrentUser != null && !VRC.Core.APIUser.CurrentUser.hasScriptingAccess && CustomDLLMaker.DoesScriptDirExist())
        {
            OnGUIWarning("Your account does not have permissions to upload custom scripts. You can test locally but need to contact VRChat to publish your world with scripts.");
        }

        //if (ShouldShowFogWarning)
        //    OnGUIWarning("Automatic stripping is selected for Fog settings, and so Fog may not appear properly in VR Chat. Check your Graphics Settings.");

        //if (ShouldShowLightmapWarning)
        //    OnGUIWarning("Automatic stripping is selected for Lightmap settings, and so Lightmaps may not work properly in VR Chat. Check your Graphics Settings.");

        string lastUrl = VRC_SdkBuilder.GetLastUrl();
        bool lastBuildPresent = lastUrl != null;

        EditorGUILayout.LabelField("Errors: " + errorCount + " Warnings: " + warningCount);

        string worldVersion = "-1";
        PipelineManager[] pms = (PipelineManager[])VRC.Tools.FindSceneObjectsOfTypeAll<PipelineManager>();
        if (pms.Length == 1)
        {
            if (scene.apiWorld == null)
            {
                scene.apiWorld = ScriptableObject.CreateInstance<ApiWorld>();
                ApiWorld.Fetch(pms[0].blueprintId, false, delegate(ApiWorld world)
                {
                    scene.apiWorld = world;
                }, delegate (string error) { });
            }
            worldVersion = (scene.apiWorld as ApiWorld).version.ToString();
        }
        EditorGUILayout.LabelField("World Version: " + worldVersion);

        GUI.enabled = (errorCount == 0);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Test", EditorStyles.boldLabel);
        numClients = EditorGUILayout.IntField("Number of Clients", numClients);
        if (lastBuildPresent == false)
            GUI.enabled = false;
        if (GUILayout.Button("Last Build"))
        {
            VRC_SdkBuilder.shouldBuildUnityPackage = false;
            VRC_SdkBuilder.numClientsToLaunch = numClients;
            VRC_SdkBuilder.RunLastExportedSceneResource();
        }
        if (APIUser.CurrentUser.developerType.HasValue && APIUser.CurrentUser.developerType.Value == APIUser.DeveloperType.Internal)
        {
            if (GUILayout.Button("Copy Test URL"))
            {
                TextEditor te = new TextEditor();
                te.text = lastUrl;
                te.SelectAll();
                te.Copy();
            }
        }
        if (lastBuildPresent == false)
            GUI.enabled = true;
        if (GUILayout.Button("New Build"))
        {
            EnvConfig.ConfigurePlayerSettings();
            VRC_SdkBuilder.shouldBuildUnityPackage = false;
            VRC_SdkBuilder.numClientsToLaunch = numClients;
            VRC_SdkBuilder.PreBuildBehaviourPackaging();
            VRC_SdkBuilder.ExportSceneResourceAndRun();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Publish", EditorStyles.boldLabel);
        if (lastBuildPresent == false)
            GUI.enabled = false;
        if (GUILayout.Button("Last Build"))
        {
            VRC_SdkBuilder.shouldBuildUnityPackage = VRC.AccountEditorWindow.FutureProofPublishEnabled;
            VRC_SdkBuilder.UploadLastExportedSceneBlueprint();
        }
        if (lastBuildPresent == false)
            GUI.enabled = true;
        if (GUILayout.Button("New Build"))
        {
            EnvConfig.ConfigurePlayerSettings();
            VRC_SdkBuilder.shouldBuildUnityPackage = VRC.AccountEditorWindow.FutureProofPublishEnabled;
            VRC_SdkBuilder.PreBuildBehaviourPackaging();
            VRC_SdkBuilder.ExportAndUploadSceneBlueprint();
        }
        EditorGUILayout.EndVertical();
        GUI.enabled = true;
    }

    void OnGUISceneLayer(int layer, string name, string description)
    {
        if (LayerMask.LayerToName(layer) != name)
            OnGUIError("Layer " + layer + " must be renamed to '" + name + "'");

        if (showLayerHelp)
            EditorGUILayout.HelpBox("Layer " + layer + " " + name + "\n" + description, MessageType.None);
    }

    int CountPolygons(Renderer r)
    {
        int result = 0;
        SkinnedMeshRenderer smr = r as SkinnedMeshRenderer;
        if (smr != null)
        {
            if (smr.sharedMesh == null)
                return 0;

            for (int i = 0; i < smr.sharedMesh.subMeshCount; ++i)
                result += smr.sharedMesh.GetTriangles(i).Length / 3;
        }

        ParticleSystemRenderer pr = r as ParticleSystemRenderer;
        if (pr != null)
        {
            result += pr.GetComponent<ParticleSystem>().main.maxParticles;
        }

        MeshRenderer mr = r as MeshRenderer;
        if (mr != null)
        {
            var mf = mr.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                return 0;
            for (int i = 0; i < mf.sharedMesh.subMeshCount; ++i)
                result += mf.sharedMesh.GetTriangles(i).Length / 3;
        }

        return result;
    }

    void AnalyzeGeometry(GameObject go, out Bounds bounds, out int polycount)
    {
        polycount = 0;
        bounds = new Bounds(go.transform.position, Vector3.zero);
        List<Renderer> ignore = new List<Renderer>();

        var lods = go.GetComponentsInChildren<LODGroup>();
        foreach (var lod in lods)
        {
            LOD[] options = lod.GetLODs();

            int highestLodPolies = 0;
            foreach (LOD l in options)
            {
                int thisLodPolies = 0;
                foreach (Renderer r in l.renderers)
                {
                    ignore.Add(r);
                    thisLodPolies += CountPolygons(r);
                }
                if (thisLodPolies > highestLodPolies)
                    highestLodPolies = thisLodPolies;
            }

            polycount += highestLodPolies;
        }

        var renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if( (r as ParticleSystemRenderer) == null )
                bounds.Encapsulate(r.bounds);

            if (ignore.Contains(r) == false)
                polycount += CountPolygons(r);
        }

        bounds.center -= go.transform.position;
    }

    bool IsAncestor(Transform ancestor, Transform child)
    {
        bool found = false;
        Transform thisParent = child.parent;
        while (thisParent != null)
        {
            if (thisParent == ancestor) { found = true; break; }
            thisParent = thisParent.parent;
        }

        return found;
    }

    void AnalyzeIK(GameObject avObj, Animator anim)
    {
        bool hasHead = false;
        bool hasFeet = false;
        bool hasHands = false;
        bool hasThreeFingers = false;
        //bool hasToes = false;
        bool correctSpineHierarchy = false;
        bool correctArmHierarchy = false;
        bool correctLegHierarchy = false;

        Transform head = anim.GetBoneTransform(HumanBodyBones.Head);
        Transform lfoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        Transform rfoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        Transform lhand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        Transform rhand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        hasHead = null!=head;
        hasFeet = (null!=lfoot && null!=rfoot);
        hasHands = (null!=lhand && null!=rhand);

        if (!hasHead || !hasFeet || !hasHands)
        {
            OnGUIError(avObj, "Humanoid avatar must have head, hands and feet bones mapped.");
            return;
        }

        //Transform ltoe = anim.GetBoneTransform(HumanBodyBones.LeftToes);
        //Transform rtoe = anim.GetBoneTransform(HumanBodyBones.RightToes);
        
        Transform lthumb = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        Transform lindex = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        Transform lmiddle = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        Transform rthumb = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        Transform rindex = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        Transform rmiddle = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);

        hasThreeFingers = null!=lthumb && null!=lindex && null!=lmiddle && null!=rthumb && null!=rindex && null!=rmiddle;

        if (!hasThreeFingers)
        {
            OnGUIWarning(avObj, "Thumb, Index, and Middle finger bones are not mapped, Full-Body IK will be disabled.");
            return;
        }

        Transform neck = anim.GetBoneTransform(HumanBodyBones.Neck);
        Transform lclav = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform rclav = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform pelvis = anim.GetBoneTransform(HumanBodyBones.Hips);
        Transform torso = anim.GetBoneTransform(HumanBodyBones.Spine);
        Transform chest = anim.GetBoneTransform(HumanBodyBones.Chest);

        if (null==neck || null==lclav || null==rclav || null==pelvis || null==torso || null==chest)
        {
            OnGUIError(avObj, "Spine hierarchy missing elements, make sure that Pelvis, Spine, Chest, Neck and Shoulders are mapped.");
            return;
        }

        correctSpineHierarchy = lclav.parent == chest && rclav.parent == chest && neck.parent == chest;

        if (!correctSpineHierarchy)
        {
            OnGUIError(avObj, "Spine hierarchy incorrect, make sure that the parent of both Shoulders and the Neck is the Chest.");
            return;
        }

        Transform lshoulder = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform lelbow = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform rshoulder = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform relbow = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);

        correctArmHierarchy = lshoulder.GetChild(0) == lelbow && lelbow.GetChild(0) == lhand &&
                              rshoulder.GetChild(0) == relbow && relbow.GetChild(0) == rhand;

        if (!correctArmHierarchy)
        {
            OnGUIWarning(avObj, "LowerArm is not first child of UpperArm or Hand is not first child of LowerArm: you may have problems with Forearm rotations.");
        }

        Transform lhip = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform lknee = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Transform rhip = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform rknee = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);

        correctLegHierarchy = lhip.GetChild(0) == lknee && lknee.GetChild(0) == lfoot &&
                              rhip.GetChild(0) == rknee && rknee.GetChild(0) == rfoot;

        if (!correctLegHierarchy)
        {
            OnGUIWarning(avObj, "LowerLeg is not first child of UpperLeg or Foot is not first child of LowerLeg: you may have problems with Shin rotations.");
        }

        if ( !(IsAncestor(pelvis, rfoot) && IsAncestor(pelvis, lfoot) && IsAncestor(pelvis, lhand) || IsAncestor(pelvis, rhand) || IsAncestor(pelvis, lhand)) )
        {
            OnGUIWarning(avObj, "This avatar has a split heirarchy (Hips bone is not the ancestor of all humanoid bones). IK may not work correctly.");
        }
    }

    void OnGUIAvatar(VRCSDK2.VRC_AvatarDescriptor avatar)
    {
        EditorGUI.BeginChangeCheck();

        errorCount = warningCount = 0;

        EditorGUILayout.InspectorTitlebar(true, avatar.gameObject);

        int polycount;
        Bounds bounds;
        AnalyzeGeometry(avatar.gameObject, out bounds, out polycount);
        if (polycount < 10000)
            OnGUIInformation(new GameObject[] { avatar.gameObject }, "Polygons: " + polycount);
        else if (polycount < 15000)
            OnGUIWarning(avatar.gameObject, "Polygons: " + polycount + " - Please try to reduce your avatar poly count to less thatn 10k.");
        else if (polycount < 20000)
            OnGUIWarning(avatar.gameObject, "Polygons: " + polycount + " - This avatar will not perform well on many systems.");
        else
            OnGUIError(new GameObject[] { avatar.gameObject }, "Polygons: " + polycount + " - This avatar has too many polygons. It must have less than 20k and should have less than 10k.");

        if (bounds.size.x > 5f || bounds.size.y > 6.0f || bounds.size.z > 5f)
            OnGUIError(avatar.gameObject, "This avatar measures too large on at least one axis. It must be <5m on a side but it's bounds are " + bounds.size.ToString());

        var eventHandler = avatar.GetComponentInChildren<VRCSDK2.VRC_EventHandler>();
        if (eventHandler != null)
        {
            OnGUIError(avatar.gameObject, "This avatar contains an EventHandler, which is not currently supported in VRChat.");
        }

        if (avatar.lipSync == VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape && avatar.VisemeSkinnedMesh == null)
            OnGUIError(avatar.gameObject, "This avatar uses Visemes but the Face Mesh is not specified.");

        var anim = avatar.GetComponent<Animator>();
        if (anim == null)
        {
            OnGUIWarning(avatar.gameObject, "This avatar does not contain an animator, and will not animate in VRChat.");
        }
        else if (anim.isHuman == false)
        {
            OnGUIWarning(avatar.gameObject, "This avatar is not imported as a humanoid rig and will not play VRChat's provided animation set.");
        }
        else if (avatar.gameObject.activeInHierarchy == false)
        {
            OnGUIError(avatar.gameObject, "Your avatar is disabled in the scene heirarchy!");
        }
        else
        {
            Transform foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform shoulder = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            if (foot == null)
                OnGUIError(avatar.gameObject, "Your avatar is humanoid, but it's feet aren't specified!");
            if (shoulder == null)
                OnGUIError(avatar.gameObject, "Your avatar is humanoid, but it's upper arms aren't specified!");

            if (foot != null && shoulder != null)
            {
                Vector3 footPos = foot.position - avatar.transform.position;
                if (footPos.y < 0)
                    OnGUIWarning(avatar.gameObject, "Avatar feet are beneath the avatar's origin (the floor). That's probably not what you want.");
                Vector3 shoulderPosition = shoulder.position - avatar.transform.position;
                if (shoulderPosition.y < 0.2f)
                    OnGUIError(avatar.gameObject, "This avatar is too short. The minimum is 20cm shoulder height.");
                else if (shoulderPosition.y < 1.0f)
                    OnGUIWarning(avatar.gameObject, "This avatar is short. This is probably shorter than you want.");
                else if (shoulderPosition.y > 5.0f)
                    OnGUIWarning(avatar.gameObject, "This avatar is too tall. The maximum is 5m shoulder height.");
                else if (shoulderPosition.y > 2.5f)
                    OnGUIWarning(avatar.gameObject, "This avatar is tall. This is probably taller than you want.");
            }
            AnalyzeIK(avatar.gameObject, anim);
        }

        EditorGUILayout.LabelField("Errors: " + errorCount + " Warnings: " + warningCount);

        GUI.enabled = (errorCount == 0) || (APIUser.CurrentUser.developerType.HasValue && APIUser.CurrentUser.developerType.Value == APIUser.DeveloperType.Internal);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build & Publish"))
        {
            VRC_SdkBuilder.shouldBuildUnityPackage = VRC.AccountEditorWindow.FutureProofPublishEnabled;
            VRC_SdkBuilder.ExportAndUploadAvatarBlueprint(avatar.gameObject);
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(avatar);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
}
