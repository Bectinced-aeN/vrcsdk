using UnityEngine;

public class VRCProjectSettings : MonoBehaviour
{
	public string[] layers;

	public int numLayers;

	public bool[] layerCollisionArr;

	public bool queriesHitTriggers;

	public VRCProjectSettings()
		: this()
	{
	}
}
