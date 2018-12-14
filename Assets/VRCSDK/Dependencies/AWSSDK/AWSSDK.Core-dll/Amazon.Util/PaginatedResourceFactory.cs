using Amazon.Util.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Amazon.Util
{
	public static class PaginatedResourceFactory
	{
		public static object Create<TItemType, TRequestType, TResponseType>(PaginatedResourceInfo pri)
		{
			pri.Verify();
			return Create<TItemType, TRequestType, TResponseType>(pri.Client, pri.MethodName, pri.Request, pri.TokenRequestPropertyPath, pri.TokenResponsePropertyPath, pri.ItemListPropertyPath);
		}

		private static PaginatedResource<ItemType> Create<ItemType, TRequestType, TResponseType>(object client, string methodName, object request, string tokenRequestPropertyPath, string tokenResponsePropertyPath, string itemListPropertyPath)
		{
			ITypeInfo typeInfo = TypeFactory.GetTypeInfo(client.GetType());
			MethodInfo fetcherMethod = typeInfo.GetMethod(methodName, new ITypeInfo[1]
			{
				TypeFactory.GetTypeInfo(typeof(TRequestType))
			});
			GetFuncType<TRequestType, TResponseType>();
			return Create<ItemType, TRequestType, TResponseType>((TRequestType req) => (TResponseType)fetcherMethod.Invoke(client, new object[1]
			{
				req
			}), (TRequestType)request, tokenRequestPropertyPath, tokenResponsePropertyPath, itemListPropertyPath);
		}

		private static PaginatedResource<ItemType> Create<ItemType, TRequestType, TResponseType>(Func<TRequestType, TResponseType> call, TRequestType request, string tokenRequestPropertyPath, string tokenResponsePropertyPath, string itemListPropertyPath)
		{
			return new PaginatedResource<ItemType>(delegate(string token)
			{
				SetPropertyValueAtPath(request, tokenRequestPropertyPath, token);
				TResponseType val = call(request);
				return new Marker<ItemType>(nextToken: GetPropertyValueFromPath<string>(val, tokenResponsePropertyPath), data: GetPropertyValueFromPath<List<ItemType>>(val, itemListPropertyPath));
			});
		}

		private static void SetPropertyValueAtPath(object instance, string path, string value)
		{
			string[] array = path.Split('.');
			object obj = instance;
			Type type = instance.GetType();
			int i;
			for (i = 0; i < array.Length - 1; i++)
			{
				string name = array[i];
				PropertyInfo property = TypeFactory.GetTypeInfo(type).GetProperty(name);
				obj = property.GetValue(obj, null);
				type = property.PropertyType;
			}
			TypeFactory.GetTypeInfo(type).GetProperty(array[i]).SetValue(obj, value, null);
		}

		private static T GetPropertyValueFromPath<T>(object instance, string path)
		{
			string[] array = path.Split('.');
			object obj = instance;
			Type type = instance.GetType();
			string[] array2 = array;
			foreach (string name in array2)
			{
				PropertyInfo property = TypeFactory.GetTypeInfo(type).GetProperty(name);
				obj = property.GetValue(obj, null);
				type = property.PropertyType;
			}
			return (T)obj;
		}

		internal static Type GetPropertyTypeFromPath(Type start, string path)
		{
			string[] array = path.Split('.');
			Type type = start;
			string[] array2 = array;
			foreach (string name in array2)
			{
				type = TypeFactory.GetTypeInfo(type).GetProperty(name).PropertyType;
			}
			return type;
		}

		private static Type GetFuncType<T, U>()
		{
			return typeof(Func<T, U>);
		}

		internal static T Cast<T>(object o)
		{
			return (T)o;
		}
	}
}
