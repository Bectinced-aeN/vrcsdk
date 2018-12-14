using UnityEditor;
using UnityEngine;
using VRC.Core;

namespace VRC
{
	public class AccountEditorWindow : EditorWindow
	{
		private static bool isInitialized;

		private string username;

		private string password;

		private static string apiEndpoint;

		private static string clientInstallPath;

		private static bool initUseDevApi;

		private static bool useDevApi;

		private static EditorWindow win;

		public AccountEditorWindow()
			: this()
		{
		}

		[MenuItem("VRChat SDK/Settings")]
		public static void Init()
		{
			win = EditorWindow.GetWindow(typeof(AccountEditorWindow));
			if (!APIUser.IsLoggedInWithCredentials && APIUser.IsCached)
			{
				APIUser.CachedLogin(null, null, shouldFetch: false);
			}
			apiEndpoint = ApiModel.GetApiUrl();
			useDevApi = (apiEndpoint == "https://dev-api.vrchat.cloud/api/1/");
			initUseDevApi = useDevApi;
			clientInstallPath = SDKClientUtilities.GetSavedVRCInstallPath();
			if (string.IsNullOrEmpty(clientInstallPath))
			{
				clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();
			}
			isInitialized = true;
		}

		private void OnVRCInstallPathGUI()
		{
			GUILayout.Label("VRChat Client", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Installed Client Path: ", clientInstallPath, (GUILayoutOption[])new GUILayoutOption[0]);
			if (GUILayout.Button("Edit", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				string empty = string.Empty;
				if (!string.IsNullOrEmpty(clientInstallPath))
				{
					empty = clientInstallPath;
				}
				clientInstallPath = EditorUtility.OpenFilePanel("Choose VRC Client Exe", empty, "exe");
				SDKClientUtilities.SetVRCInstallPath(clientInstallPath);
			}
			if (GUILayout.Button("Revert to Default", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();
			}
		}

		private void OnGUI()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (!isInitialized)
			{
				Init();
			}
			win.set_titleContent(new GUIContent("Settings"));
			GUILayout.Label("Account", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
			if (APIUser.IsLoggedInWithCredentials)
			{
				EditorGUILayout.LabelField("Logged in as " + APIUser.CurrentUser.displayName, (GUILayoutOption[])new GUILayoutOption[0]);
				if (GUILayout.Button("Logout", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					APIUser.Logout();
					win.Repaint();
				}
				if (APIUser.CurrentUser != null)
				{
					OnVRCInstallPathGUI();
					if (APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
					{
						GUILayout.Label("API", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
						useDevApi = EditorGUILayout.Toggle("Use Dev API", useDevApi, (GUILayoutOption[])new GUILayoutOption[0]);
						if (initUseDevApi != useDevApi)
						{
							if (useDevApi)
							{
								ApiModel.SetApiUrl("https://dev-api.vrchat.cloud/api/1/");
							}
							else
							{
								ApiModel.SetApiUrl("https://api.vrchat.cloud/api/1/");
							}
							initUseDevApi = useDevApi;
						}
					}
					else
					{
						ApiModel.ResetApi();
					}
				}
			}
			else
			{
				ApiModel.ResetApi();
				username = EditorGUILayout.TextField("Username", username, (GUILayoutOption[])new GUILayoutOption[0]);
				password = EditorGUILayout.PasswordField("Password", password, (GUILayoutOption[])new GUILayoutOption[0]);
				if (GUILayout.Button("Sign In", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					APIUser.Login(username, password, delegate
					{
						win.Repaint();
						EditorUtility.ClearProgressBar();
					}, delegate(string message)
					{
						Logger.Log("Error logging in - " + message);
						EditorUtility.ClearProgressBar();
						EditorUtility.DisplayDialog("Error logging in", message, "Okay");
					});
					EditorUtility.DisplayProgressBar("Logging in!", "Hang tight...", 0.5f);
				}
				if (GUILayout.Button("Sign up", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					Application.OpenURL("http://www.vrchat.net/auth/register");
				}
			}
			if (RemoteConfig.IsInitialized())
			{
				GUILayout.Label("Unity Version", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
				if (RemoteConfig.HasKey("sdkUnityVersion"))
				{
					string @string = RemoteConfig.GetString("sdkUnityVersion");
					if (Application.get_unityVersion() != @string)
					{
						EditorGUILayout.LabelField("Wrong Unity version. Please use " + @string, (GUILayoutOption[])new GUILayoutOption[0]);
					}
					else
					{
						EditorGUILayout.LabelField("You are using the correct Unity version: " + @string, (GUILayoutOption[])new GUILayoutOption[0]);
					}
				}
			}
			else if (RemoteConfig.HasCachedConfig())
			{
				RemoteConfig.Init(fetchFreshConfig: false);
			}
		}
	}
}
