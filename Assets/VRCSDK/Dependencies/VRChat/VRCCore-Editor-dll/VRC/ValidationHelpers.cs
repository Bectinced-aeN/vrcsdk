using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRC
{
	public class ValidationHelpers
	{
		private static readonly int CONTENT_WORLD_ASSET_BUNDLE_SIZE_LIMIT_PC = -1;

		private static readonly int CONTENT_AVATAR_ASSET_BUNDLE_SIZE_LIMIT_PC = -1;

		private static readonly int CONTENT_WORLD_ASSET_BUNDLE_SIZE_LIMIT_MOBILE = 52428800;

		private static readonly int CONTENT_AVATAR_ASSET_BUNDLE_SIZE_LIMIT_MOBILE = 10485760;

		public static int GetAssetBundleSizeLimit(ContentType contentType)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			bool flag = true;
			if ((int)EditorUserBuildSettings.get_selectedBuildTargetGroup() != 1)
			{
				switch (contentType)
				{
				case ContentType.World:
					return CONTENT_WORLD_ASSET_BUNDLE_SIZE_LIMIT_MOBILE;
				case ContentType.Avatar:
					return CONTENT_AVATAR_ASSET_BUNDLE_SIZE_LIMIT_MOBILE;
				default:
					return -1;
				}
			}
			switch (contentType)
			{
			case ContentType.World:
				return CONTENT_WORLD_ASSET_BUNDLE_SIZE_LIMIT_PC;
			case ContentType.Avatar:
				return CONTENT_AVATAR_ASSET_BUNDLE_SIZE_LIMIT_PC;
			default:
				return -1;
			}
		}

		public static string GetAssetBundleOverSizeLimitMessage(ContentType contentType)
		{
			return GetAssetBundleOverSizeLimitMessage(contentType, -1);
		}

		public static string GetAssetBundleOverSizeLimitMessage(ContentType contentType, int contentSize)
		{
			switch (contentType)
			{
			case ContentType.World:
				return "This world is too large for VRChat to load on this platform. The maximum size is " + FormatFileSize(GetAssetBundleSizeLimit(contentType)) + ". " + ((contentSize <= 0) ? string.Empty : ("Current size: " + FormatFileSize(contentSize)));
			case ContentType.Avatar:
				return "This avatar is too large for VRChat to load on this platform. The maximum size is " + FormatFileSize(GetAssetBundleSizeLimit(contentType)) + ". " + ((contentSize <= 0) ? string.Empty : ("Current size: " + FormatFileSize(contentSize)));
			default:
				return string.Empty;
			}
		}

		public static string GetAssetBundleOverSizeLimitMessageSDKWarning(ContentType contentType, int contentSize)
		{
			switch (contentType)
			{
			case ContentType.World:
				return "The most recent world build was too large for VRChat to load on this platform. Please reduce the number or size of assets in the scene. The maximum size is " + FormatFileSize(GetAssetBundleSizeLimit(contentType)) + ". " + ((contentSize <= 0) ? string.Empty : ("Current size: " + FormatFileSize(contentSize)));
			case ContentType.Avatar:
				return "The most recent avatar build was too large for VRChat to load on this platform. Please reduce the number or size of assets. The maximum size is " + FormatFileSize(GetAssetBundleSizeLimit(contentType)) + ". " + ((contentSize <= 0) ? string.Empty : ("Current size: " + FormatFileSize(contentSize)));
			default:
				return string.Empty;
			}
		}

		public static bool CheckIfAssetBundleFileTooLarge(ContentType contentType, string vrcFilePath, out int fileSize)
		{
			fileSize = 0;
			try
			{
				int assetBundleSizeLimit = GetAssetBundleSizeLimit(contentType);
				if (assetBundleSizeLimit < 0)
				{
					return false;
				}
				if (string.IsNullOrEmpty(vrcFilePath) || !File.Exists(vrcFilePath))
				{
					return false;
				}
				FileInfo fileInfo = new FileInfo(vrcFilePath);
				fileSize = (int)fileInfo.Length;
				if (fileSize > assetBundleSizeLimit)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Couldn't open file: " + vrcFilePath));
				Debug.LogException(ex);
			}
			return false;
		}

		public static bool CheckIfAssetBundleSizeTooLarge(ContentType contentType, int dataSize)
		{
			if (dataSize <= 0)
			{
				return false;
			}
			int assetBundleSizeLimit = GetAssetBundleSizeLimit(contentType);
			if (assetBundleSizeLimit < 0)
			{
				return false;
			}
			return dataSize > assetBundleSizeLimit;
		}

		public static string FormatFileSize(long filesize)
		{
			if (filesize < 0)
			{
				return string.Empty;
			}
			long num = 1024L;
			long num2 = num * 1024;
			long num3 = num2 * 1024;
			long num4 = num3 * 1024;
			long num5;
			string str;
			if (filesize > num4)
			{
				num5 = num4;
				str = "TB";
			}
			else if (filesize > num3)
			{
				num5 = num3;
				str = "GB";
			}
			else if (filesize > num2)
			{
				num5 = num2;
				str = "MB";
			}
			else if (filesize > num)
			{
				num5 = num;
				str = "KB";
			}
			else
			{
				num5 = 1L;
				str = "B";
			}
			return ((float)filesize / (float)num5).ToString("F2") + " " + str;
		}

		public static bool IsMobilePlatform()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			bool flag = true;
			flag = ((int)EditorUserBuildSettings.get_selectedBuildTargetGroup() == 1);
			return !flag;
		}

		public static bool IsStandalonePlatform()
		{
			return !IsMobilePlatform();
		}
	}
}
