using Amazon.Util.Internal;
using System.Globalization;
using System.Reflection;

namespace Amazon.Runtime.Internal
{
	public static class ServiceClientHelpers
	{
		public const string S3_ASSEMBLY_NAME = "AWSSDK.S3";

		public const string S3_SERVICE_CLASS_NAME = "Amazon.S3.AmazonS3Client";

		public const string STS_ASSEMBLY_NAME = "AWSSDK.SecurityToken";

		public const string STS_SERVICE_CLASS_NAME = "Amazon.SecurityToken.AmazonSecurityTokenServiceClient";

		public const string STS_SERVICE_CONFIG_NAME = "Amazon.SecurityToken.AmazonSecurityTokenServiceConfig";

		public static TClient CreateServiceFromAnother<TClient, TConfig>(AmazonServiceClient originalServiceClient) where TClient : AmazonServiceClient where TConfig : ClientConfig, new()
		{
			AWSCredentials credentials = originalServiceClient.Credentials;
			TConfig val = originalServiceClient.CloneConfig<TConfig>();
			return TypeFactory.GetTypeInfo(typeof(TClient)).GetConstructor(new ITypeInfo[2]
			{
				TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
				TypeFactory.GetTypeInfo(val.GetType())
			}).Invoke(new object[2]
			{
				credentials,
				val
			}) as TClient;
		}

		public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName, RegionEndpoint region) where TClient : class
		{
			return LoadServiceClientType(assemblyName, serviceClientClassName).GetConstructor(new ITypeInfo[1]
			{
				TypeFactory.GetTypeInfo(typeof(RegionEndpoint))
			}).Invoke(new object[1]
			{
				region
			}) as TClient;
		}

		public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName, AWSCredentials credentials, RegionEndpoint region) where TClient : class
		{
			return LoadServiceClientType(assemblyName, serviceClientClassName).GetConstructor(new ITypeInfo[2]
			{
				TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
				TypeFactory.GetTypeInfo(typeof(RegionEndpoint))
			}).Invoke(new object[2]
			{
				credentials,
				region
			}) as TClient;
		}

		public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName, AWSCredentials credentials, ClientConfig config) where TClient : class
		{
			return LoadServiceClientType(assemblyName, serviceClientClassName).GetConstructor(new ITypeInfo[2]
			{
				TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
				TypeFactory.GetTypeInfo(config.GetType())
			}).Invoke(new object[2]
			{
				credentials,
				config
			}) as TClient;
		}

		public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName, AmazonServiceClient originalServiceClient) where TClient : class
		{
			ITypeInfo typeInfo = LoadServiceClientType(assemblyName, serviceClientClassName);
			ClientConfig clientConfig = CreateServiceConfig(assemblyName, serviceClientClassName.Replace("Client", "Config"));
			originalServiceClient.CloneConfig(clientConfig);
			return typeInfo.GetConstructor(new ITypeInfo[2]
			{
				TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
				TypeFactory.GetTypeInfo(clientConfig.GetType())
			}).Invoke(new object[2]
			{
				originalServiceClient.Credentials,
				clientConfig
			}) as TClient;
		}

		public static ClientConfig CreateServiceConfig(string assemblyName, string serviceConfigClassName)
		{
			return LoadServiceConfigType(assemblyName, serviceConfigClassName).GetConstructor(new ITypeInfo[0]).Invoke(new object[0]) as ClientConfig;
		}

		private static ITypeInfo LoadServiceClientType(string assemblyName, string serviceClientClassName)
		{
			Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
			if (assembly == null)
			{
				throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture, "Failed to load assembly containing service client {0}. Be sure to include a reference to {1}.", serviceClientClassName, assemblyName));
			}
			return TypeFactory.GetTypeInfo(assembly.GetType(serviceClientClassName));
		}

		private static ITypeInfo LoadServiceConfigType(string assemblyName, string serviceConfigClassName)
		{
			Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
			if (assembly == null)
			{
				throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture, "Failed to load assembly containing service config {0}. Be sure to include a reference to {1}.", serviceConfigClassName, assemblyName));
			}
			return TypeFactory.GetTypeInfo(assembly.GetType(serviceConfigClassName));
		}
	}
}
