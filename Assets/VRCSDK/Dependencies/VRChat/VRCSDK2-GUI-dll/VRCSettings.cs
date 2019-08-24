using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VRCSettings", order = 1)]
public class VRCSettings : ScriptableObject
{
	private const string savePath = "Assets/VRCSDK/Dependencies/VRChat/Settings.asset";

	private static VRCSettings instance;

	public int activeWindowPanel;

	public int[] polygonLimits = new int[3]
	{
		1000,
		5000,
		10000
	};

	public bool DisplayAdvancedSettings;

	public VRCSettings()
		: this()
	{
	}

	public static VRCSettings Get()
	{
		if (instance == null)
		{
			instance = (AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Dependencies/VRChat/Settings.asset", typeof(VRCSettings)) as VRCSettings);
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<VRCSettings>();
				AssetDatabase.CreateAsset(instance, "Assets/VRCSDK/Dependencies/VRChat/Settings.asset");
			}
		}
		return instance;
	}
}
