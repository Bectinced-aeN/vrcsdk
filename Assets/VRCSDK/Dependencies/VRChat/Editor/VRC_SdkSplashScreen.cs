#define COMMUNITY_LABS_SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace VRCSDK2
{
    [InitializeOnLoad]
    public class VRC_SdkSplashScreen : EditorWindow
    {

        static VRC_SdkSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            EditorApplication.update += DoSplashScreen;
        }

        private static void DoSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            if (!EditorPrefs.HasKey("VRCSDK_ShowSplashScreen"))
            {
                EditorPrefs.SetBool("VRCSDK_ShowSplashScreen", true);
            }
            if (EditorPrefs.GetBool("VRCSDK_ShowSplashScreen"))
                OpenSplashScreen();
        }

        private static GUIStyle vrcSdkHeader;
        private static GUIStyle vrcSdkBottomHeader;
        private static GUIStyle vrcHeaderLearnMoreButton;
        private static GUIStyle vrcBottomHeaderLearnMoreButton;
        private static Vector2 changeLogScroll;
        [MenuItem("VRChat SDK/Splash Screen", false, 500)]
        public static void OpenSplashScreen()
        {
            GetWindow<VRC_SdkSplashScreen>(true);
        }
        
        public static void Open()
        {
            OpenSplashScreen();
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("VRChat SDK");

            maxSize = new Vector2(400, 600);
            minSize = maxSize;

            vrcSdkHeader = new GUIStyle
            {
                normal =
                    {
#if COMMUNITY_LABS_SDK
                        background = Resources.Load("vrcSdkHeaderWithCommunityLabs") as Texture2D,
#else
                        background = Resources.Load("vrcSdkHeader") as Texture2D,
#endif
                        textColor = Color.white
                    },
                fixedHeight = 200
            };

            vrcSdkBottomHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("vrcSdkBottomHeader") as Texture2D,

                    textColor = Color.white
                },
                fixedHeight = 100
            };

        }

        public void OnGUI()
        {
            GUILayout.Box("", vrcSdkHeader);
#if COMMUNITY_LABS_SDK
            vrcHeaderLearnMoreButton = EditorStyles.miniButton;
            vrcHeaderLearnMoreButton.normal.textColor = Color.black;
            vrcHeaderLearnMoreButton.fontSize = 12;
            vrcHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            Texture2D texture = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/UISprite.psd");
            vrcHeaderLearnMoreButton.normal.background = texture;
            vrcHeaderLearnMoreButton.active.background = texture;
            if (GUI.Button(new Rect(20, 140, 180, 40), "Please Read", vrcHeaderLearnMoreButton))
            {
                Application.OpenURL(CommunityLabsConstants.COMMUNITY_LABS_DOCUMENTATION_URL);
            }
#endif
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("SDK Docs"))
            {
                Application.OpenURL("https://docs.vrchat.com/");
            }
            if (GUILayout.Button("VRChat FAQ"))
            {
                Application.OpenURL("https://vrchat.com/developer-faq");
            }
            if (GUILayout.Button("Help Center"))
            {
                Application.OpenURL("http://help.vrchat.com");
            }
            if(GUILayout.Button("Examples"))
            {
                Application.OpenURL("https://docs.vrchat.com/docs/vrchat-kits");
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Building VRChat Quest Content"))
            {
                Application.OpenURL("https://docs.vrchat.com/docs/creating-content-for-the-oculus-quest");
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            changeLogScroll = GUILayout.BeginScrollView(changeLogScroll, GUILayout.Width(390));

            GUILayout.Label(
    @"Changelog:
2019.1.4p4
- Fixed an issue causing redundant error messaging when 
informing the user that there are objects that share the 
same path

2019.1.4
- Some additional tooltips in VRC_Mirror inspector for 
clarity
- Added a field for a custom shader on mirrors, allowing 
the mirror shader to be overridden without the need to 
swap materials 
using an animator
- Added a drop-down option for mirrors allowing for the 
setting of lower fixed resolutions

2019.1.3
- Implemented features to enable usage of Community Labs
    - Added Community Labs checkbox in the Publish World
screen
- Publishing a world no longer changes its release status.
If you update a Public World, it now remains public
- Changed and updated SDK UI in some places
- Mirrors should now display properly in the editor as 
we've moved the necessary shader into place

2019.1.1
- Content Manager now behaves better when resizing 
the window and scales the contents appropriately"
            );
            GUILayout.EndScrollView();

            GUILayout.Space(4);

            GUILayout.Box("", vrcSdkBottomHeader);
            vrcBottomHeaderLearnMoreButton = EditorStyles.miniButton;
            vrcBottomHeaderLearnMoreButton.normal.textColor = Color.black;
            vrcBottomHeaderLearnMoreButton.fontSize = 10;
            vrcBottomHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            vrcBottomHeaderLearnMoreButton.normal.background = texture;
            vrcBottomHeaderLearnMoreButton.active.background = texture;
            if (GUI.Button(new Rect(80, 540, 240, 30), "Learn how to create for VRChat Quest!", vrcBottomHeaderLearnMoreButton))
            {
                Application.OpenURL("https://docs.vrchat.com/docs/creating-content-for-the-oculus-quest");
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            EditorPrefs.SetBool("VRCSDK_ShowSplashScreen", GUILayout.Toggle(EditorPrefs.GetBool("VRCSDK_ShowSplashScreen"), "Show at Startup"));

            GUILayout.EndHorizontal();
        }

    }
}