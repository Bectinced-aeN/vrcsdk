#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRC.Core;

[ExecuteInEditMode]
public class VRCContentManagerWindow : EditorWindow
{
    const int PageLimit = 20;

    bool UseDevApi
    {
        get
        {
            return VRC.Core.ApiModel.GetApiUrl() == ApiModel.devApiUrl;
        }
    }

    void Update()
    {
        Repaint();
    }

    static VRCContentManagerWindow window;

    [MenuItem("VRChat SDK/Manage Uploaded Content")]
    static void Init()
    {
        window = (VRCContentManagerWindow)EditorWindow.GetWindow(typeof(VRCContentManagerWindow));
        window.titleContent.text = "VRChat Content Manager";
        window.Show();
    }

    bool CheckLogin()
    {
        return APIUser.IsLoggedInWithCredentials;
    }

    void OnFocus()
    {
        if (!CheckLogin())
            return;

        EditorCoroutine.start(FetchUploadedData());
    }

    [UnityEditor.Callbacks.PostProcessScene]
    static void OnPostProcessScene()
    {
        if (window != null)
            EditorCoroutine.start(window.FetchUploadedData());
    }

    static List<ApiWorld> uploadedWorlds;
    static Dictionary<string, Texture2D> worldImages;

    static List<ApiAvatar> uploadedAvatars;
    static Dictionary<string, Texture2D> avatarImages;

    static List<string> justDeletedContents;
    static List<ApiAvatar> justUpdatedAvatars;

    public static void ClearContent()
    {
        uploadedWorlds = null;
        uploadedAvatars = null;
    }

    IEnumerator FetchUploadedData()
    {
        if (!RemoteConfig.IsInitialized())
            RemoteConfig.Init();

        if (CheckLogin() == false)
            yield break;

        ApiModel.ClearReponseCache();

        int indexPosition = 0;
        int lastResponseCount = 0;
        bool requestActive = false;

        do
        {
            requestActive = true;
            ApiWorld.FetchList(
                delegate (List<ApiWorld> obj)
                {
                    lastResponseCount = obj.Count;
                    indexPosition += obj.Count;
                    requestActive = false;
                    SetupWorldData(obj);
                },
                delegate (string obj)
                {
                    lastResponseCount = 0;
                    requestActive = false;
                    Debug.LogError("Error fetching your uploaded worlds:\n" + obj);
                    SetupWorldData(new List<ApiWorld>());
                },
                ApiWorld.SortHeading.Updated,
                ApiWorld.SortOwnership.Mine,
                ApiWorld.SortOrder.Descending,
                indexPosition,
                PageLimit,
                "",
                null,
                null,
                "",
                ApiWorld.ReleaseStatus.All,
                false,
                true
            );
            yield return new WaitUntil(() => !requestActive);
        } while (lastResponseCount > 0);

        indexPosition = 0;

        do
        {
            requestActive = true;
            ApiAvatar.FetchList(
                delegate (List<ApiAvatar> obj)
                {
                    lastResponseCount = obj.Count;
                    indexPosition += obj.Count;
                    requestActive = false;
                    SetupAvatarData(obj);
                },
                delegate (string obj)
                {
                    lastResponseCount = 0;
                    requestActive = false;
                    Debug.LogError("Error fetching your uploaded avatars:\n" + obj);
                    SetupAvatarData(new List<ApiAvatar>());
                },
                ApiAvatar.Owner.Mine,
                ApiAvatar.ReleaseStatus.All,
                null,
                PageLimit,
                indexPosition,
                ApiAvatar.SortHeading.None,
                ApiAvatar.SortOrder.Descending,
                false,
                true
            );
            yield return new WaitUntil(() => !requestActive);
        } while (lastResponseCount > 0);
    }

    void SetupWorldData(List<ApiWorld> worlds)
    {
        worlds.RemoveAll(w => w == null || w.name == null);

        uploadedWorlds = worlds;
        uploadedWorlds.Sort((w1, w2) => w1.name.CompareTo(w2.name));
        EditorCoroutine.start(DownloadAndRenderWorldImages());
    }

    void SetupAvatarData(List<ApiAvatar> avatars)
    {
        avatars.RemoveAll(a => a == null || a.name == null);

        uploadedAvatars = avatars;
        uploadedAvatars.Sort((w1, w2) => w1.name.CompareTo(w2.name));
        EditorCoroutine.start(DownloadAndRenderAvatarImages());
    }

    IEnumerator DownloadAndRenderWorldImages()
    {
        Dictionary<string, Texture2D> imageDict = new Dictionary<string, Texture2D>();
        foreach (ApiWorld w in uploadedWorlds)
        {
            yield return EditorCoroutine.start(DownloadImageIntoImageDict(w.id, w.thumbnailImageUrl,
                delegate (Dictionary<string, Texture2D> updatedImageDict)
                {
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
        foreach (ApiAvatar a in uploadedAvatars)
        {
            yield return EditorCoroutine.start(DownloadImageIntoImageDict(a.id, a.imageUrl,
                delegate (Dictionary<string, Texture2D> updatedImageDict)
                {
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
        www.threadPriority = ThreadPriority.Low;
        while (!www.isDone)
            yield return null;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error in received data: " + www.error);
            yield break;
        }

        try
        {
            imageDict[imageId] = www.texture;
            if (onImageDownloaded != null)
                onImageDownloaded(imageDict);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    Vector2 scrollPos;
    
    bool OnGUIUserInfo()
    {
        if (!RemoteConfig.IsInitialized())
            RemoteConfig.Init();

        CheckLogin();

        if (APIUser.IsLoggedInWithCredentials)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.Space();

            if (uploadedWorlds == null)
            {
                EditorGUILayout.EndScrollView();
                return true;
            }

            EditorGUILayout.LabelField("Worlds", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Image", EditorStyles.boldLabel, GUILayout.Width(75));
            EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(50));
            //EditorGUILayout.LabelField("Version", EditorStyles.boldLabel, GUILayout.Width (75));
            EditorGUILayout.LabelField("Release Status", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            List<ApiWorld> tmpWorlds = new List<ApiWorld>();

            if (uploadedWorlds != null)
                tmpWorlds = new List<ApiWorld>(uploadedWorlds);

            foreach (ApiWorld w in tmpWorlds)
            {
                if (justDeletedContents != null && justDeletedContents.Contains(w.id)) continue;

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
                EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(50));
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

                        if (justDeletedContents == null) justDeletedContents = new List<string>();
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

            if (uploadedAvatars != null)
                tmpAvatars = new List<ApiAvatar>(uploadedAvatars);

            if (justUpdatedAvatars != null){
                foreach(ApiAvatar a in justUpdatedAvatars)
                {
                    int index = tmpAvatars.FindIndex((av) => av.id == a.id);
                    if(index != -1)
                        tmpAvatars[index] = a;
                }
            }

            foreach (ApiAvatar a in tmpAvatars)
            {
                if (justDeletedContents != null && justDeletedContents.Contains(a.id)) continue;

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
                EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(50));
                EditorGUILayout.LabelField(a.releaseStatus, GUILayout.Width(100));
                EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(50));

                string oppositeReleaseStatus = a.releaseStatus == "public" ? "private" : "public";
                if (GUILayout.Button("Make " + oppositeReleaseStatus, GUILayout.Width(100)))
                {
                    a.releaseStatus = oppositeReleaseStatus;

                    a.Save(true, delegate(ApiModel model)
                    {
                        ApiAvatar savedBP = (ApiAvatar)model;

                        if (justUpdatedAvatars == null) justUpdatedAvatars = new List<ApiAvatar>();
                        justUpdatedAvatars.Add(savedBP);

                        EditorUtility.DisplayDialog("Avatar Updated", "Avatar made " + savedBP.releaseStatus, "OK");
                    },
                    delegate( string error )
                    {
                        Debug.LogError(error);
                        EditorUtility.DisplayDialog("Avatar Updated", "Failed to change avatar release status", "OK");
                    });
                }

                if (GUILayout.Button("Copy ID", GUILayout.Width(75)))
                {
                    TextEditor te = new TextEditor();
                    te.text = a.id;
                    te.SelectAll();
                    te.Copy();
                }

                if (GUILayout.Button("Delete", GUILayout.Width(75)))
                {
                    if (EditorUtility.DisplayDialog("Delete " + a.name + "?", "Are you sure you want to delete " + a.name + "? This cannot be undone.", "Delete", "Cancel"))
                    {
                        ApiAvatar.Delete(a.id, null, null);
                        uploadedAvatars.RemoveAll(avatar => avatar.id == a.id);

                        if (justDeletedContents == null) justDeletedContents = new List<string>();
                        justDeletedContents.Add(a.id);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            return true;
        }
        else
        {
            return false;
        }
    }


    void OnGUI()
    {
        if (window == null)
            window = (VRCContentManagerWindow)EditorWindow.GetWindow(typeof(VRCContentManagerWindow));

        if (VRC.AccountEditorWindow.OnShowStatus())
            OnGUIUserInfo();

        window.Repaint();
    }
}
#endif
