using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ThirdParty.iOS4Unity
{
	public class NSData : NSObject
	{
		private class UnmanagedMemoryStreamWithRef : UnmanagedMemoryStream
		{
			private NSData _data;

			public unsafe UnmanagedMemoryStreamWithRef(NSData source)
				: base((byte*)(void*)source.Bytes, source.Length)
			{
				_data = source;
			}

			protected override void Dispose(bool disposing)
			{
				_data = null;
				base.Dispose(disposing);
			}
		}

		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public IntPtr Bytes => ObjC.MessageSendIntPtr(Handle, Selector.GetHandle("bytes"));

		public uint Length
		{
			get
			{
				return ObjC.MessageSendUInt(Handle, Selector.GetHandle("length"));
			}
			set
			{
				throw new NotImplementedException("Not available on NSData, only available on NSMutableData");
			}
		}

		public byte this[int index]
		{
			get
			{
				if (index < 0 || index > Length)
				{
					throw new ArgumentException("idx");
				}
				return Marshal.ReadByte(new IntPtr(Bytes.ToInt64() + index));
			}
			set
			{
				throw new NotImplementedException("NSData arrays can not be modified, use an NSMutableData instead");
			}
		}

		static NSData()
		{
			_classHandle = ObjC.GetClass("NSData");
		}

		internal NSData(IntPtr handle)
			: base(handle)
		{
		}

		public unsafe static NSData FromArray(byte[] buffer)
		{
			if (buffer.Length != 0)
			{
				fixed (IntPtr* value = (IntPtr*)(&buffer[0]))
				{
					return FromBytes((IntPtr)(void*)value, (uint)buffer.Length);
				}
			}
			return FromBytes(IntPtr.Zero, 0u);
		}

		public static NSData FromBytes(IntPtr bytes, uint size)
		{
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithBytes:length:"), bytes, size));
		}

		public static NSData FromData(NSData source)
		{
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithData:"), source.Handle));
		}

		public static NSData FromBytesNoCopy(IntPtr bytes, uint size)
		{
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithBytesNoCopy:length:"), bytes, size));
		}

		public static NSData FromBytesNoCopy(IntPtr bytes, uint size, bool freeWhenDone)
		{
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithBytesNoCopy:length:freeWhenDone:"), bytes, size, freeWhenDone));
		}

		public static NSData FromFile(string path, NSDataReadingOptions mask, out NSError error)
		{
			path = Path.Combine(Application.get_streamingAssetsPath(), path);
			IntPtr arg;
			NSData nSObject = Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithContentsOfFile:options:error:"), path, (uint)mask, out arg));
			error = ((arg == IntPtr.Zero) ? null : Runtime.GetNSObject<NSError>(arg));
			return nSObject;
		}

		public static NSData FromFile(string path)
		{
			path = Path.Combine(Application.get_streamingAssetsPath(), path);
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("dataWithContentsOfFile:"), path));
		}

		public static NSData FromUrl(string url)
		{
			return Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr_NSUrl(_classHandle, Selector.GetHandle("dataWithContentsOfURL:"), url));
		}

		public static NSData FromUrl(string url, NSDataReadingOptions mask, out NSError error)
		{
			IntPtr arg;
			NSData nSObject = Runtime.GetNSObject<NSData>(ObjC.MessageSendIntPtr_NSUrl(_classHandle, Selector.GetHandle("dataWithContentsOfURL:options:error:"), url, (uint)mask, out arg));
			error = ((arg == IntPtr.Zero) ? null : Runtime.GetNSObject<NSError>(arg));
			return nSObject;
		}

		public bool Save(string path, bool atomically = true)
		{
			return ObjC.MessageSendBool(Handle, Selector.GetHandle("writeToFile:atomically:"), path, atomically);
		}

		public Stream AsStream()
		{
			return new UnmanagedMemoryStreamWithRef(this);
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[Length];
			Marshal.Copy(Bytes, array, 0, array.Length);
			return array;
		}
	}
}
