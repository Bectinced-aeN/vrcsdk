using System;
using System.Collections.Generic;
using System.Text;

namespace VRC.Core.BestHTTP.SocketIO
{
	internal sealed class SocketOptions
	{
		private float randomizationFactor;

		private string BuiltQueryParams;

		public bool Reconnection
		{
			get;
			set;
		}

		public int ReconnectionAttempts
		{
			get;
			set;
		}

		public TimeSpan ReconnectionDelay
		{
			get;
			set;
		}

		public TimeSpan ReconnectionDelayMax
		{
			get;
			set;
		}

		public float RandomizationFactor
		{
			get
			{
				return randomizationFactor;
			}
			set
			{
				randomizationFactor = Math.Min(1f, Math.Max(0f, value));
			}
		}

		public TimeSpan Timeout
		{
			get;
			set;
		}

		public bool AutoConnect
		{
			get;
			set;
		}

		public Dictionary<string, string> AdditionalQueryParams
		{
			get;
			set;
		}

		public bool QueryParamsOnlyForHandshake
		{
			get;
			set;
		}

		public SocketOptions()
		{
			Reconnection = true;
			ReconnectionAttempts = 2147483647;
			ReconnectionDelay = TimeSpan.FromMilliseconds(1000.0);
			ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000.0);
			RandomizationFactor = 0.5f;
			Timeout = TimeSpan.FromMilliseconds(20000.0);
			AutoConnect = true;
			QueryParamsOnlyForHandshake = true;
		}

		internal string BuildQueryParams()
		{
			if (AdditionalQueryParams == null || AdditionalQueryParams.Count == 0)
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(BuiltQueryParams))
			{
				return BuiltQueryParams;
			}
			StringBuilder stringBuilder = new StringBuilder(AdditionalQueryParams.Count * 4);
			foreach (KeyValuePair<string, string> additionalQueryParam in AdditionalQueryParams)
			{
				stringBuilder.Append("&");
				stringBuilder.Append(additionalQueryParam.Key);
				if (!string.IsNullOrEmpty(additionalQueryParam.Value))
				{
					stringBuilder.Append("=");
					stringBuilder.Append(additionalQueryParam.Value);
				}
			}
			return BuiltQueryParams = stringBuilder.ToString();
		}
	}
}
