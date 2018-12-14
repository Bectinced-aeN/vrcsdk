using UnityEngine;

public static class ExtensionMethods
{
	public static void Reset(this Transform trans)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		trans.set_position(Vector3.get_zero());
		trans.set_localRotation(Quaternion.get_identity());
		trans.set_localScale(new Vector3(1f, 1f, 1f));
	}

	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		T val = go.GetComponent<T>();
		if ((object)val == null)
		{
			val = go.AddComponent<T>();
		}
		return val;
	}

	public static string GetHierarchyPath(this Transform transform, Transform relativeTransform = null)
	{
		string text = transform.get_name();
		while (transform.get_parent() != null && transform.get_parent() != relativeTransform)
		{
			transform = transform.get_parent();
			text = transform.get_name() + "/" + text;
		}
		return text;
	}

	public static string GetShortHierarchyPath(this Transform transform, Transform relativeTransform = null)
	{
		string text = transform.GetSiblingIndex().ToString();
		while (transform.get_parent() != null && transform.get_parent() != relativeTransform)
		{
			transform = transform.get_parent();
			text = transform.GetSiblingIndex().ToString() + "/" + text;
		}
		return text;
	}
}
