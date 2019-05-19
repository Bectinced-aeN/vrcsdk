using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VRC.Core
{
	public class Logger
	{
		public enum Color
		{
			aqua,
			black,
			blue,
			brown,
			cyan,
			darkblue,
			fuchsia,
			green,
			grey,
			lightblue,
			lime,
			magenta,
			maroon,
			navy,
			olive,
			orange,
			purple,
			red,
			silver,
			teal,
			white,
			yellow
		}

		private static List<int> enabledDebugLevels;

		private static Dictionary<int, float> _timeLastLogPrintedForHash = new Dictionary<int, float>();

		private static Dictionary<int, string> _levelMap = new Dictionary<int, string>
		{
			{
				7,
				"[<color=orange>All</color>] "
			},
			{
				0,
				"[<color=orange>Always</color>] "
			},
			{
				1,
				"[<color=red>API</color>] "
			},
			{
				2,
				"[<color=orange>AssetBundleDownloadManager</color>] "
			},
			{
				3,
				"[<color=orange>ContentCreator</color>] "
			},
			{
				4,
				"[<color=red>Network Transport</color>] "
			},
			{
				5,
				"[<color=red>Network Data</color>] "
			},
			{
				6,
				"[<color=red>Network Processing</color>] "
			}
		};

		public static IEnumerable<KeyValuePair<int, string>> KnownLevels => _levelMap.AsEnumerable();

		public static void DescribeDebugLevel(int level, string name, Color color = Color.yellow)
		{
			name = "[<color=" + color.ToString() + ">" + name + "</color>] ";
			if (_levelMap.ContainsKey(level))
			{
				_levelMap[level] = name;
			}
			else
			{
				_levelMap.Add(level, name);
			}
		}

		public static bool DebugLevelIsDescribed(int level)
		{
			return _levelMap.ContainsKey(level);
		}

		public static void SetDebugLevel(DebugLevel level)
		{
			SetDebugLevels(new int[1]
			{
				(int)level
			});
		}

		public static void SetDebugLevel(int level)
		{
			SetDebugLevels(new int[1]
			{
				level
			});
		}

		public static void SetDebugLevels(IEnumerable<DebugLevel> levels)
		{
			SetDebugLevels(levels.Cast<int>());
		}

		public static void SetDebugLevels(IEnumerable<int> levels)
		{
			enabledDebugLevels = levels.ToList();
		}

		public static void AddDebugLevel(DebugLevel level)
		{
			AddDebugLevel((int)level);
		}

		public static void AddDebugLevel(int level)
		{
			if (enabledDebugLevels == null)
			{
				enabledDebugLevels = new List<int>();
			}
			enabledDebugLevels.Add(level);
		}

		public static void RemoveDebugLevel(DebugLevel level)
		{
			RemoveDebugLevel((int)level);
		}

		public static void RemoveDebugLevel(int level)
		{
			if (enabledDebugLevels == null)
			{
				enabledDebugLevels = new List<int>();
			}
			enabledDebugLevels.Remove(level);
		}

		public static bool DebugLevelIsEnabled(DebugLevel level)
		{
			return DebugLevelIsEnabled((int)level);
		}

		public static bool DebugLevelIsEnabled(int level)
		{
			return (enabledDebugLevels != null && enabledDebugLevels.Contains(7)) || level == 0 || (enabledDebugLevels != null && enabledDebugLevels.Contains(level));
		}

		public static void Log(string message, DebugLevel debugLevel = DebugLevel.Always, Object context = null)
		{
			if (DebugLevelIsEnabled((int)debugLevel))
			{
				Debug.Log((object)(MakePrefix((int)debugLevel) + message), context);
			}
		}

		public static void LogWarning(string message, DebugLevel debugLevel = DebugLevel.Always, Object context = null)
		{
			if (DebugLevelIsEnabled((int)debugLevel))
			{
				Debug.LogWarning((object)(MakePrefix((int)debugLevel) + message), context);
			}
		}

		public static void LogError(string message, DebugLevel debugLevel = DebugLevel.Always, Object context = null)
		{
			if (DebugLevelIsEnabled((int)debugLevel))
			{
				Debug.LogError((object)(MakePrefix((int)debugLevel) + message), context);
			}
		}

		public static void Log(string message, int debugLevel, Object context = null)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.Log((object)(MakePrefix(debugLevel) + message), context);
			}
		}

		public static void LogWarning(string message, int debugLevel, Object context = null)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.LogWarning((object)(MakePrefix(debugLevel) + message), context);
			}
		}

		public static void LogError(string message, int debugLevel, Object context = null)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.LogError((object)(MakePrefix(debugLevel) + message), context);
			}
		}

		public static void LogFormat(int debugLevel, Object context, string format, params object[] args)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.LogFormat(context, MakePrefix(debugLevel) + format, args);
			}
		}

		public static void LogWarningFormat(int debugLevel, Object context, string format, params object[] args)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.LogWarningFormat(context, MakePrefix(debugLevel) + format, args);
			}
		}

		public static void LogErrorFormat(int debugLevel, Object context, string format, params object[] args)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				Debug.LogErrorFormat(context, MakePrefix(debugLevel) + format, args);
			}
		}

		public static void LogFormat(string format, params object[] args)
		{
			LogFormat(DebugLevel.All, null, format, args);
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			LogWarningFormat(DebugLevel.All, null, format, args);
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			LogErrorFormat(DebugLevel.All, null, format, args);
		}

		public static void LogFormat(Object context, string format, params object[] args)
		{
			LogFormat(DebugLevel.All, context, format, args);
		}

		public static void LogWarningFormat(Object context, string format, params object[] args)
		{
			LogWarningFormat(DebugLevel.All, context, format, args);
		}

		public static void LogErrorFormat(Object context, string format, params object[] args)
		{
			LogErrorFormat(DebugLevel.All, context, format, args);
		}

		public static void LogFormat(DebugLevel debugLevel, Object context, string format, params object[] args)
		{
			LogFormat((int)debugLevel, context, format, args);
		}

		public static void LogWarningFormat(DebugLevel debugLevel, Object context, string format, params object[] args)
		{
			LogWarningFormat((int)debugLevel, context, format, args);
		}

		public static void LogErrorFormat(DebugLevel debugLevel, Object context, string format, params object[] args)
		{
			LogErrorFormat((int)debugLevel, context, format, args);
		}

		public static void LogFormat(DebugLevel debugLevel, string format, params object[] args)
		{
			LogFormat((int)debugLevel, null, format, args);
		}

		public static void LogWarningFormat(DebugLevel debugLevel, string format, params object[] args)
		{
			LogWarningFormat((int)debugLevel, null, format, args);
		}

		public static void LogErrorFormat(DebugLevel debugLevel, string format, params object[] args)
		{
			LogErrorFormat((int)debugLevel, null, format, args);
		}

		public static void LogFormat(int debugLevel, string format, params object[] args)
		{
			LogFormat(debugLevel, null, format, args);
		}

		public static void LogWarningFormat(int debugLevel, string format, params object[] args)
		{
			LogWarningFormat(debugLevel, null, format, args);
		}

		public static void LogErrorFormat(int debugLevel, string format, params object[] args)
		{
			LogErrorFormat(debugLevel, null, format, args);
		}

		public static void LogOnceEvery(float seconds, Object obj, string message, DebugLevel debugLevel = DebugLevel.Always)
		{
			LogOnceEveryHashFormat((int)debugLevel, seconds, obj, message);
		}

		public static void LogOnceEveryFormat(DebugLevel debugLevel, float seconds, Object obj, string format, params object[] args)
		{
			LogOnceEveryHashFormat((int)debugLevel, seconds, obj, format, args);
		}

		public static void LogOnceEveryFormat(int debugLevel, float seconds, Object obj, string format, params object[] args)
		{
			LogOnceEveryHashFormat(debugLevel, seconds, obj, format, args);
		}

		public static void LogOnceEveryFormat(float seconds, Object obj, string format, params object[] args)
		{
			LogOnceEveryHashFormat(7, seconds, obj, format, args);
		}

		private static void LogOnceEveryHashFormat(int debugLevel, float seconds, Object context, string format, params object[] args)
		{
			if (DebugLevelIsEnabled(debugLevel))
			{
				int callingStackFrameHash = GetCallingStackFrameHash(context);
				float value = 0f;
				if (!_timeLastLogPrintedForHash.TryGetValue(callingStackFrameHash, out value) || !(Time.get_realtimeSinceStartup() - value < seconds))
				{
					_timeLastLogPrintedForHash[callingStackFrameHash] = Time.get_realtimeSinceStartup();
					LogFormat(debugLevel, context, format, args);
				}
			}
		}

		private static int GetCallingStackFrameHash(object instance)
		{
			StackFrame stackFrame = new StackFrame(3, fNeedFileInfo: false);
			MethodBase method = stackFrame.GetMethod();
			int a = Tools.CombineHashCodes(method.ReflectedType.Name.GetHashCode(), method.Name.GetHashCode());
			int a2 = Tools.CombineHashCodes(a, stackFrame.GetNativeOffset());
			return Tools.CombineHashCodes(a2, instance?.GetHashCode() ?? 0);
		}

		private static string MakePrefix(int level)
		{
			if (!_levelMap.ContainsKey(level))
			{
				DescribeDebugLevel(level, level.ToString());
			}
			return _levelMap[level];
		}
	}
}
