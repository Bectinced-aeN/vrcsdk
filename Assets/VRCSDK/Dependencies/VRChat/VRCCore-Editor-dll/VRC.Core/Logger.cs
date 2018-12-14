using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace VRC.Core
{
	public class Logger
	{
		private static List<DebugLevel> enabledDebugLevels;

		private static Dictionary<int, float> _timeLastLogPrintedForHash = new Dictionary<int, float>();

		public static void SetDebugLevel(DebugLevel level)
		{
			SetDebugLevels(new DebugLevel[1]
			{
				level
			});
		}

		public static void SetDebugLevels(DebugLevel[] levels)
		{
			enabledDebugLevels = new List<DebugLevel>(levels);
		}

		public static void AddDebugLevel(DebugLevel level)
		{
			if (enabledDebugLevels == null)
			{
				enabledDebugLevels = new List<DebugLevel>();
			}
			enabledDebugLevels.Add(level);
		}

		public static void Log(string message, DebugLevel debugLevel = DebugLevel.Always)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.Log((object)message);
			}
		}

		public static void LogWarning(string message, DebugLevel debugLevel = DebugLevel.Always)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.Log((object)message);
			}
		}

		public static void LogError(string message, DebugLevel debugLevel = DebugLevel.Always)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.Log((object)message);
			}
		}

		public static void LogOnceEvery(float seconds, object obj, string message)
		{
			int callingStackFrameHash = GetCallingStackFrameHash(obj);
			LogOnceEveryHash(seconds, callingStackFrameHash, message);
		}

		public static void LogOnceEveryHash(float seconds, int lineOfCodeHash, string message)
		{
			float value = 0f;
			if (!_timeLastLogPrintedForHash.TryGetValue(lineOfCodeHash, out value) || !(Time.get_realtimeSinceStartup() - value < seconds))
			{
				_timeLastLogPrintedForHash[lineOfCodeHash] = Time.get_realtimeSinceStartup();
				Log(message);
			}
		}

		public static int GetCallingStackFrameHash(object instance)
		{
			StackFrame stackFrame = new StackFrame(2, fNeedFileInfo: false);
			MethodBase method = stackFrame.GetMethod();
			int a = Tools.CombineHashCodes(method.ReflectedType.Name.GetHashCode(), method.Name.GetHashCode());
			int a2 = Tools.CombineHashCodes(a, stackFrame.GetNativeOffset());
			return Tools.CombineHashCodes(a2, instance?.GetHashCode() ?? 0);
		}

		private static bool DebugLevelIsEnabled(DebugLevel level)
		{
			return (enabledDebugLevels != null && enabledDebugLevels.Contains(DebugLevel.All)) || level == DebugLevel.Always || (enabledDebugLevels != null && enabledDebugLevels.Contains(level));
		}
	}
}
