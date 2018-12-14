using System;

namespace Amazon.Runtime.Internal
{
	public class UploadHandlerRawWrapper : IDisposable
	{
		private static Type uploadHandlerRawType;

		private bool disposedValue;

		public object Instance
		{
			get;
			private set;
		}

		static UploadHandlerRawWrapper()
		{
			uploadHandlerRawType = Type.GetType("UnityEngine.Networking.UploadHandlerRaw, UnityEngine");
			if (uploadHandlerRawType == null)
			{
				uploadHandlerRawType = Type.GetType("UnityEngine.Experimental.Networking.UploadHandlerRaw, UnityEngine");
			}
		}

		public UploadHandlerRawWrapper(byte[] data)
		{
			Instance = Activator.CreateInstance(uploadHandlerRawType, data);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					(Instance as IDisposable)?.Dispose();
				}
				Instance = null;
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}
	}
}
