using System;

namespace ThirdParty.iOS4Unity
{
	[CLSCompliant(false)]
	public enum UIPopoverArrowDirection : uint
	{
		Up = 1u,
		Down = 2u,
		Left = 4u,
		Right = 8u,
		Any = 0xF,
		Unknown = uint.MaxValue
	}
}
