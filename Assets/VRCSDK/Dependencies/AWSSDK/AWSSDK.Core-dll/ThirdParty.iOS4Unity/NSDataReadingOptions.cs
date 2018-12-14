using System;

namespace ThirdParty.iOS4Unity
{
	[Flags]
	[CLSCompliant(false)]
	public enum NSDataReadingOptions : uint
	{
		Mapped = 0x1,
		Uncached = 0x2,
		Coordinated = 0x4,
		MappedAlways = 0x8
	}
}
