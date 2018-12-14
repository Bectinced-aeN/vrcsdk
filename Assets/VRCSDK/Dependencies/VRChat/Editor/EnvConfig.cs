using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Setup up SDK env on editor launch
/// </summary>
[InitializeOnLoad]
public class EnvConfig
{
	static EnvConfig()
	{
		CustomDLLMaker.CreateDirectories();
		if(EditorApplication.timeSinceStartup < 30f)
		{
			VRC.AssetExporter.CleanupTmpFiles();
			ConfigurePlayerSettings();
#if !VRC_CLIENT
			SDKUpdater.CheckForUpdates(true);
#endif
		}
	}

	static void ConfigurePlayerSettings()
	{
		// Needed for Microsoft.CSharp namespace in DLLMaker
		// Doesn't seem to work though
		if(PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0)
			PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;

		if(!PlayerSettings.runInBackground)
			PlayerSettings.runInBackground = true;
	}
}
