using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC;
using VRC.Core;

[CustomEditor(typeof(PipelineManager), true)]
public class VRCPipelineManagerEditor : Editor
{
	public static bool launchedFromSDKPipeline;

	private string tmpBlueprintId;

	public VRCPipelineManagerEditor()
		: this()
	{
	}

	public override void OnInspectorGUI()
	{
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		PipelineManager pipelineManager = (PipelineManager)this.get_target();
		EditorGUILayout.LabelField("Unity Version", Application.get_unityVersion(), (GUILayoutOption[])new GUILayoutOption[0]);
		if (!APIUser.IsLoggedInWithCredentials && APIUser.IsCached)
		{
			pipelineManager.user = APIUser.CachedLogin(null, null, shouldFetch: false);
		}
		else if (APIUser.IsLoggedInWithCredentials && !APIUser.IsCached)
		{
			pipelineManager.user = null;
		}
		pipelineManager.launchedFromSDKPipeline = launchedFromSDKPipeline;
		string text = (!APIUser.IsLoggedInWithCredentials) ? "None" : pipelineManager.blueprintId;
		if (string.IsNullOrEmpty(text))
		{
			tmpBlueprintId = EditorGUILayout.TextField("Blueprint ID (Optional)", tmpBlueprintId, (GUILayoutOption[])new GUILayoutOption[0]);
		}
		else
		{
			EditorGUILayout.LabelField("Blueprint ID", text, (GUILayoutOption[])new GUILayoutOption[0]);
		}
		string text2 = (!string.IsNullOrEmpty(text)) ? "Detach (Optional)" : "Attach (Optional)";
		if (APIUser.IsLoggedInWithCredentials && GUILayout.Button(text2, (GUILayoutOption[])new GUILayoutOption[0]))
		{
			if (string.IsNullOrEmpty(text))
			{
				text = (pipelineManager.blueprintId = tmpBlueprintId);
				pipelineManager.assetBundleUnityVersion = Application.get_unityVersion();
			}
			else
			{
				pipelineManager.blueprintId = string.Empty;
			}
			EditorUtility.SetDirty(pipelineManager);
			EditorSceneManager.MarkSceneDirty(pipelineManager.get_gameObject().get_scene());
			EditorSceneManager.SaveScene(pipelineManager.get_gameObject().get_scene());
		}
		if (!APIUser.IsLoggedInWithCredentials && GUILayout.Button("Login", (GUILayoutOption[])new GUILayoutOption[0]))
		{
			AccountEditorWindow.Init();
		}
	}
}
