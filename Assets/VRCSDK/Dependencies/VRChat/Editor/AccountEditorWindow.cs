using UnityEngine;
using UnityEditor;
using VRC.Core;

namespace VRC
{
    [ExecuteInEditMode]
    public class AccountEditorWindow : EditorWindow 
    {
        static bool isInitialized = false;
        static string clientInstallPath;
        static bool signingIn = false;
        static string error = null;

        static AccountEditorWindow window = null;

        public static bool FutureProofPublishEnabled { get { return UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled); } }
        public static bool DefaultFutureProofPublishEnabled { get { return !SDKClientUtilities.IsInternalSDK();  } }
        
        void Update()
        {
            SignIn(false);
        }

        static string storedUsername
        {
            get
            {
                if (EditorPrefs.HasKey("sdk#username"))
                    return EditorPrefs.GetString("sdk#username");
                return null;
            }
            set
            {
                EditorPrefs.SetString("sdk#username", value);
                if (string.IsNullOrEmpty(value))
                    EditorPrefs.DeleteKey("sdk#username");
            }
        }

        static string storedPassword
        {
            get
            {
                if (EditorPrefs.HasKey("sdk#password"))
                    return EditorPrefs.GetString("sdk#password");
                return null;
            }
            set
            {
                EditorPrefs.SetString("sdk#password", value);
                if (string.IsNullOrEmpty(value))
                    EditorPrefs.DeleteKey("sdk#password");
            }
        }

        static string _username = null;
        static string _password = null;

        static string username
        {
            get
            {
                if (!string.IsNullOrEmpty(_username))
                    return _username;
                else
                    _username = storedUsername;
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        static string password
        {
            get
            {
                if (!string.IsNullOrEmpty(_password))
                    return _password;
                else
                    _password = storedPassword;
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        static bool UseDevApi
        {
            get
            {
                return VRC.Core.ApiModel.GetApiUrl() == ApiModel.devApiUrl;
            }
        }

        [MenuItem("VRChat SDK/Settings")]
        public static void CreateWindow()
        {
            Init();
            window = EditorWindow.GetWindow<AccountEditorWindow>("VRChat Settings");
            window.Show();
        }

        public static void Init () 
        {
            if (!RemoteConfig.IsInitialized())
                RemoteConfig.Init();

            if (isInitialized)
                return;

			if(!APIUser.IsLoggedInWithCredentials && ApiCredentials.Load() )
            {
				APIUser.Login( null, null );
            }

            clientInstallPath = SDKClientUtilities.GetSavedVRCInstallPath();
            if(string.IsNullOrEmpty(clientInstallPath))
                clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();

            signingIn = false;
			isInitialized = true;
        }

        static void OnVRCInstallPathGUI()
        {
            EditorGUILayout.LabelField("VRChat Client", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Installed Client Path: ", clientInstallPath);
            if(GUILayout.Button("Edit"))
            {
                string initPath = "";
                if(!string.IsNullOrEmpty(clientInstallPath))
                    initPath = clientInstallPath;

                clientInstallPath = EditorUtility.OpenFilePanel("Choose VRC Client Exe", initPath, "exe");
                SDKClientUtilities.SetVRCInstallPath(clientInstallPath);
            }
            if(GUILayout.Button("Revert to Default"))
            {
                clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();
            }

        }

        public static bool OnShowStatus()
        {
            SignIn(false);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(Status);

            if (APIUser.IsLoggedInWithCredentials)
            {
                EditorGUILayout.PrefixLabel("Logged in as " + APIUser.CurrentUser.displayName);
                EditorGUILayout.LabelField("Developer Status: " + APIUser.CurrentUser.developerType);
            }

            EditorGUILayout.EndVertical();

            return APIUser.IsLoggedInWithCredentials;
        }

        static bool OnAccountGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(Status);

            EditorGUILayout.LabelField("Account", EditorStyles.boldLabel);

            if (signingIn)
            {
                EditorGUILayout.LabelField("Signing in.");
                EditorGUILayout.EndVertical();
                return false;
            }
            else if (APIUser.IsLoggedInWithCredentials)
            {
                EditorGUILayout.PrefixLabel("Logged in as " + APIUser.CurrentUser.displayName);
                EditorGUILayout.LabelField("Developer Status: " + APIUser.CurrentUser.developerType);

                if (GUILayout.Button("Logout"))
                {
                    storedUsername = username = null;
                    storedPassword = password = null;

                    APIUser.Logout();
                }
            }
            else
            {
                if (signingIn)
                    EditorGUILayout.LabelField("Signing in.");
                else
                {
                    Init();
                    
                    username = EditorGUILayout.TextField("Username", username);
                    password = EditorGUILayout.PasswordField("Password", password);

                    if (GUILayout.Button("Sign In"))
                        SignIn(true);
                    if (GUILayout.Button("Sign up"))
                        Application.OpenURL("http://vrchat.com/register");
                }
            }

            //if (IsInternal)
            {
                if (APIUser.CurrentUser == null || APIUser.CurrentUser.developerType > APIUser.DeveloperType.Trusted)
                {
                    EditorGUILayout.LabelField("API", EditorStyles.boldLabel);
                    bool useDevApi = UseDevApi;
                    bool newUseDevApi = EditorGUILayout.Toggle("Use Dev API", useDevApi);

                    if (newUseDevApi != useDevApi)
                    {
                        if (newUseDevApi)
                            ApiModel.SetApiUrl(ApiModel.devApiUrl);
                        else
                            ApiModel.SetApiUrl(ApiModel.releaseApiUrl);
                    }

                    if (APIUser.CurrentUser == null)
                    {
                        EditorGUILayout.EndVertical();
                        return false;
                    }
                }
            }

            // Future proof upload
            {
                EditorGUILayout.LabelField("Publish", EditorStyles.boldLabel);
                bool futureProofPublish = UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled);

                futureProofPublish = EditorGUILayout.Toggle("Future Proof Publish", futureProofPublish);

                if (UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled) != futureProofPublish)
                {
                    UnityEditor.EditorPrefs.SetBool("futureProofPublish", futureProofPublish);
                }
            }

            if (APIUser.CurrentUser != null)
            {
                // custom vrchat install location
                OnVRCInstallPathGUI();

                // custom api endpoint field
                if (APIUser.CurrentUser.developerType <= APIUser.DeveloperType.Trusted && UseDevApi)
                    ApiModel.SetApiUrl(ApiModel.releaseApiUrl);
            }

            EditorGUILayout.EndVertical();

            return true;
        }

        void OnGUI ()
        {
            if (VRC.Core.RemoteConfig.IsInitialized())
            {
                EditorGUILayout.LabelField("Unity Version", EditorStyles.boldLabel);
                if (VRC.Core.RemoteConfig.HasKey("sdkUnityVersion"))
                {
                    string sdkUnityVersion = VRC.Core.RemoteConfig.GetString("sdkUnityVersion");
                    if (string.IsNullOrEmpty(sdkUnityVersion))
                        EditorGUILayout.LabelField("Could not fetch remote config.");
                    else if (Application.unityVersion != sdkUnityVersion)
                        EditorGUILayout.LabelField("Wrong Unity version. Please use " + sdkUnityVersion);
                    else
                        EditorGUILayout.LabelField("You are using the correct Unity version: " + sdkUnityVersion);
                }
            }
            else if (VRC.Core.RemoteConfig.HasCachedConfig())
            {
                VRC.Core.RemoteConfig.Init(false);
            }

            OnAccountGUI();
        }

        private static string Status
        {
            get
            {
                if (!APIUser.IsLoggedInWithCredentials)
                    return error == null ? "Use the settings menu to log in." : "Error in authenticating: " + error;
                if (signingIn)  
                    return "Logging in.";
                else
                    return "Connected to " + (UseDevApi ? "Dev" : "Prod");
            }
        }

        private static object syncObject = new object();
        private static void SignIn(bool explicitAttempt)
        {
            lock (syncObject)
            {
                if (signingIn
                    || APIUser.IsLoggedInWithCredentials
                    || (!explicitAttempt && string.IsNullOrEmpty(storedUsername))
                    || (!explicitAttempt && string.IsNullOrEmpty(storedPassword)))
                    return;

                signingIn = true;
            }

            Init();

            ApiCredentials.Clear();
            ApiCredentials.SetUser(username, password);
            APIUser.Login(
                delegate (APIUser user)
                {
                    signingIn = false;
                    error = null;
                    storedUsername = username;
                    storedPassword = password;
                },
                delegate (string message)
                {
                    signingIn = false;
                    storedUsername = null;
                    storedPassword = null;
                    error = message;
                    APIUser.Logout();
                    VRC.Core.Logger.Log("Error logging in: " + message);
                }
            );
        }

        private void OnDestroy()
        {
            signingIn = false;
            isInitialized = false;
        }
    }
}
