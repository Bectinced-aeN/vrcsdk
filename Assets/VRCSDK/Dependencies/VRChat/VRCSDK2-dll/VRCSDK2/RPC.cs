using System;
using System.Collections.Generic;
using System.Linq;

namespace VRCSDK2
{
	public class RPC : Attribute
	{
		public VRC_EventHandler.VrcTargetType[] allowedTargets;

		public RPC(params VRC_EventHandler.VrcTargetType[] targets)
		{
			if (targets == null || targets.Length == 0)
			{
				allowedTargets = Enum.GetValues(typeof(VRC_EventHandler.VrcTargetType)).Cast<VRC_EventHandler.VrcTargetType>().ToArray();
			}
			else if (targets.Contains(VRC_EventHandler.VrcTargetType.Local))
			{
				allowedTargets = targets;
			}
			else
			{
				List<VRC_EventHandler.VrcTargetType> list = targets.ToList();
				list.Add(VRC_EventHandler.VrcTargetType.Local);
				allowedTargets = list.ToArray();
			}
		}
	}
}
