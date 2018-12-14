#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRC.Core;

public class VRCContentManagerWindow : EditorWindow
{
	static bool isDevApi;
	static VRCContentManagerWindow window;

    [MenuItem("VRChat SDK/Manage Uploaded Content")]
    static void Init()
    {
		    window = (VRCContentManagerWindow)EditorWindow.GetWindow(typeof(VRCContentManagerWindow));
        window.titleContent.text = "VRChat Content Manager";
        window.Show();

        // window.FetchWorldData();
        isDevApi = ApiModel.IsDevApi();
    }

	void OnFocus()
	{
		if(isDevApi != ApiModel.IsDevApi())
		{
			FetchWorldData();	
			isDevApi = !isDevApi;
		}
		 else
		 {
		 	if(uploadedWorlds == null || uploadedAvatars == null)
		 		FetchWorldData();
		 }
	}

    string username;
    string password;

	static List<ApiWorld> uploadedWorlds;
	static Dictionary<string, Texture2D> worldImages;

	static List<ApiAvatar> uploadedAvatars;
	static Dictionary<string, Texture2D> avatarImages;

  	static List<string> justDeletedContents;

	public static void ClearContent()
	{
		uploadedWorlds = null;
		uploadedAvatars = null;
	}

	void FetchWorldData()
	{
		if (!RemoteConfig.IsInitialized())
			RemoteConfig.Init();

		if (!APIUser.IsLoggedInWithCredentials && APIUser.IsCached)
			APIUser.CachedLogin(null, null, false);

		if(RemoteConfig.IsInitialized() && APIUser.IsLoggedInWithCredentials)
		{
			ApiWorld.FetchList(
				delegate(List<ApiWorld> obj) {
					SetupWorldData(obj);
				},
				delegate(string obj) {
					Debug.LogError("Error fetching your uploaded worlds");
				},
				ApiWorld.SortHeading.Updated,
				ApiWorld.SortOwnership.Mine,
				ApiWorld.SortOrder.Descending,
				0,
				100,
        		"",
				null,
        		"",
        		ApiWorld.ReleaseStatus.All
			);

			ApiAvatar.FetchList(
				delegate(List<ApiAvatar> obj) {
					SetupAvatarData(obj);	
				},
				delegate(string obj) {
					Debug.LogError("Error fetching your uploaded avatars");
				},
				ApiAvatar.Owner.Mine,
				null,
				100
			);
		}
	}

	void SetupWorldData(List<ApiWorld> worlds)
	{
		uploadedWorlds = worlds;
		uploadedWorlds.Sort( (w1,w2)=>w1.name.CompareTo(w2.name) );
		EditorCoroutine.start(DownloadAndRenderWorldImages());
	}

	void SetupAvatarData(List<ApiAvatar> avatars)
	{
		uploadedAvatars = avatars;
		uploadedAvatars.Sort( (w1,w2)=>w1.name.CompareTo(w2.name) );
		EditorCoroutine.start(DownloadAndRenderAvatarImages());
	}

	IEnumerator DownloadAndRenderWorldImages()
	{
		Dictionary<string, Texture2D> imageDict = new Dictionary<string, Texture2D>();
		foreach(ApiWorld w in uploadedWorlds)
		{
			yield return EditorCoroutine.start(DownloadImageIntoImageDict(w.id, w.thumbnailImageUrl, 
				delegate(Dictionary<string, Texture2D> updatedImageDict) {
					imageDict = updatedImageDict;
				}, 
				imageDict
			));
		}

		worldImages = imageDict;
	}

	IEnumerator DownloadAndRenderAvatarImages()
	{
		Dictionary<string, Texture2D> imageDict = new Dictionary<string, Texture2D>();
		foreach(ApiAvatar a in uploadedAvatars)
		{
			yield return EditorCoroutine.start(DownloadImageIntoImageDict(a.id, a.imageUrl, 
				delegate(Dictionary<string, Texture2D> updatedImageDict) {
					imageDict = updatedImageDict;
				}, 
				imageDict
			));
		}

		avatarImages = imageDict;
	}

	IEnumerator DownloadImageIntoImageDict(string imageId, string imageUrl, System.Action<Dictionary<string, Texture2D>> onImageDownloaded, Dictionary<string, Texture2D> imageDict)
	{
		WWW www = new WWW(imageUrl);
		while(!www.isDone)
			yield return null;
	
		imageDict[imageId] = www.texture;
		if(onImageDownloaded != null)
			onImageDownloaded(imageDict);
	}

	Vector2 scrollPos;
	string t = "This is a string inside a Scroll view!";

    bool OnGUIUserInfo()
    {
        if (!RemoteConfig.IsInitialized())
            RemoteConfig.Init();

        if (!APIUser.IsLoggedInWithCredentials && APIUser.IsCached)
            APIUser.CachedLogin(null, null, false);

        GUILayout.Label("Account Info", EditorStyles.boldLabel);
        if (APIUser.IsLoggedInWithCredentials)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Logged in as " + APIUser.CurrentUser.displayName);
            if (GUILayout.Button("Logout"))
            {
                APIUser.Logout();
                GUILayout.EndHorizontal();
                return false;
            }
            else
            {
                GUILayout.EndHorizontal();
                GUILayout.Label("Developer Status: " + APIUser.CurrentUser.developerType );

				if(APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
                {
                    string apiMessage = ApiModel.IsDevApi() ? "Dev" : "Release";
                    GUILayout.Label("API: " + apiMessage);
                }


				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				EditorGUILayout.Space();

				if(uploadedWorlds == null)
				{
					EditorGUILayout.EndScrollView();
					return true;
				}

				EditorGUILayout.LabelField("Worlds", EditorStyles.boldLabel);
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width (200));
				EditorGUILayout.LabelField("Image", EditorStyles.boldLabel, GUILayout.Width (75));
				EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width (50));
				//EditorGUILayout.LabelField("Version", EditorStyles.boldLabel, GUILayout.Width (75));
				EditorGUILayout.LabelField("Release Status", EditorStyles.boldLabel, GUILayout.Width (100));
				EditorGUILayout.EndHorizontal();

				List<ApiWorld> tmpWorlds = new List<ApiWorld>();

				if(uploadedWorlds != null)
					tmpWorlds = new List<ApiWorld>(uploadedWorlds);
				
				foreach(ApiWorld w in tmpWorlds)
                {
					if(justDeletedContents != null && justDeletedContents.Contains(w.id)) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(w.name, GUILayout.Width(200));
                    if (worldImages != null && worldImages.ContainsKey(w.id))
                    {
                        if (GUILayout.Button(worldImages[w.id], GUILayout.Height(100), GUILayout.Width(100)))
                        {
                            Application.OpenURL(w.imageUrl);
                        }
                    }
                    else
                    {
                        GUILayout.Label("No Image", GUILayout.Height(100), GUILayout.Width(100));
                    }
                    EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(50));
                    EditorGUILayout.LabelField(w.releaseStatus, GUILayout.Width(100));
                    EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width (50));
                    if (GUILayout.Button("Copy ID", GUILayout.Width(75)))
                    {
		                TextEditor te = new TextEditor();
		                te.text = w.id;
		                te.SelectAll();
		                te.Copy();
                    }
                    if (GUILayout.Button("Delete", GUILayout.Width(75)))
                    {
                        if (EditorUtility.DisplayDialog("Delete " + w.name + "?", "Are you sure you want to delete " + w.name + "? This cannot be undone.", "Delete", "Cancel"))
                        {
                            ApiWorld.Delete(w.id, null, null);
                            uploadedWorlds.RemoveAll(world => world.id == w.id);

                            if(justDeletedContents == null) justDeletedContents = new List<string>();
                            justDeletedContents.Add(w.id);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

        EditorGUILayout.Space();
				EditorGUILayout.LabelField("Avatars", EditorStyles.boldLabel);
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(200));
				EditorGUILayout.LabelField("Image", EditorStyles.boldLabel, GUILayout.Width(75));
				EditorGUILayout.EndHorizontal();

				List<ApiAvatar> tmpAvatars = new List<ApiAvatar>();

				if(uploadedAvatars != null)
					tmpAvatars = new List<ApiAvatar>(uploadedAvatars);
				
				foreach(ApiAvatar a in tmpAvatars)
				{
                    if(justDeletedContents != null && justDeletedContents.Contains(a.id)) continue;

          					EditorGUILayout.BeginHorizontal();
          					EditorGUILayout.LabelField(a.name, GUILayout.Width(200));
                    if (avatarImages != null && avatarImages.ContainsKey(a.id))
                    {
                        if (GUILayout.Button(avatarImages[a.id], GUILayout.Height(100), GUILayout.Width(100)))
                        {
                            Application.OpenURL(a.imageUrl);
                        }
                    }
                    else
                    {
                        GUILayout.Label("No Image", GUILayout.Height(100), GUILayout.Width(100));
                    }
                    EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width (50));

                	if (GUILayout.Button("Copy ID", GUILayout.Width(75)))
                    {
		                TextEditor te = new TextEditor();
		                te.text = a.id;
		                te.SelectAll();
		                te.Copy();
                    }

					if(GUILayout.Button("Delete", GUILayout.Width (75)))
					{
                        if (EditorUtility.DisplayDialog("Delete " + a.name + "?", "Are you sure you want to delete " + a.name + "? This cannot be undone.", "Delete", "Cancel"))
                        {
                            ApiAvatar.Delete(a.id, null, null);
                            uploadedAvatars.RemoveAll(avatar => avatar.id == a.id);

							if(justDeletedContents == null) justDeletedContents = new List<string>();
                            justDeletedContents.Add(a.id);
                        }
                    }
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndScrollView();

                return true;
            }
        }
        else
        {
            VRC.Core.ApiModel.ResetApi();

            username = EditorGUILayout.TextField("Username", username);
            password = EditorGUILayout.PasswordField("Password", password);

            if (GUILayout.Button("Sign In"))
            {
                APIUser.Login(username, password,
                    delegate (APIUser user)
                    {
                        UnityEditor.EditorUtility.ClearProgressBar();
						FetchWorldData();
                    },
                    delegate (string message)
                    {
                        VRC.Core.Logger.Log("Error logging in - " + message);
                        UnityEditor.EditorUtility.ClearProgressBar();
                        UnityEditor.EditorUtility.DisplayDialog("Error logging in", message, "Okay");
                    }
                );
                UnityEditor.EditorUtility.DisplayProgressBar("Logging in!", "Hang tight...", 0.5f);
            }
            if (GUILayout.Button("Sign up"))
                Application.OpenURL("http://www.vrchat.net/auth/register");
            return false;
        }
    }


    void OnGUI()
    {
        if (window == null)
			window = (VRCContentManagerWindow)EditorWindow.GetWindow(typeof(VRCContentManagerWindow));

		OnGUIUserInfo();

        window.Repaint();
    }
}
#endif
