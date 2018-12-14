using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC.Core;

[CustomEditor(typeof(PipelineManager), true)]
public class VRCPipelineManagerEditor : Editor
{
	public static bool launchedFromSDKPipeline;

	private string tmpBlueprintId;

	private bool loggingIn;

	public VRCPipelineManagerEditor()
		: this()
	{
	}

	public override void OnInspectorGUI()
	{
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		PipelineManager pipeline = (PipelineManager)this.get_target();
		EditorGUILayout.LabelField("Unity Version", Application.get_unityVersion(), (GUILayoutOption[])new GUILayoutOption[0]);
		if (!loggingIn)
		{
			bool flag = ApiCredentials.Load();
			if (!APIUser.IsLoggedInWithCredentials && flag)
			{
				API.SetOnlineMode(online: true);
				loggingIn = true;
				APIUser.Login(delegate(APIUser user)
				{
					loggingIn = false;
					pipeline.user = user;
				}, delegate
				{
					loggingIn = false;
				});
			}
			else if (APIUser.IsLoggedInWithCredentials && !flag)
			{
				pipeline.user = null;
			}
		}
		pipeline.launchedFromSDKPipeline = launchedFromSDKPipeline;
		string text = (!APIUser.IsLoggedInWithCredentials) ? "None" : pipeline.blueprintId;
		if (string.IsNullOrEmpty(text))
		{
			tmpBlueprintId = EditorGUILayout.TextField("Blueprint ID (Optional)", tmpBlueprintId, (GUILayoutOption[])new GUILayoutOption[0]);
		}
		else
		{
			EditorGUILayout.PrefixLabel("Blueprint ID");
			EditorGUILayout.SelectableLabel(text, (GUILayoutOption[])new GUILayoutOption[0]);
		}
		string text2 = (!string.IsNullOrEmpty(text)) ? "Detach (Optional)" : "Attach (Optional)";
		if (APIUser.IsLoggedInWithCredentials && GUILayout.Button(text2, (GUILayoutOption[])new GUILayoutOption[0]))
		{
			if (string.IsNullOrEmpty(text))
			{
				text = (pipeline.blueprintId = tmpBlueprintId);
				pipeline.completedSDKPipeline = true;
			}
			else
			{
				pipeline.blueprintId = string.Empty;
				pipeline.completedSDKPipeline = false;
			}
			EditorUtility.SetDirty(pipeline);
			EditorSceneManager.MarkSceneDirty(pipeline.get_gameObject().get_scene());
			EditorSceneManager.SaveScene(pipeline.get_gameObject().get_scene());
		}
		if (!APIUser.IsLoggedInWithCredentials)
		{
			GUILayout.Label("Use the settings menu to log in.", EditorStyles.get_boldLabel(), (GUILayoutOption[])new GUILayoutOption[0]);
		}
	}
}
