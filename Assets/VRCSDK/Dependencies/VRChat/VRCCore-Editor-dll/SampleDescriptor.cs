using System;
using UnityEngine;

internal sealed class SampleDescriptor
{
	public bool IsLabel
	{
		get;
		set;
	}

	public Type Type
	{
		get;
		set;
	}

	public string DisplayName
	{
		get;
		set;
	}

	public string Description
	{
		get;
		set;
	}

	public string CodeBlock
	{
		get;
		set;
	}

	public bool IsSelected
	{
		get;
		set;
	}

	public GameObject UnityObject
	{
		get;
		set;
	}

	public bool IsRunning => UnityObject != null;

	public SampleDescriptor(Type type, string displayName, string description, string codeBlock)
	{
		Type = type;
		DisplayName = displayName;
		Description = description;
		CodeBlock = codeBlock;
	}

	public void CreateUnityObject()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		if (!(UnityObject != null))
		{
			UnityObject = new GameObject(DisplayName);
			UnityObject.AddComponent(Type);
		}
	}

	public void DestroyUnityObject()
	{
		if (UnityObject != null)
		{
			Object.Destroy(UnityObject);
			UnityObject = null;
		}
	}
}
