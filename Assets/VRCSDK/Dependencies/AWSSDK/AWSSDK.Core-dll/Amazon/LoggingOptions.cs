using System;

namespace Amazon
{
	[Flags]
	public enum LoggingOptions
	{
		None = 0x0,
		Log4Net = 0x1,
		SystemDiagnostics = 0x2,
		Console = 0x10,
		UnityLogger = 0x8
	}
}
