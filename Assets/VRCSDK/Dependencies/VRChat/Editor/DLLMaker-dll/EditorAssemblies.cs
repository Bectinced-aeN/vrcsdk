using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class EditorAssemblies
{
	public delegate void OnDeserialized();

	public static string SERIALIZATION_PATH = Application.get_dataPath() + "/../vrcAssemblyCallbacks";

	public static string TMP_FILE_PATH = Application.get_dataPath() + "/VRCSDK/_tmp.cs";

	private static List<SerializableFunction> OnAssemblyReloadCallbacks;

	public OnDeserialized onDeserialized;

	[DidReloadScripts]
	public unsafe static void OnCompileScripts()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		Debug.Log((object)"Reloading Assemblies");
		EditorApplication.delayCall = Delegate.Combine((Delegate)EditorApplication.delayCall, (Delegate)new CallbackFunction((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public static void ReloadAssemblies()
	{
		if (!File.Exists(TMP_FILE_PATH))
		{
			FileStream fileStream = File.Create(TMP_FILE_PATH);
			fileStream.Close();
		}
		else
		{
			File.Delete(TMP_FILE_PATH);
		}
		AssetDatabase.Refresh();
	}

	public static void DoIt()
	{
		DeserializeCallbacks();
		if (File.Exists(SERIALIZATION_PATH))
		{
			File.Delete(SERIALIZATION_PATH);
		}
		ExecuteAssemblyReloadCallbacks();
	}

	private static void DeserializeCallbacks()
	{
		if (File.Exists(SERIALIZATION_PATH))
		{
			StreamReader streamReader = new StreamReader(SERIALIZATION_PATH);
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				string[] array = text.Split(',');
				AddOnAssemblyReloadCallback(array[0], array[1], autoSerialize: false);
			}
			streamReader.Close();
		}
	}

	private static void SerializeCallbacks()
	{
		if (File.Exists(SERIALIZATION_PATH))
		{
			File.Delete(SERIALIZATION_PATH);
		}
		using (TextWriter textWriter = File.CreateText(SERIALIZATION_PATH))
		{
			foreach (SerializableFunction onAssemblyReloadCallback in OnAssemblyReloadCallbacks)
			{
				textWriter.WriteLine(onAssemblyReloadCallback.className + "," + onAssemblyReloadCallback.methodName);
			}
			textWriter.Close();
		}
	}

	public static void AddOnAssemblyReloadCallback(string className, string staticFunctionName, bool autoSerialize = true)
	{
		if (OnAssemblyReloadCallbacks == null)
		{
			OnAssemblyReloadCallbacks = new List<SerializableFunction>();
		}
		if (!OnAssemblyReloadCallbacks.Any((SerializableFunction sf) => sf.methodName == staticFunctionName && sf.className == className))
		{
			SerializableFunction item = new SerializableFunction(className, staticFunctionName);
			OnAssemblyReloadCallbacks.Add(item);
			if (autoSerialize)
			{
				SerializeCallbacks();
			}
		}
	}

	public static void ClearAssemblyReloadCallbacks()
	{
		if (OnAssemblyReloadCallbacks != null)
		{
			OnAssemblyReloadCallbacks = null;
		}
	}

	private static void ExecuteAssemblyReloadCallbacks()
	{
		List<SerializableFunction> onAssemblyReloadCallbacks = OnAssemblyReloadCallbacks;
		ClearAssemblyReloadCallbacks();
		if (onAssemblyReloadCallbacks != null)
		{
			foreach (SerializableFunction item in onAssemblyReloadCallbacks)
			{
				try
				{
					Type type = Assembly.GetExecutingAssembly().GetType(item.className);
					MethodInfo method = type.GetMethod(item.methodName, BindingFlags.Static | BindingFlags.Public);
					method.Invoke(null, null);
				}
				catch (Exception ex)
				{
					Debug.Log((object)("Error calling " + item.className + "." + item.methodName + " - " + ex.Message));
				}
			}
		}
	}

	public static List<string> GetLoadedAssemblies()
	{
		List<string> list = new List<string>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		Assembly[] array = assemblies;
		foreach (Assembly assembly in array)
		{
			AssemblyName name = assembly.GetName();
			string name2 = name.Name;
			list.Add(name2);
		}
		return list;
	}

	public static List<string> GetCustomLoadedAssembles()
	{
		List<string> loadedAssemblies = GetLoadedAssemblies();
		return (from a in loadedAssemblies
		where a.StartsWith("vrc")
		select a).ToList();
	}
}
