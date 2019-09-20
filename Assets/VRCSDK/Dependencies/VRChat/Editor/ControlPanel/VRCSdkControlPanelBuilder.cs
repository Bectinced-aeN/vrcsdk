using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;
using VRC;
using VRC.Core;
using VRCSDK2.Validation;
using VRCSDK2.Validation.Performance;
using VRCSDK2.Validation.Performance.Stats;

public partial class VRCSdkControlPanel : EditorWindow
{
    public static System.Action _EnableSpatialization = null;   // assigned in AutoAddONSPAudioSourceComponents

    const string kCantPublishContent = "Before you can upload avatars or worlds, you will need to spend some time in VRChat.";
    const string kCantPublishAvatars = "Before you can upload avatars, you will need to spend some time in VRChat.";
    const string kCantPublishWorlds = "Before you can upload worlds, you will need to spend some time in VRChat.";
    private const string FIX_ISSUES_TO_BUILD_OR_TEST_WARNING_STRING = "You must address the above issues before you can build or test this content!";
    private const string kAvatarOptimizationTipsURL = "https://docs.vrchat.com/docs/avatar-optimizing-tips";
    private const string kAvatarRigRequirementsURL = "https://docs.vrchat.com/docs/rig-requirements";

    static Texture _perfIcon_Excellent;
    static Texture _perfIcon_Good;
    static Texture _perfIcon_Medium;
    static Texture _perfIcon_Poor;
    static Texture _perfIcon_VeryPoor;
    static Texture _bannerImage;

    private void ResetIssues()
    {
        GUIErrors.Clear();
        GUIInfos.Clear();
        GUIWarnings.Clear();
        GUILinks.Clear();
        GUIStats.Clear();
        checkedForIssues = false;
    }

    bool checkedForIssues = false;

    class Issue
    {
        public string issueText;
        public System.Action showThisIssue;
        public System.Action fixThisIssue;

        public Issue(string text, System.Action show, System.Action fix)
        {
            issueText = text;
            showThisIssue = show;
            fixThisIssue = fix;
        }

        public class Equality : IEqualityComparer<Issue>, IComparer<Issue>
        {
            public bool Equals(Issue b1, Issue b2)
            {
                return (b1.issueText == b2.issueText);
            }
            public int Compare(Issue b1, Issue b2)
            {
                return string.Compare(b1.issueText, b2.issueText);
            }
            public int GetHashCode(Issue bx)
            {
                return bx.issueText.GetHashCode();
            }
        }
    }

    Dictionary<Object, List<Issue>> GUIErrors = new Dictionary<Object, List<Issue>>();
    Dictionary<Object, List<Issue>> GUIWarnings = new Dictionary<Object, List<Issue>>();
    Dictionary<Object, List<Issue>> GUIInfos = new Dictionary<Object, List<Issue>>();
    Dictionary<Object, List<Issue>> GUILinks = new Dictionary<Object, List<Issue>>();
    Dictionary<Object, List<KeyValuePair<string, PerformanceRating>>> GUIStats = new Dictionary<Object, List<KeyValuePair<string, PerformanceRating>>>();

    private string customNamespace;

    void AddToReport(Dictionary<Object, List<Issue>> report, Object subject, string output, System.Action show, System.Action fix)
    {
        if (subject == null)
            subject = this;
        if (!report.ContainsKey(subject))
            report.Add(subject, new List<Issue>());

        var issue = new Issue(output, show, fix);
        if (!report[subject].Contains(issue, new Issue.Equality()))
        {
            report[subject].Add(issue);
            report[subject].Sort(new Issue.Equality());
        }
    }

    void BuilderAssemblyReload()
    {
        ResetIssues();
    }

    void OnGUIError(Object subject, string output, System.Action show, System.Action fix)
    {
        AddToReport(GUIErrors, subject, output, show, fix);
    }

    void OnGUIWarning(Object subject, string output, System.Action show, System.Action fix)
    {
        AddToReport(GUIWarnings, subject, output, show, fix);
    }

    void OnGUIInformation(Object subject, string output)
    {
        AddToReport(GUIInfos, subject, output, null, null);
    }

    void OnGUILink(Object subject, string output, string link)
    {
        AddToReport(GUILinks, subject, output + "\n" + link, null, null);
    }

    void OnGUIStat(Object subject, string output, PerformanceRating rating)
    {
        if (subject == null)
            subject = this;
        if (!GUIStats.ContainsKey(subject))
            GUIStats.Add(subject, new List<KeyValuePair<string, PerformanceRating>>());
        GUIStats[subject].Add(new KeyValuePair<string, PerformanceRating>(output, rating));
    }

    VRCSDK2.VRC_SceneDescriptor[] scenes;
    VRCSDK2.VRC_AvatarDescriptor[] avatars;
    Vector2 scrollPos;
    Vector2 avatarListScrollPos;
    VRCSDK2.VRC_AvatarDescriptor selectedAvatar;

    public static void SelectAvatar( VRCSDK2.VRC_AvatarDescriptor  avatar )
    {
        if (window != null)
            window.selectedAvatar = avatar;
    }

    private bool showAvatarPerformanceDetails
    {
        get { return EditorPrefs.GetBool("VRCSDK2_showAvatarPerformanceDetails", false); }
        set { EditorPrefs.SetBool("VRCSDK2_showAvatarPerformanceDetails", value); }
    }

    public void FindScenesAndAvatars()
    {
        var newScenes = (VRCSDK2.VRC_SceneDescriptor[])VRC.Tools.FindSceneObjectsOfTypeAll<VRCSDK2.VRC_SceneDescriptor>();
        List<VRCSDK2.VRC_AvatarDescriptor> allavatars = VRC.Tools.FindSceneObjectsOfTypeAll<VRCSDK2.VRC_AvatarDescriptor>().ToList();
        // select only the active avatars
        var newAvatars = allavatars.Where(av => (null != av) && av.gameObject.activeInHierarchy).ToArray();

        if (scenes != null)
        {
            foreach (var s in newScenes)
                if (scenes.Contains(s) == false)
                    checkedForIssues = false;
        }

        if (avatars != null)
        {
            foreach (var a in newAvatars)
                if (avatars.Contains(a) == false)
                    checkedForIssues = false;
        }

        scenes = newScenes;
        avatars = newAvatars;
    }

    void ShowBuilder()
    {
        if (VRC.Core.RemoteConfig.IsInitialized())
        {
            string sdkUnityVersion = VRC.Core.RemoteConfig.GetString("sdkUnityVersion");
            if (Application.unityVersion != sdkUnityVersion)
            {
                OnGUIWarning(null, "You are not using the recommended Unity version for the VRChat SDK. Content built with this version may not work correctly. Please use Unity " + sdkUnityVersion, null, null);
            }
        }

        FindScenesAndAvatars();

        if ((null == scenes) || (null == avatars)) return;

        if (scenes.Length > 0 && avatars.Length > 0)
        {
            GameObject[] gos = new GameObject[avatars.Length];
            for (int i = 0; i < avatars.Length; ++i)
                gos[i] = avatars[i].gameObject;
            OnGUIError(null, "a unity scene containing a VRChat Scene Descriptor should not also contain avatars.",
                delegate ()
                {
                    List<GameObject> show = new List<GameObject>();
                    foreach (var s in scenes)
                        show.Add(s.gameObject);
                    foreach (var a in avatars)
                        show.Add(a.gameObject);
                    Selection.objects = show.ToArray();
                }, null);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(GUILayout.Width(SdkWindowWidth));
            OnGUIShowIssues();
            EditorGUILayout.EndVertical();
        }
        else if (scenes.Length > 1)
        {
            GameObject[] gos = new GameObject[scenes.Length];
            for (int i = 0; i < scenes.Length; ++i)
                gos[i] = scenes[i].gameObject;
            OnGUIError(null, "a unity scene containing a VRChat Scene Descriptor should only contain one scene descriptor.",
                delegate { Selection.objects = gos; }, null);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(GUILayout.Width(SdkWindowWidth));
            OnGUIShowIssues();
            EditorGUILayout.EndVertical();
        }
        else if (scenes.Length == 1)
        {
            bool inScroller = false;
            try
            {
                bool setupRequired = OnGUISceneSetup(scenes[0]);

                if (!setupRequired)
                {
                    if (!checkedForIssues)
                    {
                        ResetIssues();
                        OnGUISceneCheck(scenes[0]);
                        checkedForIssues = true;
                    }

                    OnGUISceneSettings(scenes[0]);

                    inScroller = true;
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(SdkWindowWidth));
                    OnGUIShowIssues(scenes[0]);
                    EditorGUILayout.EndScrollView();
                    inScroller = false;

                    GUILayout.FlexibleSpace();

                    OnGUIScene(scenes[0]);

                }
                else
                {
                    OnGuiFixIssuesToBuildOrTest();
                }
            }
            catch (System.Exception)
            {
                if (inScroller)
                    EditorGUILayout.EndScrollView();
            }
        }
        else if (avatars.Length > 0)
        {
            if (!checkedForIssues)
            {
                ResetIssues();
                for (int i = 0; i < avatars.Length; ++i)
                    OnGUIAvatarCheck(avatars[i]);
                checkedForIssues = true;
            }

            bool drawList = true;
            if( avatars.Length == 1 )
            {
                drawList = false;
                selectedAvatar = avatars[0];
            }

            if (drawList)
            {
                EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"), GUILayout.Width(SdkWindowWidth), GUILayout.MaxHeight(150));
                avatarListScrollPos = EditorGUILayout.BeginScrollView(avatarListScrollPos, false, false);

                for (int i = 0; i < avatars.Length; ++i)
                {
                    var av = avatars[i];
                    EditorGUILayout.Space();
                    if (selectedAvatar == av)
                    {
                        if (GUILayout.Button(av.gameObject.name, listButtonStyleSelected, GUILayout.Width(SdkWindowWidth - 50)))
                            selectedAvatar = null;
                    }
                    else
                    {
                        if (GUILayout.Button(av.gameObject.name, ((i & 0x01) > 0) ? (listButtonStyleOdd) : (listButtonStyleEven), GUILayout.Width(SdkWindowWidth - 50)))
                            selectedAvatar = av;
                    }
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(GUILayout.Width(SdkWindowWidth));
            OnGUIShowIssues();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            if (selectedAvatar != null)
            {
                EditorGUILayout.BeginVertical(boxGuiStyle);
                OnGUIAvatarSettings(selectedAvatar);
                EditorGUILayout.EndVertical();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(SdkWindowWidth));
                OnGUIShowIssues(selectedAvatar);
                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical(boxGuiStyle);
                OnGUIAvatar(selectedAvatar);
                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("A VRC_SceneDescriptor or VRC_AvatarDescriptor is required to build VRChat SDK Content", titleGuiStyle);
        }
    }

    bool showLayerHelp = false;
    int numClients = 1;

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

    void DrawIssueBox(MessageType msgType, Texture icon, string message, System.Action show, System.Action fix)
    {
        GUIStyle style = GUI.skin.GetStyle("HelpBox");

        EditorGUILayout.BeginHorizontal();
        if (icon != null)
            GUILayout.Box(new GUIContent(message, icon), style);
        else
            EditorGUILayout.HelpBox(message, msgType);
        EditorGUILayout.BeginVertical();
        GUI.enabled = show != null;
        if (GUILayout.Button("Select"))
            show();
        GUI.enabled = fix != null;
        if (GUILayout.Button("Auto Fix"))
        {
            fix();
            EditorApplication.MarkSceneDirty();
            //EditorSceneManager.MarkSceneDirty();

            checkedForIssues = false;
            Repaint();
        }
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.Space();
    }

    void OnGuiFixIssuesToBuildOrTest()
    {
        GUIStyle s = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.Space();
        GUILayout.BeginVertical(boxGuiStyle, GUILayout.Height(WARNING_ICON_SIZE), GUILayout.Width(SdkWindowWidth));
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        var textDimensions = s.CalcSize(new GUIContent(FIX_ISSUES_TO_BUILD_OR_TEST_WARNING_STRING));
        GUILayout.Label(new GUIContent(warningIconGraphic), GUILayout.Width(WARNING_ICON_SIZE), GUILayout.Height(WARNING_ICON_SIZE));
        EditorGUILayout.LabelField(FIX_ISSUES_TO_BUILD_OR_TEST_WARNING_STRING, s, GUILayout.Width(textDimensions.x), GUILayout.Height(WARNING_ICON_SIZE));
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    void OnGUIShowIssues(Object subject = null)
    {
        if (subject == null)
            subject = this;

        EditorGUI.BeginChangeCheck();

        GUIStyle style = GUI.skin.GetStyle("HelpBox");

        if (GUIErrors.ContainsKey(subject))
            foreach (Issue error in GUIErrors[subject].Where(s => !string.IsNullOrEmpty(s.issueText)))
                DrawIssueBox(MessageType.Error, null, error.issueText, error.showThisIssue, error.fixThisIssue);
        if (GUIWarnings.ContainsKey(subject))
            foreach (Issue error in GUIWarnings[subject].Where(s => !string.IsNullOrEmpty(s.issueText)))
                DrawIssueBox(MessageType.Warning, null, error.issueText, error.showThisIssue, error.fixThisIssue);

        if (GUIStats.ContainsKey(subject))
        {
            foreach (var kvp in GUIStats[subject].Where(k => k.Value == PerformanceRating.VeryPoor))
                GUILayout.Box(new GUIContent(kvp.Key, GetPerformanceIconForRating(kvp.Value)), style);

            foreach (var kvp in GUIStats[subject].Where(k => k.Value == PerformanceRating.Poor))
                GUILayout.Box(new GUIContent(kvp.Key, GetPerformanceIconForRating(kvp.Value)), style);

            foreach (var kvp in GUIStats[subject].Where(k => k.Value == PerformanceRating.Medium))
                GUILayout.Box(new GUIContent(kvp.Key, GetPerformanceIconForRating(kvp.Value)), style);

            foreach (var kvp in GUIStats[subject].Where(k => k.Value == PerformanceRating.Good || k.Value == PerformanceRating.Excellent))
                GUILayout.Box(new GUIContent(kvp.Key, GetPerformanceIconForRating(kvp.Value)), style);
        }

        if (GUIInfos.ContainsKey(subject))
            foreach (Issue error in GUIInfos[subject].Where(s => !string.IsNullOrEmpty(s.issueText)))
                EditorGUILayout.HelpBox(error.issueText, MessageType.Info);
        if (GUILinks.ContainsKey(subject))
        {
            EditorGUILayout.BeginVertical(style);
            foreach (Issue error in GUILinks[subject].Where(s => !string.IsNullOrEmpty(s.issueText)))
            {
                var s = error.issueText.Split('\n');
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(s[0]);
                if (GUILayout.Button("Open Link", GUILayout.Width(100)))
                    Application.OpenURL(s[1]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(subject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }

    private Texture GetPerformanceIconForRating(PerformanceRating value)
    {
        if (_perfIcon_Excellent == null)
            _perfIcon_Excellent = Resources.Load<Texture>("PerformanceIcons/Perf_Great_32");
        if (_perfIcon_Good == null)
            _perfIcon_Good = Resources.Load<Texture>("PerformanceIcons/Perf_Good_32");
        if (_perfIcon_Medium == null)
            _perfIcon_Medium = Resources.Load<Texture>("PerformanceIcons/Perf_Medium_32");
        if (_perfIcon_Poor == null)
            _perfIcon_Poor = Resources.Load<Texture>("PerformanceIcons/Perf_Poor_32");
        if (_perfIcon_VeryPoor == null)
            _perfIcon_VeryPoor = Resources.Load<Texture>("PerformanceIcons/Perf_Horrible_32");

        switch (value)
        {
            case PerformanceRating.Excellent:
                return _perfIcon_Excellent;
            case PerformanceRating.Good:
                return _perfIcon_Good;
            case PerformanceRating.Medium:
                return _perfIcon_Medium;
            case PerformanceRating.Poor:
                return _perfIcon_Poor;
            case PerformanceRating.None:
            case PerformanceRating.VeryPoor:
                return _perfIcon_VeryPoor;
        }

        return _perfIcon_Excellent;
    }

    Texture2D CreateBackgroundColorImage(Color color)
    {
        int w = 4, h = 4;
        Texture2D back = new Texture2D(w, h);
        Color[] buffer = new Color[w * h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                buffer[i + w * j] = color;
        back.SetPixels(buffer);
        back.Apply(false);
        return back;
    }

    bool IsAudioSource2D(AudioSource src)
    {
        AnimationCurve curve = src.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
        return src.spatialBlend == 0 && (curve == null || curve.keys.Length <= 1);
    }

    void OnGUISceneCheck(VRCSDK2.VRC_SceneDescriptor scene)
    {
        CheckUploadChanges(scene);

        if (VRC.Core.APIUser.CurrentUser != null && VRC.Core.APIUser.CurrentUser.hasScriptingAccess && !CustomDLLMaker.DoesScriptDirExist())
        {
            CustomDLLMaker.CreateDirectories();
        }

        Vector3 g = Physics.gravity;
        if (g.x != 0.0f || g.z != 0.0f)
            OnGUIWarning(scene, "Gravity vector is not straight down. Though we support different gravity, player orientation is always 'upwards' so things don't always behave as you intend.",
                delegate { EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics"); /*SettingsService.OpenProjectSettings("Project/Physics");*/ }, null);
        if (g.y > 0)
            OnGUIWarning(scene, "Gravity vector is not straight down, inverted or zero gravity will make walking extremely difficult.",
                delegate { EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics"); /*SettingsService.OpenProjectSettings("Project/Physics");*/}, null);
        if (g.y == 0)
            OnGUIWarning(scene, "Zero gravity will make walking extremely difficult, though we support different gravity, player orientation is always 'upwards' so this may not have the effect you're looking for.",
                delegate { EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics"); /*SettingsService.OpenProjectSettings("Project/Physics");*/}, null);

        // warn those without scripting access if they choose to script locally
        if (VRC.Core.APIUser.CurrentUser != null && !VRC.Core.APIUser.CurrentUser.hasScriptingAccess && CustomDLLMaker.DoesScriptDirExist())
        {
            OnGUIWarning(scene, "Your account does not have permissions to upload custom scripts. You can test locally but need to contact VRChat to publish your world with scripts.", null, null);
        }

        if (scene.autoSpatializeAudioSources)
        {
            OnGUIWarning(scene, "Your scene previously used the 'Auto Spatialize Audio Sources' feature. This has been deprecated, press 'Fix' to disable. Also, please add VRC_SpatialAudioSource to all your audio sources. Make sure Spatial Blend is set to 3D for the sources you want to spatialize.",
                        null,
                        delegate { scene.autoSpatializeAudioSources = false; }
                        );
        }

        var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        foreach( var a in audioSources )
        {
            if( a.GetComponent<ONSPAudioSource>() != null )
            {
                OnGUIWarning(scene, "Found audio source(s) using ONSP, this is deprecated. Press 'fix' to convert to VRC_SpatialAudioSource.",
                        delegate { Selection.activeObject = a.gameObject; }, 
                        delegate { Selection.activeObject = a.gameObject; AutoAddSpatialAudioComponents.ConvertONSPAudioSource(a); }
                        );
                break;
            }
            else if( a.GetComponent<VRCSDK2.VRC_SpatialAudioSource>() == null )
            {
                string msg = "Found 3D audio source with no VRC Spatial Audio component, this is deprecated. Press 'fix' to add a VRC_SpatialAudioSource.";
                if (IsAudioSource2D(a))
                    msg = "Found 2D audio source with no VRC Spatial Audio component, this is deprecated. Press 'fix' to add a (disabled) VRC_SpatialAudioSource.";

                OnGUIWarning(scene, msg,
                        delegate { Selection.activeObject = a.gameObject; }, 
                        delegate { Selection.activeObject = a.gameObject; AutoAddSpatialAudioComponents.AddVRCSpatialToBareAudioSource(a); }
                        );
                break;
            }
        }

        foreach (VRCSDK2.VRC_DataStorage ds in GameObject.FindObjectsOfType<VRCSDK2.VRC_DataStorage>())
        {
            VRCSDK2.VRC_ObjectSync os = ds.GetComponent<VRCSDK2.VRC_ObjectSync>();
            if (os != null && os.SynchronizePhysics)
                OnGUIWarning(scene, ds.name + " has a VRC_DataStorage and VRC_ObjectSync, with SynchronizePhysics enabled.",
                    delegate { Selection.activeObject = os.gameObject; }, null);
        }

        string vrcFilePath = WWW.UnEscapeURL(UnityEditor.EditorPrefs.GetString("lastVRCPath"));
        int fileSize = 0;
        if (!string.IsNullOrEmpty(vrcFilePath) && ValidationHelpers.CheckIfAssetBundleFileTooLarge(ContentType.World, vrcFilePath, out fileSize))
        {
            OnGUIWarning(scene, ValidationHelpers.GetAssetBundleOverSizeLimitMessageSDKWarning(ContentType.World, fileSize), null, null);
        }

        if (scene.UpdateTimeInMS < (int)(1000f / 90f * 3f))
            OnGUIWarning(scene, "Room has a very fast update rate; experience may suffer with many users.",
                delegate { Selection.activeObject = scene.gameObject; },
                delegate { scene.UpdateTimeInMS = 100; Selection.activeObject = scene.gameObject; });

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.transform.parent == null)
            {
                // check root game objects
#if UNITY_ANDROID
                IEnumerable<Shader> illegalShaders = WorldValidation.FindIllegalShaders(go);
                foreach (Shader s in illegalShaders)
                {
                    OnGUIWarning(scene, "World uses unsupported shader '" + s.name + "'. This could cause low performance or future compatibility issues.", null, null);
                }
#endif
            }
            else
            {
                //if (go.transform.parent.name == "Action-PlayHaptics")
                //    Debug.Log("break");

                // check sibling game objects
                for (int idx = 0; idx < go.transform.parent.childCount; ++idx)
                {
                    Transform t = go.transform.parent.GetChild(idx);
                    if (t == go.transform)
                        continue;
                    else if (t.name == go.transform.name
                            && !(t.GetComponent<VRCSDK2.VRC_ObjectSync>()
                                || t.GetComponent<VRCSDK2.VRC_SyncAnimation>()
                                || t.GetComponent<VRCSDK2.VRC_SyncVideoPlayer>()
                                || t.GetComponent<VRCSDK2.VRC_SyncVideoStream>()))
                    {
                        string path = t.name;
                        Transform p = t.parent;
                        while (p != null)
                        {
                            path = p.name + "/" + path;
                            p = p.parent;
                        }

                        OnGUIWarning(scene, "Sibling objects share the same path, which may break network events: " + path,
                            delegate
                            {
                                List<GameObject> gos = new List<GameObject>();
                                for (int c = 0; c < go.transform.parent.childCount; ++c)
                                    if (go.transform.parent.GetChild(c).name == go.name)
                                        gos.Add(go.transform.parent.GetChild(c).gameObject);
                                Selection.objects = gos.ToArray();
                            },
                            delegate
                            {
                                List<GameObject> gos = new List<GameObject>();
                                for (int c = 0; c < go.transform.parent.childCount; ++c)
                                    if (go.transform.parent.GetChild(c).name == go.name)
                                        gos.Add(go.transform.parent.GetChild(c).gameObject);
                                Selection.objects = gos.ToArray();
                                for (int i = 0; i < gos.Count; ++i)
                                    gos[i].name = gos[i].name + "-" + i.ToString("00");
                            });
                        break;
                    }
                }
            }
        }
    }

    bool OnGUISceneSetup(VRCSDK2.VRC_SceneDescriptor scene)
    {
        bool mandatoryExpand = !UpdateLayers.AreLayersSetup() || !UpdateLayers.IsCollisionLayerMatrixSetup();
        if (mandatoryExpand)
            EditorGUILayout.LabelField("VRChat Scene Setup", titleGuiStyle, GUILayout.Height(50));

        if (!UpdateLayers.AreLayersSetup())
        {
            GUILayout.BeginVertical(boxGuiStyle, GUILayout.Height(100), GUILayout.Width(SdkWindowWidth));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            EditorGUILayout.Space();
            GUILayout.Label("Layers", infoGuiStyle);
            GUILayout.Label("VRChat scenes must have the same Unity layer configuration as VRChat so we can all predict things like physics and collisions. Pressing this button will configure your project's layers to match VRChat.", infoGuiStyle, GUILayout.Width(300));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(150));
            GUILayout.Label("", GUILayout.Height(15));
            if (UpdateLayers.AreLayersSetup())
            {
                GUILayout.Label("Step Complete!", infoGuiStyle);
            }
            else if (GUILayout.Button("Setup Layers for VRChat"))
            {
                bool doIt = EditorUtility.DisplayDialog("Setup Layers for VRChat", "This adds all VRChat layers to your project and pushes any custom layers down the layer list. If you have custom layers assigned to gameObjects, you'll need to reassign them. Are you sure you want to continue?", "Do it!", "Don't do it");
                if (doIt)
                    UpdateLayers.SetupEditorLayers();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        if (!UpdateLayers.IsCollisionLayerMatrixSetup())
        {
            GUILayout.BeginVertical(boxGuiStyle, GUILayout.Height(100), GUILayout.Width(SdkWindowWidth));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            EditorGUILayout.Space();
            GUILayout.Label("Collision Matrix", infoGuiStyle);
            GUILayout.Label("VRChat uses specific layers for collision. In order for testing and development to run smoothly it is necessary to configure your project's collision matrix to match that of VRChat.", infoGuiStyle, GUILayout.Width(300));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(150));
            GUILayout.Label("", GUILayout.Height(15));
            if (UpdateLayers.AreLayersSetup() == false)
            {
                GUILayout.Label("You must first configure your layers for VRChat to proceed. Please see above.", infoGuiStyle);
            }
            else if (UpdateLayers.IsCollisionLayerMatrixSetup())
            {
                GUILayout.Label("Step Complete!", infoGuiStyle);
            }
            else
            {
                if (GUILayout.Button("Set Collision Matrix"))
                {
                    bool doIt = EditorUtility.DisplayDialog("Setup Collision Layer Matrix for VRChat", "This will setup the correct physics collisions in the PhysicsManager for VRChat layers. Are you sure you want to continue?", "Do it!", "Don't do it");
                    if (doIt)
                    {
                        UpdateLayers.SetupCollisionLayerMatrix();
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        return mandatoryExpand;
    }

    void OnGUIAvatarSettings(VRCSDK2.VRC_AvatarDescriptor avatar)
    {
        GUILayout.BeginVertical(boxGuiStyle, GUILayout.Width(SdkWindowWidth));

        string name = avatar.gameObject.name;
        if (avatar.apiAvatar != null)
            name = (avatar.apiAvatar as ApiAvatar).name;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(name, titleGuiStyle);

        PipelineManager pm = avatar.GetComponent<PipelineManager>();
        if (pm != null && !string.IsNullOrEmpty(pm.blueprintId))
        {
            if (avatar.apiAvatar == null)
            {
                ApiAvatar av = API.FromCacheOrNew<ApiAvatar>(pm.blueprintId);
                av.Fetch(
                    (c) => avatar.apiAvatar = c.Model as ApiAvatar,
                    (c) =>
                    {
                        if (c.Code == 404)
                        {
                            Debug.LogErrorFormat("Could not load avatar {0} because it didn't exist.", pm.blueprintId);
                            ApiCache.Invalidate<ApiWorld>(pm.blueprintId);
                        }
                        else
                            Debug.LogErrorFormat("Could not load avatar {0} because {1}", pm.blueprintId, c.Error);
                    });
                avatar.apiAvatar = av;
            }
        }

        if (avatar.apiAvatar != null)
        {
            ApiAvatar a = (avatar.apiAvatar as ApiAvatar);
            EditorGUILayout.LabelField("Version: " + a.version.ToString());
            EditorGUILayout.LabelField("Name: " + a.name);
            GUILayout.Label(a.description, infoGuiStyle, GUILayout.Width(400));
            EditorGUILayout.LabelField("Release: " + a.releaseStatus);
            if (a.tags != null)
                foreach (var t in a.tags)
                    EditorGUILayout.LabelField("Tag: " + t);

            if (a.supportedPlatforms == ApiModel.SupportedPlatforms.Android || a.supportedPlatforms == ApiModel.SupportedPlatforms.All)
                EditorGUILayout.LabelField("Supports: Android");
            if (a.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows || a.supportedPlatforms == ApiModel.SupportedPlatforms.All)
                EditorGUILayout.LabelField("Supports: Windows");

            //w.imageUrl;
        }
        else
        {
            EditorGUILayout.LabelField("Version: " + "Unpublished");
            EditorGUILayout.LabelField("Name: " + "");
            GUILayout.Label("", infoGuiStyle, GUILayout.Width(400));
            EditorGUILayout.LabelField("Release: " + "");
            //foreach (var t in w.tags)
            //    EditorGUILayout.LabelField("Tag: " + "");

            //if (w.supportedPlatforms == ApiModel.SupportedPlatforms.Android || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
            //    EditorGUILayout.LabelField("Supports: Android");
            //if (w.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
            //    EditorGUILayout.LabelField("Supports: Windows");

            //w.imageUrl;
        }

        GUILayout.EndVertical();
    }

    void OnGUISceneSettings(VRCSDK2.VRC_SceneDescriptor scene)
    {
        GUILayout.BeginVertical(boxGuiStyle, GUILayout.Width(SdkWindowWidth));

        string name = "Unpublished VRChat World";
        if (scene.apiWorld != null)
            name = (scene.apiWorld as ApiWorld).name;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(name, titleGuiStyle);

        PipelineManager[] pms = (PipelineManager[])VRC.Tools.FindSceneObjectsOfTypeAll<PipelineManager>();
        if (pms.Length == 1)
        {
            if (!string.IsNullOrEmpty(pms[0].blueprintId))
            {
                if (scene.apiWorld == null)
                {
                    ApiWorld world = API.FromCacheOrNew<ApiWorld>(pms[0].blueprintId);
                    world.Fetch(null, null,
                        (c) => scene.apiWorld = c.Model as ApiWorld,
                        (c) =>
                        {
                            if (c.Code == 404)
                            {
                                Debug.LogErrorFormat("Could not load world {0} because it didn't exist.", pms[0].blueprintId);
                                ApiCache.Invalidate<ApiWorld>(pms[0].blueprintId);
                            }
                            else
                                Debug.LogErrorFormat("Could not load world {0} because {1}", pms[0].blueprintId, c.Error);
                        });
                    scene.apiWorld = world;
                }
            }
            else
            {   // clear scene.apiworld if blueprint ID has been detached, so world details in builder panel are also cleared
                scene.apiWorld = null;
            }
        }

        if (scene.apiWorld != null)
        {
            ApiWorld w = (scene.apiWorld as ApiWorld);
            EditorGUILayout.LabelField("Version: " + w.version.ToString());
            EditorGUILayout.LabelField("Name: " + w.name);
            GUILayout.Label(w.description, infoGuiStyle, GUILayout.Width(400));
            EditorGUILayout.LabelField("Capacity: " + w.capacity);
            EditorGUILayout.LabelField("Release: " + w.releaseStatus);
            if (w.tags != null)
                foreach (var t in w.tags)
                    EditorGUILayout.LabelField("Tag: " + t);

            if (w.supportedPlatforms == ApiModel.SupportedPlatforms.Android || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
                EditorGUILayout.LabelField("Supports: Android");
            if (w.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
                EditorGUILayout.LabelField("Supports: Windows");

            //w.imageUrl;
        }
        else
        {
            EditorGUILayout.LabelField("Version: " + "Unpublished");
            EditorGUILayout.LabelField("Name: " + "");
            GUILayout.Label("", infoGuiStyle, GUILayout.Width(400));
            EditorGUILayout.LabelField("Capacity: " + "");
            EditorGUILayout.LabelField("Release: " + "");
            //foreach (var t in w.tags)
            //    EditorGUILayout.LabelField("Tag: " + "");

            //if (w.supportedPlatforms == ApiModel.SupportedPlatforms.Android || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
            //    EditorGUILayout.LabelField("Supports: Android");
            //if (w.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows || w.supportedPlatforms == ApiModel.SupportedPlatforms.All)
            //    EditorGUILayout.LabelField("Supports: Windows");

            //w.imageUrl;
        }

        if (APIUser.CurrentUser.hasScriptingAccess && VRCSettings.Get().DisplayAdvancedSettings)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            customNamespace = EditorGUILayout.TextField("Custom Namespace", customNamespace);
            if (GUILayout.Button("Regenerate"))
            {
                customNamespace = "vrc" + Path.GetRandomFileName().Replace(".", "");
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    void OnGUIScene(VRCSDK2.VRC_SceneDescriptor scene)
    {
        EditorGUILayout.Space();

        GUILayout.BeginVertical(boxGuiStyle, GUILayout.Width(SdkWindowWidth));
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.Space();
        GUILayout.Label("Offline Testing", infoGuiStyle);
        GUILayout.Label("Before uploading your world you may build and test it in the VRChat client. You won't be able to invite anyone from online but you can launch multiple of your own clients.", infoGuiStyle);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.Space();
        numClients = EditorGUILayout.IntField("Number of Clients", numClients);

        GUI.enabled = (GUIErrors.Count == 0 && checkedForIssues);
        string lastUrl = VRC_SdkBuilder.GetLastUrl();
        bool lastBuildPresent = lastUrl != null;

        if (lastBuildPresent == false)
            GUI.enabled = false;
        if (VRCSettings.Get().DisplayAdvancedSettings)
        {
            if (GUILayout.Button("Last Build"))
            {
                VRC_SdkBuilder.shouldBuildUnityPackage = false;
                VRC_SdkBuilder.numClientsToLaunch = numClients;
                VRC_SdkBuilder.RunLastExportedSceneResource();
            }
            if (APIUser.CurrentUser.hasSuperPowers)
            {
                if (GUILayout.Button("Copy Test URL"))
                {
                    TextEditor te = new TextEditor();
                    te.text = lastUrl;
                    te.SelectAll();
                    te.Copy();
                }
            }
        }
        GUI.enabled = (GUIErrors.Count == 0 && checkedForIssues) || APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal;
        if (GUILayout.Button("Build & Test"))
        {
            EnvConfig.ConfigurePlayerSettings();
            VRC_SdkBuilder.shouldBuildUnityPackage = false;
            VRC.AssetExporter.CleanupUnityPackageExport();  // force unity package rebuild on next publish
            VRC_SdkBuilder.numClientsToLaunch = numClients;
            VRC_SdkBuilder.PreBuildBehaviourPackaging();
            VRC_SdkBuilder.ExportSceneResourceAndRun(customNamespace);
        }
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical(boxGuiStyle, GUILayout.Width(SdkWindowWidth));

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.Space();
        GUILayout.Label("Online Publishing", infoGuiStyle);
        GUILayout.Label("In order for other people to enter your world in VRChat it must be built and published to our game servers.", infoGuiStyle);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.Space();

        if (lastBuildPresent == false)
            GUI.enabled = false;
        if (VRCSettings.Get().DisplayAdvancedSettings)
        {
            if (GUILayout.Button("Last Build"))
            {
                if (APIUser.CurrentUser.canPublishWorlds)
                {
                    VRC_SdkBuilder.shouldBuildUnityPackage = VRCSdkControlPanel.FutureProofPublishEnabled;
                    VRC_SdkBuilder.UploadLastExportedSceneBlueprint();
                }
                else
                {
                    ShowContentPublishPermissionsDialog();
                }
            }
        }
        GUI.enabled = (GUIErrors.Count == 0 && checkedForIssues) || APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal;
        if (GUILayout.Button("Build & Publish"))
        {
            if (APIUser.CurrentUser.canPublishWorlds)
            {
                EnvConfig.ConfigurePlayerSettings();
                VRC_SdkBuilder.shouldBuildUnityPackage = VRCSdkControlPanel.FutureProofPublishEnabled;
                VRC_SdkBuilder.PreBuildBehaviourPackaging();
                VRC_SdkBuilder.ExportAndUploadSceneBlueprint(customNamespace);
            }
            else 
            {
                ShowContentPublishPermissionsDialog();
            }
        }
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
        GUILayout.EndVertical();
    }

    void OnGUISceneLayer(int layer, string name, string description)
    {
        if (LayerMask.LayerToName(layer) != name)
            OnGUIError(null, "Layer " + layer + " must be renamed to '" + name + "'",
                delegate { EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics"); /*SettingsService.OpenProjectSettings("Project/Physics");*/ }, null);

        if (showLayerHelp)
            OnGUIInformation(null, "Layer " + layer + " " + name + "\n" + description);
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

    List<Transform> FindBonesBetween(Transform top, Transform bottom)
    {
        List<Transform> list = new List<Transform>();
        if (top == null || bottom == null) return list;
        Transform bt = top.parent;
        while (bt != bottom && bt != null)
        {
            list.Add(bt);
            bt = bt.parent;
        }
        return list;
    }

    bool AnalyzeIK(VRCSDK2.VRC_AvatarDescriptor ad, GameObject avObj, Animator anim)
    {
        bool hasHead = false;
        bool hasFeet = false;
        bool hasHands = false;
        bool hasThreeFingers = false;
        //bool hasToes = false;
        bool correctSpineHierarchy = false;
        bool correctArmHierarchy = false;
        bool correctLegHierarchy = false;

        bool status = true;

        Transform head = anim.GetBoneTransform(HumanBodyBones.Head);
        Transform lfoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        Transform rfoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        Transform lhand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        Transform rhand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        hasHead = null != head;
        hasFeet = (null != lfoot && null != rfoot);
        hasHands = (null != lhand && null != rhand);

        if (!hasHead || !hasFeet || !hasHands)
        {
            OnGUIError(ad, "Humanoid avatar must have head, hands and feet bones mapped.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            return false;
        }

        Transform lthumb = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        Transform lindex = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        Transform lmiddle = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        Transform rthumb = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        Transform rindex = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        Transform rmiddle = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);

        hasThreeFingers = null != lthumb && null != lindex && null != lmiddle && null != rthumb && null != rindex && null != rmiddle;

        if (!hasThreeFingers)
        {
            // although its only a warning, we return here because the rest
            // of the analysis is for VRIK
            OnGUIWarning(ad, "Thumb, Index, and Middle finger bones are not mapped, Full-Body IK will be disabled.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            status = false;
        }

        Transform pelvis = anim.GetBoneTransform(HumanBodyBones.Hips);
        Transform chest = anim.GetBoneTransform(HumanBodyBones.Chest);
        Transform upperchest = anim.GetBoneTransform(HumanBodyBones.UpperChest);
        Transform torso = anim.GetBoneTransform(HumanBodyBones.Spine);

        Transform neck = anim.GetBoneTransform(HumanBodyBones.Neck);
        Transform lclav = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform rclav = anim.GetBoneTransform(HumanBodyBones.RightShoulder);

        if (null == neck || null == lclav || null == rclav || null == pelvis || null == torso || null == chest)
        {
            OnGUIError(ad, "Spine hierarchy missing elements, make sure that Pelvis, Spine, Chest, Neck and Shoulders are mapped.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            return false;
        }

        if (null != upperchest)
            correctSpineHierarchy = lclav.parent == upperchest && rclav.parent == upperchest && neck.parent == upperchest;
        else
            correctSpineHierarchy = lclav.parent == chest && rclav.parent == chest && neck.parent == chest;

        if (!correctSpineHierarchy)
        {
            OnGUIError(ad, "Spine hierarchy incorrect. Make sure that the parent of both Shoulders and the Neck is the Chest (or UpperChest if set).", delegate () { Selection.activeObject = anim.gameObject; }, null);
            return false;
        }

        Transform lshoulder = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform lelbow = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform rshoulder = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform relbow = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);

        correctArmHierarchy = lshoulder.GetChild(0) == lelbow && lelbow.GetChild(0) == lhand &&
            rshoulder.GetChild(0) == relbow && relbow.GetChild(0) == rhand;

        if (!correctArmHierarchy)
        {
            OnGUIWarning(ad, "LowerArm is not first child of UpperArm or Hand is not first child of LowerArm: you may have problems with Forearm rotations.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            status = false;
        }

        Transform lhip = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform lknee = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Transform rhip = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform rknee = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);

        correctLegHierarchy = lhip.GetChild(0) == lknee && lknee.GetChild(0) == lfoot &&
            rhip.GetChild(0) == rknee && rknee.GetChild(0) == rfoot;

        if (!correctLegHierarchy)
        {
            OnGUIWarning(ad, "LowerLeg is not first child of UpperLeg or Foot is not first child of LowerLeg: you may have problems with Shin rotations.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            status = false;
        }

        if (!(IsAncestor(pelvis, rfoot) && IsAncestor(pelvis, lfoot) && IsAncestor(pelvis, lhand) || IsAncestor(pelvis, rhand) || IsAncestor(pelvis, lhand)))
        {
            OnGUIWarning(ad, "This avatar has a split heirarchy (Hips bone is not the ancestor of all humanoid bones). IK may not work correctly.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            status = false;
        }

        // if thigh bone rotations diverge from 180 from hip bone rotations, full-body tracking/ik does not work well
        Vector3 hipLocalUp = pelvis.InverseTransformVector(Vector3.up);
        Vector3 legLDir = lhip.TransformVector(hipLocalUp);
        Vector3 legRDir = rhip.TransformVector(hipLocalUp);
        float angL = Vector3.Angle(Vector3.up, legLDir);
        float angR = Vector3.Angle(Vector3.up, legRDir);
        if (angL < 175f || angR < 175f)
        {
            string angle = string.Format("{0:F1}", Mathf.Min(angL, angR));
            OnGUIWarning(ad, "The angle between pelvis and thigh bones should be close to 180 degrees (this avatar's angle is " + angle + "). Your avatar may not work well with full-body IK and Tracking.", delegate () { Selection.activeObject = anim.gameObject; }, null);
            status = false;
        }
        return status;
    }

    void ShowRestrictedComponents(IEnumerable<Component> componentsToRemove)
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (var c in componentsToRemove)
            gos.Add(c.gameObject);
        Selection.objects = gos.ToArray();
    }

    void FixRestrictedComponents(IEnumerable<Component> componentsToRemove)
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (var c in componentsToRemove)
        {
            gos.Add(c.gameObject);
            Destroy(c);
        }
    }

    void OnGUIAvatarCheck(VRCSDK2.VRC_AvatarDescriptor avatar)
    {
        string vrcFilePath = WWW.UnEscapeURL(UnityEditor.EditorPrefs.GetString("currentBuildingAssetBundlePath"));
        int fileSize = 0;
        if (!string.IsNullOrEmpty(vrcFilePath) && ValidationHelpers.CheckIfAssetBundleFileTooLarge(ContentType.Avatar, vrcFilePath, out fileSize))
        {
            OnGUIWarning(avatar, ValidationHelpers.GetAssetBundleOverSizeLimitMessageSDKWarning(ContentType.Avatar, fileSize), delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }

        AvatarPerformanceStats perfStats = new AvatarPerformanceStats();
        AvatarPerformance.CalculatePerformanceStats(avatar.Name, avatar.gameObject, perfStats);

        OnGUIPerformanceInfo(avatar, perfStats, AvatarPerformanceCategory.Overall);
        OnGUIPerformanceInfo(avatar, perfStats, AvatarPerformanceCategory.PolyCount);
        OnGUIPerformanceInfo(avatar, perfStats, AvatarPerformanceCategory.AABB);

        var eventHandler = avatar.GetComponentInChildren<VRCSDK2.VRC_EventHandler>();
        if (eventHandler != null)
        {
            OnGUIError(avatar, "This avatar contains an EventHandler, which is not currently supported in VRChat.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }

        if (avatar.lipSync == VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape && avatar.VisemeSkinnedMesh == null)
            OnGUIError(avatar, "This avatar uses Visemes but the Face Mesh is not specified.", delegate () { Selection.activeObject = avatar.gameObject; }, null);

        var anim = avatar.GetComponent<Animator>();
        if (anim == null)
        {
            OnGUIWarning(avatar, "This avatar does not contain an animator, and will not animate in VRChat.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }
        else if (anim.isHuman == false)
        {
            OnGUIWarning(avatar, "This avatar is not imported as a humanoid rig and will not play VRChat's provided animation set.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }
        else if (avatar.gameObject.activeInHierarchy == false)
        {
            OnGUIError(avatar, "Your avatar is disabled in the scene hierarchy!", delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }
        else
        {
            Transform foot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform shoulder = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            if (foot == null)
                OnGUIError(avatar, "Your avatar is humanoid, but it's feet aren't specified!", delegate () { Selection.activeObject = avatar.gameObject; }, null);
            if (shoulder == null)
                OnGUIError(avatar, "Your avatar is humanoid, but it's upper arms aren't specified!", delegate () { Selection.activeObject = avatar.gameObject; }, null);

            if (foot != null && shoulder != null)
            {
                Vector3 footPos = foot.position - avatar.transform.position;
                if (footPos.y < 0)
                    OnGUIWarning(avatar, "Avatar feet are beneath the avatar's origin (the floor). That's probably not what you want.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
                Vector3 shoulderPosition = shoulder.position - avatar.transform.position;
                if (shoulderPosition.y < 0.2f)
                    OnGUIError(avatar, "This avatar is too short. The minimum is 20cm shoulder height.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
                else if (shoulderPosition.y < 1.0f)
                    OnGUIWarning(avatar, "This avatar is short. This is probably shorter than you want.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
                else if (shoulderPosition.y > 5.0f)
                    OnGUIWarning(avatar, "This avatar is too tall. The maximum is 5m shoulder height.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
                else if (shoulderPosition.y > 2.5f)
                    OnGUIWarning(avatar, "This avatar is tall. This is probably taller than you want.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
            }

            if (AnalyzeIK(avatar, avatar.gameObject, anim) == false)
                OnGUILink(avatar, "See Avatar Rig Requirements for more information.", kAvatarRigRequirementsURL);
        }

        IEnumerable<Component> componentsToRemove = AvatarValidation.FindIllegalComponents(avatar.gameObject);
        HashSet<string> componentsToRemoveNames = new HashSet<string>();
        foreach (Component c in componentsToRemove)
        {
            if (componentsToRemoveNames.Contains(c.GetType().Name) == false)
                componentsToRemoveNames.Add(c.GetType().Name);
        }

        if (componentsToRemoveNames.Count > 0)
            OnGUIError(avatar, "The following component types are found on the Avatar and will be removed by the client: " + string.Join(", ", componentsToRemoveNames.ToArray()), delegate () { ShowRestrictedComponents(componentsToRemove); }, delegate () { FixRestrictedComponents(componentsToRemove); });

        if (AvatarValidation.EnforceAudioSourceLimits(avatar.gameObject).Count > 0)
            OnGUIWarning(avatar, "Audio sources found on Avatar, they will be adjusted to safe limits, if necessary.", delegate () { Selection.activeObject = avatar.gameObject; }, null);

        if (AvatarValidation.EnforceAvatarStationLimits(avatar.gameObject).Count > 0)
            OnGUIWarning(avatar, "Stations found on Avatar, they will be adjusted to safe limits, if necessary.", delegate () { Selection.activeObject = avatar.gameObject; }, null);

        if (avatar.gameObject.GetComponentInChildren<Camera>() != null)
            OnGUIWarning(avatar, "Cameras are removed from non-local avatars at runtime.", delegate () { Selection.activeObject = avatar.gameObject; }, null);

#if UNITY_ANDROID
        IEnumerable<Shader> illegalShaders = AvatarValidation.FindIllegalShaders(avatar.gameObject);
        foreach (Shader s in illegalShaders)
        {
            OnGUIError(avatar, "Avatar uses unsupported shader '" + s.name + "'. You can only use the shaders provided in 'VRChat/Mobile' for Quest avatars.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
        }
#endif

        foreach (AvatarPerformanceCategory perfCategory in System.Enum.GetValues(typeof(AvatarPerformanceCategory)))
        {
            if (perfCategory == AvatarPerformanceCategory.Overall ||
                perfCategory == AvatarPerformanceCategory.PolyCount ||
                perfCategory == AvatarPerformanceCategory.AABB ||
                perfCategory == AvatarPerformanceCategory.AvatarPerformanceCategoryCount)
            {
                continue;
            }

            OnGUIPerformanceInfo(avatar, perfStats, perfCategory);
        }

        OnGUILink(avatar, "Avatar Optimization Tips", kAvatarOptimizationTipsURL);
    }

    void OnGUIPerformanceInfo(VRCSDK2.VRC_AvatarDescriptor avatar, AvatarPerformanceStats perfStats, AvatarPerformanceCategory perfCategory)
    {
        string text;
        PerformanceInfoDisplayLevel displayLevel;
        PerformanceRating rating = perfStats.GetPerformanceRatingForCategory(perfCategory);
        SDKPerformanceDisplay.GetSDKPerformanceInfoText(perfStats, perfCategory, out text, out displayLevel);

        switch(displayLevel)
        {
            case PerformanceInfoDisplayLevel.None:
            {
                break;
            }
            case PerformanceInfoDisplayLevel.Verbose:
            {
                if(showAvatarPerformanceDetails)
                {
                    OnGUIStat(avatar, text, rating);
                }
                break;
            }
            case PerformanceInfoDisplayLevel.Info:
            {
                OnGUIStat(avatar, text, rating);
                break;
            }
            case PerformanceInfoDisplayLevel.Warning:
            {
                OnGUIStat(avatar, text, rating);
                break;
            }
            case PerformanceInfoDisplayLevel.Error:
            {
                OnGUIStat(avatar, text, rating);
                OnGUIError(avatar, text, delegate () { Selection.activeObject = avatar.gameObject; }, null);
                break;
            }
            default:
            {
                OnGUIError(avatar, "Unknown performance display level.", delegate () { Selection.activeObject = avatar.gameObject; }, null);
                break;
            }
        }
    }

    void OnGUIAvatar(VRCSDK2.VRC_AvatarDescriptor avatar)
    {
        EditorGUILayout.Space();

        //GUILayout.BeginVertical(boxGuiStyle, GUILayout.Width(SdkWindowWidth));

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.Space();
        GUILayout.Label("Online Publishing", infoGuiStyle);
        GUILayout.Label("In order for other people to see your avatar in VRChat it must be built and published to our game servers.", infoGuiStyle);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.Space();

        GUI.enabled = (GUIErrors.Count == 0 && checkedForIssues) || APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal;
        if (GUILayout.Button("Build & Publish"))
        {
            if (APIUser.CurrentUser.canPublishAvatars)
            {
                VRC_SdkBuilder.shouldBuildUnityPackage = VRCSdkControlPanel.FutureProofPublishEnabled;
                VRC_SdkBuilder.ExportAndUploadAvatarBlueprint(avatar.gameObject);
            }
            else
            {
                ShowContentPublishPermissionsDialog();
            }
        }
        GUI.enabled = true;
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        //GUILayout.EndVertical();
    }

    public static void ShowContentPublishPermissionsDialog()
    {
        if (!RemoteConfig.IsInitialized())
        {
            RemoteConfig.Init(() => ShowContentPublishPermissionsDialog());
            return;
        }

        string message = RemoteConfig.GetString("sdkNotAllowedToPublishMessage");
        int result = UnityEditor.EditorUtility.DisplayDialogComplex("VRChat SDK", message, "Developer FAQ", "VRChat Discord", "OK");
        if (result == 0)
        {
            ShowDeveloperFAQ();
        }
        if (result == 1)
        {
            ShowVRChatDiscord();
        }
    }
}
