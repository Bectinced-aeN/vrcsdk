using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amazon.Util.Internal
{
	public interface ITypeInfo
	{
		Type BaseType
		{
			get;
		}

		Type Type
		{
			get;
		}

		Assembly Assembly
		{
			get;
		}

		bool IsArray
		{
			get;
		}

		bool IsEnum
		{
			get;
		}

		bool IsClass
		{
			get;
		}

		bool IsValueType
		{
			get;
		}

		bool IsInterface
		{
			get;
		}

		bool IsAbstract
		{
			get;
		}

		bool IsSealed
		{
			get;
		}

		string FullName
		{
			get;
		}

		string Name
		{
			get;
		}

		bool IsGenericTypeDefinition
		{
			get;
		}

		bool IsGenericType
		{
			get;
		}

		bool ContainsGenericParameters
		{
			get;
		}

		Array ArrayCreateInstance(int length);

		Type GetInterface(string name);

		Type[] GetInterfaces();

		IEnumerable<PropertyInfo> GetProperties();

		IEnumerable<FieldInfo> GetFields();

		FieldInfo GetField(string name);

		MethodInfo GetMethod(string name);

		MethodInfo GetMethod(string name, ITypeInfo[] paramTypes);

		MemberInfo[] GetMembers();

		ConstructorInfo GetConstructor(ITypeInfo[] paramTypes);

		PropertyInfo GetProperty(string name);

		bool IsAssignableFrom(ITypeInfo typeInfo);

		object EnumToObject(object value);

		ITypeInfo EnumGetUnderlyingType();

		object CreateInstance();

		ITypeInfo GetElementType();

		bool IsType(Type type);

		Type GetGenericTypeDefinition();

		Type[] GetGenericArguments();

		object[] GetCustomAttributes(bool inherit);

		object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit);
	}
}
