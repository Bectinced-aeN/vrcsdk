using System;

namespace VRC.Core
{
	public class ApiModelContainer<T> : ApiDictContainer where T : ApiModel, new()
	{
		public ApiModelContainer(T target)
			: base(target.RequiredProperties)
		{
			if (target == null)
			{
				base.Error = "Provided a null model?!";
			}
			base.Model = target;
		}

		public ApiModelContainer()
			: this(new T())
		{
		}

		protected override bool Validate(bool success, Func<byte[]> readData, Func<string> readTextData)
		{
			if (base.Model == null)
			{
				return false;
			}
			if (!base.Validate(success, readData, readTextData))
			{
				return false;
			}
			try
			{
				if (!base.Model.SetApiFieldsFromJson(base.ResponseDictionary, ref responseError))
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				base.Error = "An exception was caught when filling the model: " + ex.Message + "\n" + ex.StackTrace;
				return false;
				IL_006e:;
			}
			if (!string.IsNullOrEmpty(base.Model.id))
			{
				ApiCache.Save(base.Model.id, base.Model, andClone: true);
			}
			return true;
		}
	}
}
