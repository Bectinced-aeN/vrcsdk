using Amazon.Util.Internal;

namespace Amazon
{
	internal class V4ClientSection : ConfigurationElement
	{
		public bool? UseSignatureVersion4
		{
			get;
			set;
		}

		public V4ClientSection()
			: this()
		{
		}
	}
}
