using System;

namespace ThirdParty.iOS4Unity
{
	[Flags]
	public enum UIRemoteNotificationType
	{
		None = 0x0,
		Badge = 0x1,
		Sound = 0x2,
		Alert = 0x4,
		NewsstandContentAvailability = 0x8
	}
}
