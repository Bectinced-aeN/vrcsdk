using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class Logger
	{
		private static List<DebugLevel> enabledDebugLevels;

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

		private static bool DebugLevelIsEnabled(DebugLevel level)
		{
			return (enabledDebugLevels != null && enabledDebugLevels.Contains(DebugLevel.All)) || level == DebugLevel.Always || (enabledDebugLevels != null && enabledDebugLevels.Contains(level));
		}
	}
}
