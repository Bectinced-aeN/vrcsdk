using Amazon.Util.Internal;
using System.Xml.Linq;

namespace Amazon
{
	public static class AWSConfigsS3
	{
		private const string s3Key = "s3";

		public const string S3UseSignatureVersion4Key = "AWS.S3.UseSignatureVersion4";

		private static bool _useSignatureVersion4;

		public static bool UseSignatureVersion4
		{
			get
			{
				return _useSignatureVersion4;
			}
			set
			{
				UseSigV4SetExplicitly = true;
				_useSignatureVersion4 = value;
			}
		}

		internal static bool UseSigV4SetExplicitly
		{
			get;
			private set;
		}

		static AWSConfigsS3()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			_useSignatureVersion4 = false;
			UseSigV4SetExplicitly = false;
			string config = AWSConfigs.GetConfig("AWS.S3.UseSignatureVersion4");
			if (!string.IsNullOrEmpty(config))
			{
				UseSignatureVersion4 = bool.Parse(config);
			}
			XElement serviceSection = new RootConfig().GetServiceSection("s3");
			if (serviceSection != null)
			{
				V4ClientSectionRoot v4ClientSectionRoot = new V4ClientSectionRoot(serviceSection);
				if (v4ClientSectionRoot.S3 != null)
				{
					V4ClientSection s = v4ClientSectionRoot.S3;
					if (s.get_ElementInformation().get_IsPresent() && s.UseSignatureVersion4.HasValue)
					{
						UseSignatureVersion4 = s.UseSignatureVersion4.Value;
					}
				}
			}
		}
	}
}
