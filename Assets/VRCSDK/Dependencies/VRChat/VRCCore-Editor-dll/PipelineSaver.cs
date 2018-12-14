using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRC.Core;

[ExecuteInEditMode]
public class PipelineSaver : MonoBehaviour
{
	public PipelineManager.ContentType contentType;

	private PipelineManager pipeline;

	public PipelineSaver()
		: this()
	{
	}

	private void Start()
	{
		pipeline = this.GetComponent<PipelineManager>();
	}

	private void Update()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!EditorApplication.get_isPlaying() && !EditorApplication.get_isPlayingOrWillChangePlaymode())
		{
			pipeline.assetBundleUnityVersion = Application.get_unityVersion();
			if (EditorPrefs.HasKey("blueprintID-" + pipeline.GetInstanceID().ToString()))
			{
				string @string = EditorPrefs.GetString("blueprintID-" + pipeline.GetInstanceID().ToString());
				pipeline.blueprintId = @string;
				EditorPrefs.DeleteKey("blueprintID-" + pipeline.GetInstanceID().ToString());
			}
			EditorUtility.SetDirty(pipeline);
			EditorSceneManager.MarkSceneDirty(pipeline.get_gameObject().get_scene());
			EditorSceneManager.SaveScene(pipeline.get_gameObject().get_scene());
			Object.DestroyImmediate(this);
		}
	}
}
