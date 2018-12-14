using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_MetadataListener : MonoBehaviour
	{
		public delegate void MetadataCallback(Dictionary<string, object> data);

		private static MetadataCallback callbacks;

		public MetadataCallback metadataUpdate;

		public VRC_MetadataListener()
			: this()
		{
		}

		private void Awake()
		{
			callbacks = (MetadataCallback)Delegate.Combine(callbacks, new MetadataCallback(MetadataChangedInternal));
		}

		private void OnDestroy()
		{
			callbacks = (MetadataCallback)Delegate.Remove(callbacks, new MetadataCallback(MetadataChangedInternal));
		}

		public static void TriggerUpdate(Dictionary<string, object> data)
		{
			if (callbacks != null)
			{
				callbacks(data);
			}
		}

		private void MetadataChangedInternal(Dictionary<string, object> data)
		{
			if (metadataUpdate != null)
			{
				metadataUpdate(data);
			}
		}
	}
}
