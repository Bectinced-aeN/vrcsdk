using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Amazon.S3.Util
{
	public static class AmazonS3Util
	{
		private static Dictionary<string, string> extensionToMime = new Dictionary<string, string>(200, StringComparer.OrdinalIgnoreCase)
		{
			{
				".ai",
				"application/postscript"
			},
			{
				".aif",
				"audio/x-aiff"
			},
			{
				".aifc",
				"audio/x-aiff"
			},
			{
				".aiff",
				"audio/x-aiff"
			},
			{
				".asc",
				"text/plain"
			},
			{
				".au",
				"audio/basic"
			},
			{
				".avi",
				"video/x-msvideo"
			},
			{
				".bcpio",
				"application/x-bcpio"
			},
			{
				".bin",
				"application/octet-stream"
			},
			{
				".c",
				"text/plain"
			},
			{
				".cc",
				"text/plain"
			},
			{
				".ccad",
				"application/clariscad"
			},
			{
				".cdf",
				"application/x-netcdf"
			},
			{
				".class",
				"application/octet-stream"
			},
			{
				".cpio",
				"application/x-cpio"
			},
			{
				".cpp",
				"text/plain"
			},
			{
				".cpt",
				"application/mac-compactpro"
			},
			{
				".cs",
				"text/plain"
			},
			{
				".csh",
				"application/x-csh"
			},
			{
				".css",
				"text/css"
			},
			{
				".dcr",
				"application/x-director"
			},
			{
				".dir",
				"application/x-director"
			},
			{
				".dms",
				"application/octet-stream"
			},
			{
				".doc",
				"application/msword"
			},
			{
				".docx",
				"application/vnd.openxmlformats-officedocument.wordprocessingml.document"
			},
			{
				".dot",
				"application/msword"
			},
			{
				".drw",
				"application/drafting"
			},
			{
				".dvi",
				"application/x-dvi"
			},
			{
				".dwg",
				"application/acad"
			},
			{
				".dxf",
				"application/dxf"
			},
			{
				".dxr",
				"application/x-director"
			},
			{
				".eps",
				"application/postscript"
			},
			{
				".etx",
				"text/x-setext"
			},
			{
				".exe",
				"application/octet-stream"
			},
			{
				".ez",
				"application/andrew-inset"
			},
			{
				".f",
				"text/plain"
			},
			{
				".f90",
				"text/plain"
			},
			{
				".fli",
				"video/x-fli"
			},
			{
				".gif",
				"image/gif"
			},
			{
				".gtar",
				"application/x-gtar"
			},
			{
				".gz",
				"application/x-gzip"
			},
			{
				".h",
				"text/plain"
			},
			{
				".hdf",
				"application/x-hdf"
			},
			{
				".hh",
				"text/plain"
			},
			{
				".hqx",
				"application/mac-binhex40"
			},
			{
				".htm",
				"text/html"
			},
			{
				".html",
				"text/html"
			},
			{
				".ice",
				"x-conference/x-cooltalk"
			},
			{
				".ief",
				"image/ief"
			},
			{
				".iges",
				"model/iges"
			},
			{
				".igs",
				"model/iges"
			},
			{
				".ips",
				"application/x-ipscript"
			},
			{
				".ipx",
				"application/x-ipix"
			},
			{
				".jpe",
				"image/jpeg"
			},
			{
				".jpeg",
				"image/jpeg"
			},
			{
				".jpg",
				"image/jpeg"
			},
			{
				".js",
				"application/x-javascript"
			},
			{
				".kar",
				"audio/midi"
			},
			{
				".latex",
				"application/x-latex"
			},
			{
				".lha",
				"application/octet-stream"
			},
			{
				".lsp",
				"application/x-lisp"
			},
			{
				".lzh",
				"application/octet-stream"
			},
			{
				".m",
				"text/plain"
			},
			{
				".m3u8",
				"application/x-mpegURL"
			},
			{
				".man",
				"application/x-troff-man"
			},
			{
				".me",
				"application/x-troff-me"
			},
			{
				".mesh",
				"model/mesh"
			},
			{
				".mid",
				"audio/midi"
			},
			{
				".midi",
				"audio/midi"
			},
			{
				".mime",
				"www/mime"
			},
			{
				".mov",
				"video/quicktime"
			},
			{
				".movie",
				"video/x-sgi-movie"
			},
			{
				".mp2",
				"audio/mpeg"
			},
			{
				".mp3",
				"audio/mpeg"
			},
			{
				".mpe",
				"video/mpeg"
			},
			{
				".mpeg",
				"video/mpeg"
			},
			{
				".mpg",
				"video/mpeg"
			},
			{
				".mpga",
				"audio/mpeg"
			},
			{
				".ms",
				"application/x-troff-ms"
			},
			{
				".msi",
				"application/x-ole-storage"
			},
			{
				".msh",
				"model/mesh"
			},
			{
				".nc",
				"application/x-netcdf"
			},
			{
				".oda",
				"application/oda"
			},
			{
				".pbm",
				"image/x-portable-bitmap"
			},
			{
				".pdb",
				"chemical/x-pdb"
			},
			{
				".pdf",
				"application/pdf"
			},
			{
				".pgm",
				"image/x-portable-graymap"
			},
			{
				".pgn",
				"application/x-chess-pgn"
			},
			{
				".png",
				"image/png"
			},
			{
				".pnm",
				"image/x-portable-anymap"
			},
			{
				".pot",
				"application/mspowerpoint"
			},
			{
				".ppm",
				"image/x-portable-pixmap"
			},
			{
				".pps",
				"application/mspowerpoint"
			},
			{
				".ppt",
				"application/mspowerpoint"
			},
			{
				".pptx",
				"application/vnd.openxmlformats-officedocument.presentationml.presentation"
			},
			{
				".ppz",
				"application/mspowerpoint"
			},
			{
				".pre",
				"application/x-freelance"
			},
			{
				".prt",
				"application/pro_eng"
			},
			{
				".ps",
				"application/postscript"
			},
			{
				".qt",
				"video/quicktime"
			},
			{
				".ra",
				"audio/x-realaudio"
			},
			{
				".ram",
				"audio/x-pn-realaudio"
			},
			{
				".ras",
				"image/cmu-raster"
			},
			{
				".rgb",
				"image/x-rgb"
			},
			{
				".rm",
				"audio/x-pn-realaudio"
			},
			{
				".roff",
				"application/x-troff"
			},
			{
				".rpm",
				"audio/x-pn-realaudio-plugin"
			},
			{
				".rtf",
				"text/rtf"
			},
			{
				".rtx",
				"text/richtext"
			},
			{
				".scm",
				"application/x-lotusscreencam"
			},
			{
				".set",
				"application/set"
			},
			{
				".sgm",
				"text/sgml"
			},
			{
				".sgml",
				"text/sgml"
			},
			{
				".sh",
				"application/x-sh"
			},
			{
				".shar",
				"application/x-shar"
			},
			{
				".silo",
				"model/mesh"
			},
			{
				".sit",
				"application/x-stuffit"
			},
			{
				".skd",
				"application/x-koan"
			},
			{
				".skm",
				"application/x-koan"
			},
			{
				".skp",
				"application/x-koan"
			},
			{
				".skt",
				"application/x-koan"
			},
			{
				".smi",
				"application/smil"
			},
			{
				".smil",
				"application/smil"
			},
			{
				".snd",
				"audio/basic"
			},
			{
				".sol",
				"application/solids"
			},
			{
				".spl",
				"application/x-futuresplash"
			},
			{
				".src",
				"application/x-wais-source"
			},
			{
				".step",
				"application/STEP"
			},
			{
				".stl",
				"application/SLA"
			},
			{
				".stp",
				"application/STEP"
			},
			{
				".sv4cpio",
				"application/x-sv4cpio"
			},
			{
				".sv4crc",
				"application/x-sv4crc"
			},
			{
				".svg",
				"image/svg+xml"
			},
			{
				".swf",
				"application/x-shockwave-flash"
			},
			{
				".t",
				"application/x-troff"
			},
			{
				".tar",
				"application/x-tar"
			},
			{
				".tcl",
				"application/x-tcl"
			},
			{
				".tex",
				"application/x-tex"
			},
			{
				".tif",
				"image/tiff"
			},
			{
				".tiff",
				"image/tiff"
			},
			{
				".tr",
				"application/x-troff"
			},
			{
				".ts",
				"video/MP2T"
			},
			{
				".tsi",
				"audio/TSP-audio"
			},
			{
				".tsp",
				"application/dsptype"
			},
			{
				".tsv",
				"text/tab-separated-values"
			},
			{
				".txt",
				"text/plain"
			},
			{
				".unv",
				"application/i-deas"
			},
			{
				".ustar",
				"application/x-ustar"
			},
			{
				".vcd",
				"application/x-cdlink"
			},
			{
				".vda",
				"application/vda"
			},
			{
				".vrml",
				"model/vrml"
			},
			{
				".wav",
				"audio/x-wav"
			},
			{
				".wrl",
				"model/vrml"
			},
			{
				".xbm",
				"image/x-xbitmap"
			},
			{
				".xlc",
				"application/vnd.ms-excel"
			},
			{
				".xll",
				"application/vnd.ms-excel"
			},
			{
				".xlm",
				"application/vnd.ms-excel"
			},
			{
				".xls",
				"application/vnd.ms-excel"
			},
			{
				".xlsx",
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
			},
			{
				".xlw",
				"application/vnd.ms-excel"
			},
			{
				".xml",
				"text/xml"
			},
			{
				".xpm",
				"image/x-xpixmap"
			},
			{
				".xwd",
				"image/x-xwindowdump"
			},
			{
				".xyz",
				"chemical/x-pdb"
			},
			{
				".zip",
				"application/zip"
			},
			{
				".m4v",
				"video/x-m4v"
			},
			{
				".webm",
				"video/webm"
			},
			{
				".ogv",
				"video/ogv"
			},
			{
				".xap",
				"application/x-silverlight-app"
			},
			{
				".mp4",
				"video/mp4"
			},
			{
				".wmv",
				"video/x-ms-wmv"
			}
		};

		public static string FormattedCurrentTimestamp => AWSSDKUtils.get_FormattedCurrentTimestampGMT();

		public static string MimeTypeFromExtension(string ext)
		{
			if (extensionToMime.ContainsKey(ext))
			{
				return extensionToMime[ext];
			}
			return "application/octet-stream";
		}

		public static string UrlEncode(string data, bool path)
		{
			return AWSSDKUtils.UrlEncode(data, path);
		}

		public static Stream MakeStreamSeekable(Stream input)
		{
			MemoryStream memoryStream = new MemoryStream();
			byte[] buffer = new byte[32768];
			int num = 0;
			using (input)
			{
				while ((num = input.Read(buffer, 0, 32768)) > 0)
				{
					memoryStream.Write(buffer, 0, num);
				}
			}
			memoryStream.Position = 0L;
			return memoryStream;
		}

		public static string GenerateChecksumForContent(string content, bool fBase64Encode)
		{
			byte[] array = CryptoUtilFactory.get_CryptoInstance().ComputeMD5Hash(Encoding.UTF8.GetBytes(content));
			if (fBase64Encode)
			{
				return Convert.ToBase64String(array);
			}
			return BitConverter.ToString(array).Replace("-", string.Empty);
		}

		internal static string ComputeEncodedMD5FromEncodedString(string base64EncodedString)
		{
			byte[] array = Convert.FromBase64String(base64EncodedString);
			return Convert.ToBase64String(CryptoUtilFactory.get_CryptoInstance().ComputeMD5Hash(array));
		}

		internal static void SetMetadataHeaders(IRequest request, MetadataCollection metadata)
		{
			foreach (string key in metadata.Keys)
			{
				request.get_Headers()[key] = metadata[key];
			}
		}

		internal static DateTime? ParseExpiresHeader(string rawValue, string requestId)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(rawValue))
			{
				try
				{
					return S3Transforms.ToDateTime(rawValue);
				}
				catch (FormatException ex)
				{
					throw new AmazonDateTimeUnmarshallingException(requestId, string.Empty, string.Empty, rawValue, string.Format(CultureInfo.InvariantCulture, "The value {0} cannot be converted to a DateTime instance.", rawValue), (Exception)ex);
				}
			}
			return default(DateTime);
		}

		public static bool ValidateV2Bucket(string bucketName)
		{
			if (string.IsNullOrEmpty(bucketName))
			{
				throw new ArgumentNullException("bucketName", "Please specify a bucket name");
			}
			if (bucketName.StartsWith("s3.amazonaws.com", StringComparison.Ordinal))
			{
				return false;
			}
			int num = bucketName.IndexOf(".s3.amazonaws.com", StringComparison.Ordinal);
			if (num > 0)
			{
				bucketName = bucketName.Substring(0, num);
			}
			if (bucketName.Length < 3 || bucketName.Length > 63 || bucketName.StartsWith(".", StringComparison.Ordinal) || bucketName.EndsWith(".", StringComparison.Ordinal))
			{
				return false;
			}
			if (new Regex("^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$").IsMatch(bucketName))
			{
				return false;
			}
			Regex regex = new Regex("^[a-z0-9]([a-z0-9\\-]*[a-z0-9])?$");
			string[] array = bucketName.Split("\\.".ToCharArray());
			foreach (string input in array)
			{
				if (!regex.IsMatch(input))
				{
					return false;
				}
			}
			return true;
		}

		internal static void AddQueryStringParameter(StringBuilder queryString, string parameterName, string parameterValue)
		{
			AddQueryStringParameter(queryString, parameterName, parameterValue, null);
		}

		internal static void AddQueryStringParameter(StringBuilder queryString, string parameterName, string parameterValue, IDictionary<string, string> parameterMap)
		{
			if (queryString.Length > 0)
			{
				queryString.Append("&");
			}
			queryString.AppendFormat("{0}={1}", parameterName, parameterValue);
			parameterMap?.Add(parameterName, parameterValue);
		}

		internal static string TagSetToQueryString(List<Tag> tags)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Tag tag in tags)
			{
				AddQueryStringParameter(stringBuilder, tag.Key, tag.Value);
			}
			return stringBuilder.ToString();
		}

		internal static void SerializeTagToXml(XmlWriter xmlWriter, Tag tag)
		{
			xmlWriter.WriteStartElement("Tag", "");
			if (tag.IsSetKey())
			{
				xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(tag.Key));
			}
			if (tag.IsSetValue())
			{
				xmlWriter.WriteElementString("Value", "", S3Transforms.ToXmlStringValue(tag.Value));
			}
			xmlWriter.WriteEndElement();
		}

		internal static void SerializeTagSetToXml(XmlWriter xmlWriter, List<Tag> tagset)
		{
			xmlWriter.WriteStartElement("TagSet", "");
			if (tagset != null && tagset.Count > 0)
			{
				foreach (Tag item in tagset)
				{
					SerializeTagToXml(xmlWriter, item);
				}
			}
			xmlWriter.WriteEndElement();
		}

		internal static string SerializeTaggingToXml(Tagging tagging)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("Tagging", "");
				SerializeTagSetToXml(xmlWriter, tagging.TagSet);
				xmlWriter.WriteEndElement();
			}
			return stringWriter.ToString();
		}

		internal static void ParseAmzRestoreHeader(string header, out bool restoreInProgress, out DateTime? restoreExpiration)
		{
			restoreExpiration = null;
			restoreInProgress = false;
			if (header != null)
			{
				int num = header.IndexOf("ongoing-request", StringComparison.Ordinal);
				if (num != -1)
				{
					int num2 = header.IndexOf('"', num) + 1;
					int num3 = header.IndexOf('"', num2 + 1);
					if (bool.TryParse(header.Substring(num2, num3 - num2), out bool result))
					{
						restoreInProgress = result;
					}
				}
				num = header.IndexOf("expiry-date", StringComparison.Ordinal);
				if (num != -1)
				{
					int num4 = header.IndexOf('"', num) + 1;
					int num5 = header.IndexOf('"', num4 + 1);
					if (DateTime.TryParseExact(header.Substring(num4, num5 - num4), "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result2))
					{
						restoreExpiration = result2;
					}
				}
			}
		}
	}
}
