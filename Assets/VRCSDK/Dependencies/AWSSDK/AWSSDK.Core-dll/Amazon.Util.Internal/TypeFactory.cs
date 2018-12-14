using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amazon.Util.Internal
{
	public static class TypeFactory
	{
		private abstract class AbstractTypeInfo : ITypeInfo
		{
			protected Type _type;

			public Type Type => _type;

			public abstract Type BaseType
			{
				get;
			}

			public abstract Assembly Assembly
			{
				get;
			}

			public abstract bool IsClass
			{
				get;
			}

			public abstract bool IsInterface
			{
				get;
			}

			public abstract bool IsAbstract
			{
				get;
			}

			public abstract bool IsSealed
			{
				get;
			}

			public abstract bool IsEnum
			{
				get;
			}

			public abstract bool IsValueType
			{
				get;
			}

			public abstract bool ContainsGenericParameters
			{
				get;
			}

			public abstract bool IsGenericTypeDefinition
			{
				get;
			}

			public abstract bool IsGenericType
			{
				get;
			}

			public bool IsArray => _type.IsArray;

			public string FullName => _type.FullName;

			public string Name => _type.Name;

			internal AbstractTypeInfo(Type type)
			{
				_type = type;
			}

			public override int GetHashCode()
			{
				return _type.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				AbstractTypeInfo abstractTypeInfo = obj as AbstractTypeInfo;
				if (abstractTypeInfo == null)
				{
					return false;
				}
				return _type.Equals(abstractTypeInfo._type);
			}

			public bool IsType(Type type)
			{
				return _type == type;
			}

			public abstract Type GetInterface(string name);

			public abstract Type[] GetInterfaces();

			public abstract IEnumerable<PropertyInfo> GetProperties();

			public abstract IEnumerable<FieldInfo> GetFields();

			public abstract FieldInfo GetField(string name);

			public abstract MethodInfo GetMethod(string name);

			public abstract MethodInfo GetMethod(string name, ITypeInfo[] paramTypes);

			public abstract MemberInfo[] GetMembers();

			public abstract PropertyInfo GetProperty(string name);

			public abstract bool IsAssignableFrom(ITypeInfo typeInfo);

			public abstract ConstructorInfo GetConstructor(ITypeInfo[] paramTypes);

			public abstract object[] GetCustomAttributes(bool inherit);

			public abstract object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit);

			public abstract Type GetGenericTypeDefinition();

			public abstract Type[] GetGenericArguments();

			public object EnumToObject(object value)
			{
				return Enum.ToObject(_type, value);
			}

			public ITypeInfo EnumGetUnderlyingType()
			{
				return GetTypeInfo(Enum.GetUnderlyingType(_type));
			}

			public object CreateInstance()
			{
				return Activator.CreateInstance(_type);
			}

			public Array ArrayCreateInstance(int length)
			{
				return Array.CreateInstance(_type, length);
			}

			public ITypeInfo GetElementType()
			{
				return GetTypeInfo(_type.GetElementType());
			}
		}

		private class TypeInfoWrapper : AbstractTypeInfo
		{
			public override Type BaseType => _type.BaseType;

			public override bool IsClass => _type.IsClass;

			public override bool IsValueType => _type.IsValueType;

			public override bool IsInterface => _type.IsInterface;

			public override bool IsAbstract => _type.IsAbstract;

			public override bool IsSealed => _type.IsSealed;

			public override bool IsEnum => _type.IsEnum;

			public override bool ContainsGenericParameters => _type.ContainsGenericParameters;

			public override bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;

			public override bool IsGenericType => _type.IsGenericType;

			public override Assembly Assembly => _type.Assembly;

			internal TypeInfoWrapper(Type type)
				: base(type)
			{
			}

			public override FieldInfo GetField(string name)
			{
				return _type.GetField(name);
			}

			public override Type GetInterface(string name)
			{
				return _type.GetInterfaces().FirstOrDefault((Type x) => x.Namespace + "." + x.Name == name);
			}

			public override Type[] GetInterfaces()
			{
				return _type.GetInterfaces();
			}

			public override IEnumerable<PropertyInfo> GetProperties()
			{
				return _type.GetProperties();
			}

			public override IEnumerable<FieldInfo> GetFields()
			{
				return _type.GetFields();
			}

			public override MemberInfo[] GetMembers()
			{
				return _type.GetMembers();
			}

			public override MethodInfo GetMethod(string name)
			{
				return _type.GetMethod(name);
			}

			public override MethodInfo GetMethod(string name, ITypeInfo[] paramTypes)
			{
				Type[] array = new Type[paramTypes.Length];
				for (int i = 0; i < paramTypes.Length; i++)
				{
					array[i] = ((AbstractTypeInfo)paramTypes[i]).Type;
				}
				return _type.GetMethod(name, array);
			}

			public override ConstructorInfo GetConstructor(ITypeInfo[] paramTypes)
			{
				Type[] array = new Type[paramTypes.Length];
				for (int i = 0; i < paramTypes.Length; i++)
				{
					array[i] = ((AbstractTypeInfo)paramTypes[i]).Type;
				}
				return _type.GetConstructor(array);
			}

			public override PropertyInfo GetProperty(string name)
			{
				return _type.GetProperty(name);
			}

			public override bool IsAssignableFrom(ITypeInfo typeInfo)
			{
				return _type.IsAssignableFrom(((AbstractTypeInfo)typeInfo).Type);
			}

			public override Type GetGenericTypeDefinition()
			{
				return _type.GetGenericTypeDefinition();
			}

			public override Type[] GetGenericArguments()
			{
				return _type.GetGenericArguments();
			}

			public override object[] GetCustomAttributes(bool inherit)
			{
				return _type.GetCustomAttributes(inherit);
			}

			public override object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit)
			{
				return _type.GetCustomAttributes(((TypeInfoWrapper)attributeType)._type, inherit);
			}
		}

		public static readonly ITypeInfo[] EmptyTypes = new ITypeInfo[0];

		public static ITypeInfo GetTypeInfo(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return new TypeInfoWrapper(type);
		}
	}
}
