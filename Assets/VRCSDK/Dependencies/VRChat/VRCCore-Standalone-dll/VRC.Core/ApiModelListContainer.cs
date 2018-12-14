using System;
using System.Collections.Generic;

namespace VRC.Core
{
	public class ApiModelListContainer<T> : ApiListContainer where T : ApiModel, new()
	{
		public List<T> ResponseModels
		{
			get;
			private set;
		}

		public ApiModelListContainer()
		{
			ResponseModels = new List<T>();
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			ResponseModels = API.ConvertJsonListToModelList<T>(base.ResponseList, ref responseError, base.DataTimestamp);
			return ResponseModels != null;
		}
	}
}
