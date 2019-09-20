using System;
using System.Text.RegularExpressions;

namespace VRC.Core
{
	public struct UnityVersion : IComparable<UnityVersion>
	{
		private static readonly Regex versionRegex = new Regex("(?<major>\\d{1,4})[.](?<minor>\\d{1,4})[.](?<update>\\d{1,4})(f|p)(?<patch>\\d{1,4})", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		public readonly int major;

		public readonly int minor;

		public readonly int update;

		public readonly int patch;

		public UnityVersion(int major, int minor, int update, int patch)
		{
			this.major = major;
			this.minor = minor;
			this.update = update;
			this.patch = patch;
		}

		public int CompareTo(UnityVersion other)
		{
			int num = major.CompareTo(other.major);
			if (num != 0)
			{
				return num;
			}
			int num2 = minor.CompareTo(other.minor);
			if (num2 != 0)
			{
				return num2;
			}
			int num3 = update.CompareTo(other.update);
			if (num3 != 0)
			{
				return num3;
			}
			return patch.CompareTo(other.patch);
		}

		public static UnityVersion Parse(string unityVersionString)
		{
			Match match = versionRegex.Match(unityVersionString);
			if (!match.Success)
			{
				throw new FormatException("Input string was not a UnityVersion string: " + unityVersionString);
			}
			int num = int.Parse(match.Groups["major"].Value);
			int num2 = int.Parse(match.Groups["minor"].Value);
			int num3 = int.Parse(match.Groups["update"].Value);
			int num4 = int.Parse(match.Groups["patch"].Value);
			return new UnityVersion(num, num2, num3, num4);
		}

		public static bool TryParse(string unityVersionString, out UnityVersion unityVersion)
		{
			Match match = versionRegex.Match(unityVersionString);
			if (!match.Success)
			{
				unityVersion = default(UnityVersion);
				return false;
			}
			int num = int.Parse(match.Groups["major"].Value);
			int num2 = int.Parse(match.Groups["minor"].Value);
			int num3 = int.Parse(match.Groups["update"].Value);
			int num4 = int.Parse(match.Groups["patch"].Value);
			unityVersion = new UnityVersion(num, num2, num3, num4);
			return true;
		}

		public override string ToString()
		{
			return $"{major}.{minor}.{update}" + ((patch == 0) ? string.Empty : ("p" + patch));
		}
	}
}
