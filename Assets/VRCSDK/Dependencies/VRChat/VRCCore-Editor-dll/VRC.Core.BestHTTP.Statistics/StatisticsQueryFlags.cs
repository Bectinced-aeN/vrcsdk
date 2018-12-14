using System;

namespace VRC.Core.BestHTTP.Statistics
{
	[Flags]
	public enum StatisticsQueryFlags : byte
	{
		Connections = 0x1,
		Cache = 0x2,
		Cookies = 0x4,
		All = byte.MaxValue
	}
}
