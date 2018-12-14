using System.Text;
using UnityEngine;

namespace VRC.Compression
{
	public class GZipTest : MonoBehaviour
	{
		public GZipTest()
			: this()
		{
		}

		private void Start()
		{
			RunAssetBundleCompressionTest();
		}

		private void RunBasicTests()
		{
			byte[] bytes = Encoding.ASCII.GetBytes(new string('X', 10000));
			string text = Application.get_persistentDataPath() + "/compressedTest.gz";
			string text2 = Application.get_persistentDataPath() + "/compressedTest.txt";
			string compressedPath = text2;
			GZip.CompressToFile(bytes, text);
			if (GZip.IsValid(text))
			{
				Debug.Log((object)"Test 1 : PASS");
				GZip.DecompressToFile(text, text2);
			}
			else
			{
				Debug.Log((object)"Test 1 : FAIL");
			}
			if (!GZip.IsValid(compressedPath))
			{
				Debug.Log((object)"Test 2 : PASS");
				GZip.DecompressToFile(text, text2);
			}
			else
			{
				Debug.Log((object)"Test 2 : FAIL");
			}
		}

		private void RunAssetBundleCompressionTest()
		{
			Debug.Log((object)"Running Asset Bundle Compression Test");
			string uncompressedPath = Application.get_persistentDataPath() + "/customavatar.unity3d";
			string compressedPath = Application.get_persistentDataPath() + "/myAvatar.vrca";
			GZip.CompressToFile(uncompressedPath, compressedPath);
		}

		private void RunImageCompressionTest()
		{
			Debug.Log((object)"Running Image Compression Test");
			string uncompressedPath = Application.get_persistentDataPath() + "/test.png";
			string compressedPath = Application.get_persistentDataPath() + "/test.gz";
			GZip.CompressToFile(uncompressedPath, compressedPath);
		}
	}
}
