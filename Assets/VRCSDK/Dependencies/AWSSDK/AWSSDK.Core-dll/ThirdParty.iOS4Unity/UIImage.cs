using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ThirdParty.iOS4Unity
{
	public class UIImage : NSObject
	{
		private class UIImageDispatcher : NSObject
		{
			private static readonly IntPtr _classHandle;

			public readonly Action<NSError> Action;

			public override IntPtr ClassHandle => _classHandle;

			static UIImageDispatcher()
			{
				_classHandle = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "__UIImageDispatcher", 0);
			}

			public UIImageDispatcher(Action<NSError> action)
			{
				Action = action;
			}
		}

		private const string SelectorName = "__onSaveToPhotoAlbum:";

		private static readonly IntPtr _classHandle;

		public override IntPtr ClassHandle => _classHandle;

		public float CurrentScale => ObjC.MessageSendFloat(Handle, Selector.GetHandle("scale"));

		public CGSize Size => new CGSize(ObjC.MessageSendCGSize(Handle, "size"));

		[DllImport("/System/Library/Frameworks/UIKit.framework/UIKit")]
		private static extern IntPtr UIImageJPEGRepresentation(IntPtr image, float compressionQuality);

		[DllImport("/System/Library/Frameworks/UIKit.framework/UIKit")]
		private static extern IntPtr UIImagePNGRepresentation(IntPtr image);

		[DllImport("/System/Library/Frameworks/UIKit.framework/UIKit")]
		private static extern void UIImageWriteToSavedPhotosAlbum(IntPtr image, IntPtr obj, IntPtr selector, IntPtr ctx);

		static UIImage()
		{
			_classHandle = ObjC.GetClass("UIImage");
		}

		internal UIImage(IntPtr handle)
			: base(handle)
		{
		}

		public NSData AsJPEG(float compressionQuality = 1f)
		{
			return Runtime.GetNSObject<NSData>(UIImageJPEGRepresentation(Handle, compressionQuality));
		}

		public NSData AsPNG()
		{
			return Runtime.GetNSObject<NSData>(UIImagePNGRepresentation(Handle));
		}

		public static UIImage FromBundle(string name)
		{
			return Runtime.GetNSObject<UIImage>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("imageNamed:"), name));
		}

		public static UIImage FromFile(string filename)
		{
			filename = Path.Combine(Application.get_streamingAssetsPath(), filename);
			return Runtime.GetNSObject<UIImage>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("imageWithContentsOfFile:"), filename));
		}

		public static UIImage LoadFromData(NSData data)
		{
			return Runtime.GetNSObject<UIImage>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("imageWithData:"), data.Handle));
		}

		public static UIImage LoadFromData(NSData data, float scale)
		{
			return Runtime.GetNSObject<UIImage>(ObjC.MessageSendIntPtr(_classHandle, Selector.GetHandle("imageWithData:scale:"), data.Handle, scale));
		}

		public void SaveToPhotosAlbum(Action<NSError> callback = null)
		{
			if (callback == null)
			{
				UIImageWriteToSavedPhotosAlbum(Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
			else
			{
				UIImageDispatcher dispatcher = new UIImageDispatcher(callback);
				Callbacks.Subscribe(dispatcher, "__onSaveToPhotoAlbum:", delegate(IntPtr obj, IntPtr e, IntPtr ctx)
				{
					callback((e == IntPtr.Zero) ? null : Runtime.GetNSObject<NSError>(e));
					dispatcher.Dispose();
				});
				UIImageWriteToSavedPhotosAlbum(Handle, dispatcher.Handle, ObjC.GetSelector("__onSaveToPhotoAlbum:"), IntPtr.Zero);
			}
		}
	}
}
