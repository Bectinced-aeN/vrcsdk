using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiModel : ApiCacheObject
	{
		[Flags]
		public enum SupportedPlatforms
		{
			None = 0x0,
			StandaloneWindows = 0x1,
			Android = 0x2,
			All = 0x3
		}

		protected enum PostOrPutSelect
		{
			Auto,
			Post,
			Put
		}

		private static Dictionary<string, ApiContainer> activeRequests = new Dictionary<string, ApiContainer>();

		public SupportedPlatforms supportedPlatforms = SupportedPlatforms.All;

		private static Dictionary<Type, string[]> requiredProperties = null;

		private static Dictionary<Type, IEnumerable<PropertyInfo>> targetProperties = null;

		private static Dictionary<Type, Dictionary<string, PropertyInfo>> foundProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		[ApiField(Required = false)]
		public string id
		{
			get;
			set;
		}

		public bool Populated
		{
			get;
			private set;
		}

		public string Endpoint
		{
			get;
			protected set;
		}

		public string[] RequiredProperties
		{
			get
			{
				Type type = GetType();
				if (requiredProperties == null)
				{
					requiredProperties = new Dictionary<Type, string[]>();
				}
				if (!requiredProperties.ContainsKey(type))
				{
					requiredProperties.Add(type, (from p in TargetProperties
					where ((ApiFieldAttribute)p.GetCustomAttributes(inherit: false).First((object a) => a is ApiFieldAttribute)).Required
					select FindPropertyName(p)).ToArray());
				}
				return requiredProperties[type];
			}
		}

		private IEnumerable<PropertyInfo> TargetProperties
		{
			get
			{
				Type type = GetType();
				if (targetProperties == null)
				{
					targetProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
				}
				if (!targetProperties.ContainsKey(type))
				{
					targetProperties.Add(type, from p in type.GetProperties()
					where p?.IsDefined(typeof(ApiFieldAttribute), inherit: true) ?? false
					select p);
				}
				return targetProperties[type];
			}
		}

		public ApiModel()
		{
			Endpoint = null;
			Populated = false;
		}

		public ApiModel(string endpoint)
		{
			Endpoint = endpoint;
		}

		public ApiModel(string endpoint, Dictionary<string, object> fields)
			: this(endpoint)
		{
			string Error = null;
			SetApiFieldsFromJson(fields, ref Error);
			if (Error != null)
			{
				Logger.LogFormat(DebugLevel.API, "Error applying fields: {0}", Error);
			}
		}

		public virtual bool ShouldCache()
		{
			return Populated && !string.IsNullOrEmpty(id);
		}

		public virtual bool ShouldClearOnLevelLoad()
		{
			return false;
		}

		public virtual float GetLifeSpan()
		{
			return 3600f;
		}

		public ApiCacheObject Clone()
		{
			return Clone(id);
		}

		public ApiModel Clone(string newID = null)
		{
			return Clone(GetType(), newID);
		}

		public virtual ApiModel Clone(Type targetType = null, string newID = null)
		{
			try
			{
				if (targetType == null)
				{
					targetType = GetType();
				}
				else if (!typeof(ApiModel).IsAssignableFrom(targetType))
				{
					Logger.LogErrorFormat(DebugLevel.API, "Expected an ApiModel type.");
					return null;
				}
				ApiModel apiModel = Activator.CreateInstance(targetType) as ApiModel;
				string Error = null;
				Dictionary<string, object> fields = ExtractApiFields();
				if (!apiModel.SetApiFieldsFromJson(fields, ref Error))
				{
					Logger.LogErrorFormat(DebugLevel.API, "Unable to clone {0}: {1}", targetType.Name, Error);
				}
				if (newID != null)
				{
					apiModel.id = newID;
				}
				apiModel.Endpoint = Endpoint;
				return apiModel;
				IL_00a3:
				ApiModel result;
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return null;
				IL_00b7:
				ApiModel result;
				return result;
			}
		}

		public virtual void Save(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			PostOrPut(onSuccess, onFailure, PostOrPutSelect.Auto);
		}

		public virtual void SaveField(string field, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", id);
			object data = null;
			ReadField(field, ref data);
			dictionary.Add(field, data);
			PostOrPut(onSuccess, onFailure, PostOrPutSelect.Auto, dictionary);
		}

		public virtual void Post(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null)
		{
			PostOrPut(onSuccess, onFailure, PostOrPutSelect.Post, parameters);
		}

		public virtual void Put(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null)
		{
			PostOrPut(onSuccess, onFailure, PostOrPutSelect.Put, parameters);
		}

		public void Fetch(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null, bool disableCache = false)
		{
			Get(onSuccess, onFailure, parameters, disableCache);
		}

		public virtual void Get(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null, bool disableCache = false)
		{
			if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(Endpoint))
			{
				if (onFailure != null)
				{
					onFailure(new ApiContainer
					{
						Error = "Fetch called with null id."
					});
				}
			}
			else
			{
				string key = MakeRequestEndpoint() + Json.Encode(parameters);
				if (activeRequests.ContainsKey(key))
				{
					ApiContainer apiContainer = activeRequests[key];
					Action<ApiContainer> originalSuccess = apiContainer.OnSuccess;
					Action<ApiContainer> onSuccess2 = delegate(ApiContainer c)
					{
						if (activeRequests.ContainsKey(key))
						{
							activeRequests.Remove(key);
						}
						try
						{
							if (onSuccess != null)
							{
								onSuccess(c);
							}
						}
						catch (Exception ex4)
						{
							Debug.LogException(ex4);
						}
						if (originalSuccess != null)
						{
							originalSuccess(c);
						}
					};
					Action<ApiContainer> originalError = apiContainer.OnError;
					Action<ApiContainer> onError = delegate(ApiContainer c)
					{
						if (activeRequests.ContainsKey(key))
						{
							activeRequests.Remove(key);
						}
						try
						{
							if (onFailure != null)
							{
								onFailure(c);
							}
						}
						catch (Exception ex3)
						{
							Debug.LogException(ex3);
						}
						if (originalError != null)
						{
							originalError(c);
						}
					};
					apiContainer.OnSuccess = onSuccess2;
					apiContainer.OnError = onError;
				}
				else
				{
					Action<ApiContainer> onSuccess3 = delegate(ApiContainer c)
					{
						if (activeRequests.ContainsKey(key))
						{
							activeRequests.Remove(key);
						}
						try
						{
							if (onSuccess != null)
							{
								onSuccess(c);
							}
						}
						catch (Exception ex2)
						{
							Debug.LogException(ex2);
						}
						ApiCache.Save(c.Model.id, c.Model, andClone: true);
					};
					Action<ApiContainer> onFailure2 = delegate(ApiContainer c)
					{
						if (activeRequests.ContainsKey(key))
						{
							activeRequests.Remove(key);
						}
						try
						{
							if (onFailure != null)
							{
								onFailure(c);
							}
						}
						catch (Exception ex)
						{
							Debug.LogException(ex);
						}
					};
					ApiContainer apiContainer2 = MakeModelContainer(onSuccess3, onFailure2);
					activeRequests.Add(key, apiContainer2);
					SendGetRequest(apiContainer2, parameters, disableCache);
				}
			}
		}

		protected virtual void PostOrPut(Action<ApiContainer> onSuccess, Action<ApiContainer> onFailure, PostOrPutSelect select, Dictionary<string, object> requestParams = null)
		{
			if (string.IsNullOrEmpty(Endpoint))
			{
				Logger.LogErrorFormat(DebugLevel.API, "Cannot save to null endpoint");
			}
			else
			{
				if (requestParams == null)
				{
					requestParams = ExtractApiFields();
				}
				if (APIUser.CurrentUser == null || !APIUser.CurrentUser.hasSuperPowers)
				{
					List<KeyValuePair<string, object>> list = (from kvp in requestParams
					where IsAdminWritableOnly(FindProperty(kvp.Key))
					select kvp).ToList();
					foreach (KeyValuePair<string, object> item in list)
					{
						requestParams.Remove(item.Key);
					}
				}
				List<KeyValuePair<string, object>> list2 = (from kvp in requestParams
				where IsApiWritableOnly(FindProperty(kvp.Key))
				select kvp).ToList();
				foreach (KeyValuePair<string, object> item2 in list2)
				{
					requestParams.Remove(item2.Key);
				}
				Action<ApiContainer> onSuccess2 = delegate(ApiContainer c)
				{
					ApiCache.Save(c.Model.id, c.Model, andClone: true);
					if (onSuccess != null)
					{
						onSuccess(c);
					}
				};
				switch (select)
				{
				case PostOrPutSelect.Auto:
					if (!string.IsNullOrEmpty(id))
					{
						SendPutRequest(new ApiContainer
						{
							OnSuccess = onSuccess2,
							OnError = onFailure,
							Model = this
						}, requestParams);
					}
					else
					{
						API.SendPostRequest(Endpoint, MakeModelContainer(onSuccess2, onFailure), requestParams);
					}
					break;
				case PostOrPutSelect.Post:
					API.SendPostRequest(Endpoint, MakeModelContainer(onSuccess2, onFailure), requestParams);
					break;
				case PostOrPutSelect.Put:
				{
					ApiModel target = null;
					if (ApiCache.Fetch(GetType(), id + "_copy", ref target, 3.40282347E+38f))
					{
						foreach (KeyValuePair<string, object> item3 in target.ExtractApiFields())
						{
							if (requestParams.ContainsKey(item3.Key))
							{
								if (typeof(IList).IsAssignableFrom(item3.Value.GetType()) && typeof(IList).IsAssignableFrom(requestParams[item3.Key].GetType()))
								{
									IList a = item3.Value as IList;
									IList b = requestParams[item3.Key] as IList;
									if (!b.Cast<object>().Any((object bo) => !a.Contains(bo)) && !a.Cast<object>().Any((object ao) => !b.Contains(ao)))
									{
										requestParams.Remove(item3.Key);
									}
								}
								else if (item3.Value.Equals(requestParams[item3.Key]))
								{
									requestParams.Remove(item3.Key);
								}
							}
						}
					}
					SendPutRequest(new ApiContainer
					{
						OnSuccess = onSuccess2,
						OnError = onFailure,
						Model = this
					}, requestParams);
					break;
				}
				}
			}
		}

		public virtual void Delete(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			if (string.IsNullOrEmpty(id))
			{
				onFailure(new ApiContainer
				{
					Error = "Delete called with null id."
				});
			}
			else
			{
				ApiCache.Invalidate(GetType(), id);
				if (Endpoint == null)
				{
					Logger.LogErrorFormat(DebugLevel.API, "NULL endpoint for {0} object, DELETE ignored.", GetType().Name);
					onFailure?.Invoke(new ApiContainer
					{
						Error = "NULL endpoint for " + GetType().Name + " object, DELETE ignored.",
						Model = this
					});
				}
				else
				{
					API.SendRequest(Endpoint + "/" + id, HTTPMethods.Delete, new ApiContainer
					{
						OnSuccess = onSuccess,
						OnError = onFailure
					}, null, authenticationRequired: true, Application.get_isEditor());
				}
			}
		}

		public bool SetApiFieldsFromJson(Dictionary<string, object> fields)
		{
			string Error = null;
			if (!SetApiFieldsFromJson(fields, ref Error))
			{
				Logger.LogErrorFormat(DebugLevel.API, "Unable to set fields from json: {0}", Error);
				return false;
			}
			return true;
		}

		public virtual bool SetApiFieldsFromJson(Dictionary<string, object> fields, ref string Error)
		{
			List<string> missing = new List<string>();
			foreach (KeyValuePair<string, object> field in fields)
			{
				if (!WriteField(field.Key, field.Value))
				{
					missing.Add(field.Key);
				}
			}
			bool flag = true;
			if (missing.Count > 0)
			{
				Error = "Error writing the following fields: " + string.Join(", ", missing.ToArray());
				string[] source = RequiredProperties;
				flag = (!source.Any((string s) => missing.Contains(s)) && source.All((string s) => fields.Keys.Contains(s)));
			}
			Populated = flag;
			if (flag && fields.Count != TargetProperties.Count() && API.IsDevApi())
			{
				Debug.LogFormat("<color=yellow>{0}: missing fields: {1}</color>\n{2}", new object[3]
				{
					GetType().Name,
					string.Join(", ", (from p in TargetProperties
					where !fields.Keys.Contains(FindPropertyName(p))
					select FindPropertyName(p)).ToArray()),
					Json.Encode(fields)
				});
			}
			return flag;
		}

		public virtual Dictionary<string, object> ExtractApiFields()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (PropertyInfo targetProperty in TargetProperties)
			{
				string text = FindPropertyName(targetProperty);
				object data = null;
				if (ReadField(text, ref data) && data != null)
				{
					if (dictionary.ContainsKey(text))
					{
						dictionary[text] = data;
					}
					else
					{
						dictionary.Add(text, data);
					}
				}
			}
			return dictionary;
		}

		protected virtual ApiContainer MakeModelContainer(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			Type type = GetType();
			Type type2 = typeof(ApiModelContainer<>).MakeGenericType(type);
			ApiContainer apiContainer = Activator.CreateInstance(type2, this) as ApiContainer;
			apiContainer.OnSuccess = onSuccess;
			apiContainer.OnError = onFailure;
			apiContainer.Model = this;
			return apiContainer;
		}

		protected virtual bool ReadField(string fieldName, ref object data)
		{
			PropertyInfo propertyInfo = FindProperty(fieldName);
			if (propertyInfo == null)
			{
				Logger.LogFormat(DebugLevel.API, "{0}: Could not read property {1}", GetType().Name, fieldName);
				return false;
			}
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			data = getMethod.Invoke(this, new object[0]);
			if (data == null)
			{
				return false;
			}
			if (!TryReadConvert(ref data))
			{
				Logger.LogErrorFormat(DebugLevel.API, "{0}: Could not read property due to encoding failure, {1} is a {2}", GetType().Name, fieldName, propertyInfo.PropertyType.FullName);
				return false;
			}
			return true;
		}

		private static bool TryReadConvert(ref object obj)
		{
			Type type = obj.GetType();
			if (typeof(ApiModel).IsAssignableFrom(type))
			{
				obj = (obj as ApiModel).ExtractApiFields();
			}
			else if (!type.IsGenericType || !typeof(IList).IsAssignableFrom(type.GetGenericTypeDefinition()))
			{
				try
				{
					obj = Convert.ChangeType(obj, typeof(string));
				}
				catch (InvalidCastException)
				{
				}
			}
			else if (typeof(ApiModel).IsAssignableFrom(type.GetGenericArguments()[0]))
			{
				List<object> list = new List<object>((obj as IList).Count);
				foreach (object item in obj as IList)
				{
					list.Add((item as ApiModel).ExtractApiFields());
				}
				obj = list;
			}
			else
			{
				List<object> list2 = new List<object>((obj as IList).Count);
				foreach (object item2 in obj as IList)
				{
					list2.Add(item2.ToString());
				}
				obj = list2;
			}
			return true;
		}

		protected virtual bool WriteField(string fieldName, object data)
		{
			PropertyInfo propertyInfo = FindProperty(fieldName);
			if (propertyInfo == null)
			{
				return false;
			}
			try
			{
				if (!TryWriteConvert(propertyInfo.PropertyType, ref data))
				{
					bool success = false;
					if (data is string)
					{
						data = Json.Decode(data as string, ref success);
					}
					if (!success || !TryWriteConvert(propertyInfo.PropertyType, ref data))
					{
						Logger.LogErrorFormat(DebugLevel.API, "{0}: Could not write property due to decoding failure, wanted {1} for {2}\n{3}", GetType().Name, propertyInfo.PropertyType.FullName, fieldName, (data != null) ? data.ToString() : "null");
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogErrorFormat(DebugLevel.API, "{0}: could not write {1} of type {2}\n{3}", GetType().Name, fieldName, propertyInfo.PropertyType.Name, ex.Message);
				return false;
				IL_00f0:;
			}
			try
			{
				MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
				if (setMethod == null)
				{
					return false;
				}
				setMethod.Invoke(this, new object[1]
				{
					data
				});
			}
			catch (Exception ex2)
			{
				Logger.LogErrorFormat(DebugLevel.API, "{0}: failed to set {1}\n", GetType().Name, fieldName, ex2.Message);
				return false;
				IL_015e:;
			}
			return true;
		}

		private static bool TryWriteConvert(Type targetType, ref object data)
		{
			if (data == null)
			{
				return false;
			}
			if (targetType == data.GetType())
			{
				return true;
			}
			if (data is object[] && typeof(IList).IsAssignableFrom(targetType))
			{
				data = ((object[])data).ToList();
			}
			try
			{
				data = Convert.ChangeType(data, targetType);
				return true;
				IL_0059:;
			}
			catch (InvalidCastException)
			{
			}
			if (targetType.IsEnum && data is string)
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((data as string).ToLower());
				data = Enum.Parse(targetType, value, ignoreCase: true);
			}
			else if (data is List<object> && targetType == typeof(List<string>))
			{
				data = Tools.ObjListToStringList(data as List<object>);
			}
			else if (data is Dictionary<string, object> && typeof(ApiModel).IsAssignableFrom(targetType))
			{
				MethodInfo methodInfo = typeof(API).GetMethod("CreateFromJson").MakeGenericMethod(targetType);
				data = Convert.ChangeType(methodInfo.Invoke(null, new object[1]
				{
					data as Dictionary<string, object>
				}), targetType);
			}
			else
			{
				if (!(data is List<object>) || !targetType.IsGenericType || targetType.GetGenericTypeDefinition() != typeof(List<>))
				{
					return false;
				}
				if (!typeof(ApiModel).IsAssignableFrom(targetType.GetGenericArguments()[0]))
				{
					try
					{
						Type conversionType = targetType.GetGenericArguments()[0];
						IList list = (IList)Activator.CreateInstance(targetType);
						foreach (object item in data as List<object>)
						{
							list.Add(Convert.ChangeType(item, conversionType));
						}
						data = list;
					}
					catch (InvalidCastException)
					{
						return false;
						IL_02a1:;
					}
				}
				else
				{
					Type type = targetType.GetGenericArguments()[0];
					MethodInfo methodInfo2 = typeof(API).GetMethod("CreateFromJson").MakeGenericMethod(type);
					IList list2 = (IList)Activator.CreateInstance(targetType);
					foreach (object item2 in data as List<object>)
					{
						list2.Add(Convert.ChangeType(methodInfo2.Invoke(null, new object[1]
						{
							item2 as Dictionary<string, object>
						}), type));
					}
					data = list2;
				}
			}
			return true;
		}

		private void SendGetRequest(ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null, bool disableCache = false)
		{
			if (responseContainer == null)
			{
				responseContainer = MakeModelContainer();
			}
			if (Endpoint == null)
			{
				Logger.LogErrorFormat(DebugLevel.API, "NULL endpoint for {0} object, GET ignored.", GetType().Name);
				if (responseContainer.OnError != null)
				{
					responseContainer.Error = "NULL endpoint for " + GetType().Name + " object, GET ignored.";
					responseContainer.OnError(responseContainer);
				}
			}
			else
			{
				API.SendGetRequest(MakeRequestEndpoint(), responseContainer, requestParams, disableCache);
			}
		}

		private void SendPutRequest(ApiContainer responseContainer = null, Dictionary<string, object> requestParams = null)
		{
			if (responseContainer == null)
			{
				responseContainer = MakeModelContainer();
			}
			if (Endpoint == null)
			{
				Logger.LogErrorFormat(DebugLevel.API, "NULL endpoint for {0} object, PUT ignored.", GetType().Name);
				if (responseContainer.OnError != null)
				{
					responseContainer.Error = "NULL endpoint for " + GetType().Name + " object, PUT ignored.";
					responseContainer.OnError(responseContainer);
				}
			}
			else
			{
				API.SendPutRequest(MakeRequestEndpoint(), responseContainer, requestParams);
			}
		}

		protected virtual string MakeRequestEndpoint()
		{
			return Endpoint + ((!string.IsNullOrEmpty(id)) ? ("/" + id) : string.Empty);
		}

		private string FindPropertyName(PropertyInfo pi)
		{
			ApiFieldAttribute apiFieldAttribute = (ApiFieldAttribute)pi.GetCustomAttributes(inherit: true).FirstOrDefault((object a) => a is ApiFieldAttribute);
			return (apiFieldAttribute != null && !string.IsNullOrEmpty(apiFieldAttribute.Name)) ? apiFieldAttribute.Name : pi.Name;
		}

		private PropertyInfo FindProperty(string fieldName)
		{
			Type type = GetType();
			if (!foundProperties.ContainsKey(type))
			{
				foundProperties.Add(type, new Dictionary<string, PropertyInfo>());
			}
			Dictionary<string, PropertyInfo> dictionary = foundProperties[type];
			if (!dictionary.ContainsKey(fieldName))
			{
				dictionary.Add(fieldName, TargetProperties.FirstOrDefault((PropertyInfo p) => FindPropertyName(p).ToLower() == fieldName.ToLower()));
			}
			return dictionary[fieldName];
		}

		private bool IsAdminWritableOnly(PropertyInfo pi)
		{
			if (pi == null)
			{
				return false;
			}
			return ((ApiFieldAttribute)pi.GetCustomAttributes(inherit: true).FirstOrDefault((object a) => a is ApiFieldAttribute))?.IsAdminWritableOnly ?? false;
		}

		private bool IsApiWritableOnly(PropertyInfo pi)
		{
			if (pi == null)
			{
				return false;
			}
			return ((ApiFieldAttribute)pi.GetCustomAttributes(inherit: true).FirstOrDefault((object a) => a is ApiFieldAttribute))?.IsApiWritableOnly ?? false;
		}
	}
}
