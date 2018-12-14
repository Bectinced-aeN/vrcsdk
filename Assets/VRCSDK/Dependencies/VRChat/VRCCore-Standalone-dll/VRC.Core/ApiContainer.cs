using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class ApiContainer
	{
		protected string responseError;

		public Action<ApiContainer> OnError;

		public Action<ApiContainer> OnSuccess;

		public bool IsValid
		{
			get;
			private set;
		}

		public int Code
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public object Data
		{
			get;
			protected set;
		}

		public float DataTimestamp
		{
			get;
			protected set;
		}

		public string Error
		{
			get
			{
				if (!string.IsNullOrEmpty(responseError))
				{
					return responseError;
				}
				return GetErrorMessage();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					responseError = value;
				}
				else
				{
					responseError = value;
					Debug.Log((object)("<color=red>" + ((Model == null) ? string.Empty : (Model.GetType().Name + ":" + Model.id + ": ")) + value + "</color>"));
				}
			}
		}

		public ApiModel Model
		{
			get;
			set;
		}

		protected virtual bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!string.IsNullOrEmpty(Error))
			{
				return false;
			}
			return success;
		}

		protected virtual string GetErrorMessage()
		{
			if (!(Data is Dictionary<string, object>))
			{
				return null;
			}
			Dictionary<string, object> dictionary = Data as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("error"))
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["error"];
				if (dictionary2 != null && dictionary2.ContainsKey("message"))
				{
					return (string)dictionary2["message"];
				}
			}
			return null;
		}

		public bool OnComplete(bool success, string endpoint, int responseCode, string responseText, Func<byte[]> readData, Func<string> readTextData, float dataTimestamp = -1f)
		{
			Logger.Log("<color=magenta>API response from " + endpoint + ":\n" + readTextData() + "</color>", DebugLevel.API);
			try
			{
				if (!success || responseCode < 200 || responseCode >= 400)
				{
					Data = Json.Decode(readTextData());
					if (string.IsNullOrEmpty(Error))
					{
						Error = responseText;
					}
				}
				Code = responseCode;
				Text = responseText;
				DataTimestamp = ((!(dataTimestamp >= 0f)) ? Time.get_realtimeSinceStartup() : dataTimestamp);
				IsValid = Validate(success, readData, readTextData);
			}
			catch (Exception ex)
			{
				IsValid = false;
				Error = "Exception in validating response: " + ex.Message;
			}
			return IsValid;
		}
	}
}
