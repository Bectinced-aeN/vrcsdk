#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VRCSDK2
{
    [CustomEditor(typeof(VRC_WebPanel))]
    public class VRC_WebPanelEditor : Editor
    {
        VRC_WebPanel web;


        void OnEnable()
        {
            web = (VRC_WebPanel)target;

        }

        private void InspectorField(string propertyName)
        {
            serializedObject.Update();
            SerializedProperty propertyField = serializedObject.FindProperty(propertyName);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propertyField, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
        
        bool showFiles = false;
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

			InspectorField("defaultUrl");
            EditorGUILayout.Space();

			InspectorField("webRoot");
			InspectorField("virtualHost");
            showFiles = web.webData != null && EditorGUILayout.Foldout(showFiles, web.webData.Count.ToString() + " files imported");
            if (showFiles)
            {
                foreach (var file in web.webData)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(file.path);
                    EditorGUILayout.LabelField(file.data.Length.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("Import Web Root"))
                ImportWebData();
            EditorGUILayout.Space();

			InspectorField("interactive");

			InspectorField("syncURI");

			InspectorField("syncInput");

            EditorGUILayout.Space();

            InspectorField("station");
            EditorGUILayout.Space();

			InspectorField("cursor");

			InspectorField("cookiesEnabled");

            EditorGUILayout.Space();

			InspectorField("resolutionWidth");
			InspectorField("resolutionHeight");
			InspectorField("displayRegion");
			InspectorField("transparent");
            EditorGUILayout.Space();

            InspectorField("extraVideoScreens");
            EditorGUILayout.EndVertical();
        }

        private void ReadData(string root)
        {
            string[] files = Directory.GetFiles(root);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta"))
                    continue;

                string fixed_file = file.Replace('\\', '/');
                web.webData.Add(new VRC_WebPanel.WebFile()
                {
                    path = fixed_file.Remove(0, Application.dataPath.Length + 1 + web.webRoot.Length),
                    data = File.ReadAllBytes(fixed_file)
                });
            }

            string[] subdirectories = Directory.GetDirectories(root);
            foreach (string directory in subdirectories)
            {
                string fixed_directory = directory.Replace('\\', '/');
                ReadData(fixed_directory);
            }
        }

        private void ImportWebData()
        {
            if (web.webRoot != null)
            {
                string path = Application.dataPath + "/" + web.webRoot;
                if (Directory.Exists(path))
                {
                    web.webData = new List<VRC_WebPanel.WebFile>();

                    ReadData(path);

                    string files = string.Join("\n ", web.webData.ConvertAll(item => item.path + " [" + item.data.Length + "]").ToArray());
                    Debug.Log("Web Panel has files: \n" + files);
                }
            }
        }
    }
}
#endif