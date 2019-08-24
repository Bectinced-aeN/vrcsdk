using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.Core;

public partial class VRCSdkControlPanel : EditorWindow
{
    bool UseDevApi
    {
        get
        {
            return VRC.Core.API.GetApiUrl() == VRC.Core.API.devApiUrl;
        }
    }

    string clientVersionDate;
    string sdkVersionDate;
    Vector2 settingsScroll;

    private void Awake()
    {
        GetClientSdkVersionInformation();
    }

    public void GetClientSdkVersionInformation()
    {
        clientVersionDate = VRC.Core.SDKClientUtilities.GetTestClientVersionDate();
        sdkVersionDate = VRC.Core.SDKClientUtilities.GetSDKVersionDate();
    }

    public void OnConfigurationChanged()
    {
        GetClientSdkVersionInformation();
    }

    void ShowSettings()
    {
        settingsScroll = EditorGUILayout.BeginScrollView( settingsScroll, GUILayout.Width(SdkWindowWidth) );

        EditorGUILayout.BeginVertical(boxGuiStyle);
        EditorGUILayout.LabelField("Developer", EditorStyles.boldLabel);
        VRCSettings.Get().DisplayAdvancedSettings = EditorGUILayout.ToggleLeft("Show Extra Options on build page and account page",VRCSettings.Get().DisplayAdvancedSettings );
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical(boxGuiStyle);
        GUILayout.Label("Avatar Options", EditorStyles.boldLabel);
        bool prevShowPerfDetails = showAvatarPerformanceDetails;
        bool showPerfDetails = EditorGUILayout.ToggleLeft("Show All Avatar Performance Details", prevShowPerfDetails);
        if (showPerfDetails != prevShowPerfDetails)
        {
            showAvatarPerformanceDetails = showPerfDetails;
            ResetIssues();
        }
        EditorGUILayout.EndVertical();

        // debugging
        if (APIUser.CurrentUser != null && APIUser.CurrentUser.hasSuperPowers)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(boxGuiStyle);

            EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);

            // API logging
            {
                bool isLoggingEnabled = UnityEditor.EditorPrefs.GetBool("apiLoggingEnabled");
                bool enableLogging = EditorGUILayout.ToggleLeft("API Logging Enabled", isLoggingEnabled);
                if (enableLogging != isLoggingEnabled)
                {
                    if (enableLogging)
                        VRC.Core.Logger.AddDebugLevel(DebugLevel.API);
                    else
                        VRC.Core.Logger.RemoveDebugLevel(DebugLevel.API);

                    UnityEditor.EditorPrefs.SetBool("apiLoggingEnabled", enableLogging);
                }
            }

            // All logging
            {
                bool isLoggingEnabled = UnityEditor.EditorPrefs.GetBool("allLoggingEnabled");
                bool enableLogging = EditorGUILayout.ToggleLeft("All Logging Enabled", isLoggingEnabled);
                if (enableLogging != isLoggingEnabled)
                {
                    if (enableLogging)
                        VRC.Core.Logger.AddDebugLevel(DebugLevel.All);
                    else
                        VRC.Core.Logger.RemoveDebugLevel(DebugLevel.All);

                    UnityEditor.EditorPrefs.SetBool("allLoggingEnabled", enableLogging);
                }
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            if (UnityEditor.EditorPrefs.GetBool("apiLoggingEnabled"))
                UnityEditor.EditorPrefs.SetBool("apiLoggingEnabled", false);
            if (UnityEditor.EditorPrefs.GetBool("allLoggingEnabled"))
                UnityEditor.EditorPrefs.SetBool("allLoggingEnabled", false);
        }

        // Future proof upload
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(boxGuiStyle);

            EditorGUILayout.LabelField("Publish", EditorStyles.boldLabel);
            bool futureProofPublish = UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled);

            futureProofPublish = EditorGUILayout.ToggleLeft("Future Proof Publish", futureProofPublish);

            if (UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled) != futureProofPublish)
            {
                UnityEditor.EditorPrefs.SetBool("futureProofPublish", futureProofPublish);
            }
            EditorGUILayout.LabelField("Client Version Date", clientVersionDate);
            EditorGUILayout.LabelField("SDK Version Date", sdkVersionDate);

            EditorGUILayout.EndVertical();
        }


        if (APIUser.CurrentUser != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(boxGuiStyle);

            // custom vrchat install location
            OnVRCInstallPathGUI();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    static void OnVRCInstallPathGUI()
    {
        EditorGUILayout.LabelField("VRChat Client", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Installed Client Path: ", clientInstallPath);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("");
        if (GUILayout.Button("Edit"))
        {
            string initPath = "";
            if (!string.IsNullOrEmpty(clientInstallPath))
                initPath = clientInstallPath;

            clientInstallPath = EditorUtility.OpenFilePanel("Choose VRC Client Exe", initPath, "exe");
            SDKClientUtilities.SetVRCInstallPath(clientInstallPath);
            window.OnConfigurationChanged();
        }
        if (GUILayout.Button("Revert to Default"))
        {
            clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();
            window.OnConfigurationChanged();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
    }
}
