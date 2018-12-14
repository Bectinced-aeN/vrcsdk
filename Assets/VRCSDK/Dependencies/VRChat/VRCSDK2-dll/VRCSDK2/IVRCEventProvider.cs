using System.Collections.Generic;

namespace VRCSDK2
{
	public interface IVRCEventProvider
	{
		IEnumerable<VRC_EventHandler.VrcEvent> ProvideEvents();
	}
}
