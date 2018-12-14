using Amazon.Util.Internal;
using System;
using System.Globalization;
using System.Reflection;

namespace Amazon.Util
{
	public class PaginatedResourceInfo
	{
		private string tokenRequestPropertyPath;

		private string tokenResponsePropertyPath;

		internal object Client
		{
			get;
			set;
		}

		internal string MethodName
		{
			get;
			set;
		}

		internal object Request
		{
			get;
			set;
		}

		internal string TokenRequestPropertyPath
		{
			get
			{
				string text = tokenRequestPropertyPath;
				if (string.IsNullOrEmpty(text))
				{
					text = "NextToken";
				}
				return text;
			}
			set
			{
				tokenRequestPropertyPath = value;
			}
		}

		internal string TokenResponsePropertyPath
		{
			get
			{
				string text = tokenResponsePropertyPath;
				if (string.IsNullOrEmpty(text))
				{
					text = "{0}";
					if (Client != null && !string.IsNullOrEmpty(MethodName))
					{
						MethodInfo method = TypeFactory.GetTypeInfo(Client.GetType()).GetMethod(MethodName);
						if (method != null)
						{
							Type returnType = method.ReturnType;
							string text2 = returnType.Name;
							if (text2.EndsWith("Response", StringComparison.Ordinal))
							{
								text2 = text2.Substring(0, text2.Length - 8);
							}
							if (TypeFactory.GetTypeInfo(returnType).GetProperty(string.Format(CultureInfo.InvariantCulture, "{0}Result", text2)) != null)
							{
								text = string.Format(CultureInfo.InvariantCulture, text, string.Format(CultureInfo.InvariantCulture, "{0}Result.{1}", text2, "{0}"));
							}
						}
					}
					text = string.Format(CultureInfo.InvariantCulture, text, "NextToken");
				}
				return text;
			}
			set
			{
				tokenResponsePropertyPath = value;
			}
		}

		internal string ItemListPropertyPath
		{
			get;
			set;
		}

		public PaginatedResourceInfo WithClient(object client)
		{
			Client = client;
			return this;
		}

		public PaginatedResourceInfo WithMethodName(string methodName)
		{
			MethodName = methodName;
			return this;
		}

		public PaginatedResourceInfo WithRequest(object request)
		{
			Request = request;
			return this;
		}

		public PaginatedResourceInfo WithTokenRequestPropertyPath(string tokenRequestPropertyPath)
		{
			TokenRequestPropertyPath = tokenRequestPropertyPath;
			return this;
		}

		public PaginatedResourceInfo WithTokenResponsePropertyPath(string tokenResponsePropertyPath)
		{
			TokenResponsePropertyPath = tokenResponsePropertyPath;
			return this;
		}

		public PaginatedResourceInfo WithItemListPropertyPath(string itemListPropertyPath)
		{
			ItemListPropertyPath = itemListPropertyPath;
			return this;
		}

		internal void Verify()
		{
			if (Client == null)
			{
				throw new ArgumentException("PaginatedResourceInfo.Client needs to be set.");
			}
			Type type = Client.GetType();
			MethodInfo method = TypeFactory.GetTypeInfo(type).GetMethod(MethodName, new ITypeInfo[1]
			{
				TypeFactory.GetTypeInfo(Request.GetType())
			});
			if (method == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} has no method called {1}", type.Name, MethodName));
			}
			Type parameterType = method.GetParameters()[0].ParameterType;
			try
			{
				Convert.ChangeType(Request, parameterType, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new ArgumentException("PaginatedResourcInfo.Request is an incompatible type.");
			}
			Type returnType = method.ReturnType;
			VerifyProperty("TokenRequestPropertyPath", parameterType, TokenRequestPropertyPath, typeof(string));
			VerifyProperty("TokenResponsePropertyPath", returnType, TokenResponsePropertyPath, typeof(string));
			VerifyProperty("ItemListPropertyPath", returnType, ItemListPropertyPath, typeof(string), skipTypecheck: true);
		}

		private static void VerifyProperty(string propName, Type start, string path, Type expectedType)
		{
			VerifyProperty(propName, start, path, expectedType, skipTypecheck: false);
		}

		private static void VerifyProperty(string propName, Type start, string path, Type expectedType, bool skipTypecheck)
		{
			Type type = null;
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} must contain a value.", propName));
			}
			try
			{
				type = PaginatedResourceFactory.GetPropertyTypeFromPath(start, path);
			}
			catch (Exception)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} does not exist on {1}", path, start.Name));
			}
			if (!skipTypecheck && type != expectedType)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} on {1} is not of type {2}", path, start.Name, expectedType.Name));
			}
		}
	}
}
