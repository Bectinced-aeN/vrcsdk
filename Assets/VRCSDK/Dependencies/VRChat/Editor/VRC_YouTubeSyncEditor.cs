#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VRCSDK2
{
    [CustomEditor(typeof(VRC_YouTubeSync))]
	public class VRC_YouTubeSyncEditor : Editor
    {
		SerializedProperty screenName;
		SerializedProperty videoId;
		SerializedProperty hideUi;
		SerializedProperty loop;
        SerializedProperty autoplay;
   	    SerializedProperty volume;

		void OnEnable()
		{
			videoId = serializedObject.FindProperty("videoID");
			hideUi = serializedObject.FindProperty("hideUI");
			loop = serializedObject.FindProperty("loop");
			autoplay = serializedObject.FindProperty("autoplay");
			volume = serializedObject.FindProperty("volume");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawDefaultInspector(); // doing this for the playlist array. Arrays + property fields are hard
			EditorGUILayout.PropertyField(videoId, new GUIContent("Youtube Video Id (optional)"));
			EditorGUILayout.PropertyField(hideUi, new GUIContent("Hide UI"));
			EditorGUILayout.PropertyField(loop, new GUIContent("Loop Video"));
			EditorGUILayout.PropertyField(autoplay, new GUIContent("Autoplay"));
			EditorGUILayout.IntSlider(volume, 0, 100, new GUIContent("Volume"));

			serializedObject.ApplyModifiedProperties();
		}
	} 
}
#endif