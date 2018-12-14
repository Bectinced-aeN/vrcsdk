using System.Xml.Linq;

namespace Amazon
{
	internal class V4ClientSectionRoot
	{
		private const string s3Key = "s3";

		public V4ClientSection S3
		{
			get;
			set;
		}

		public V4ClientSectionRoot(XElement section)
		{
			if (section != null)
			{
				S3 = AWSConfigs.GetObject<V4ClientSection>(section, "s3");
			}
		}
	}
}
