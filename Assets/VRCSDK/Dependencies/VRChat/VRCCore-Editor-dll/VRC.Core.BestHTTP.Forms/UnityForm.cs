using UnityEngine;

namespace VRC.Core.BestHTTP.Forms
{
	internal sealed class UnityForm : HTTPFormBase
	{
		public WWWForm Form
		{
			get;
			set;
		}

		public UnityForm()
		{
		}

		public UnityForm(WWWForm form)
		{
			Form = form;
		}

		public override void CopyFrom(HTTPFormBase fields)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			base.Fields = fields.Fields;
			base.IsChanged = true;
			if (Form == null)
			{
				Form = new WWWForm();
				if (base.Fields != null)
				{
					for (int i = 0; i < base.Fields.Count; i++)
					{
						HTTPFieldData hTTPFieldData = base.Fields[i];
						if (string.IsNullOrEmpty(hTTPFieldData.Text) && hTTPFieldData.Binary != null)
						{
							Form.AddBinaryData(hTTPFieldData.Name, hTTPFieldData.Binary, hTTPFieldData.FileName, hTTPFieldData.MimeType);
						}
						else
						{
							Form.AddField(hTTPFieldData.Name, hTTPFieldData.Text, hTTPFieldData.Encoding);
						}
					}
				}
			}
		}

		public override void PrepareRequest(HTTPRequest request)
		{
			if (Form.get_headers().ContainsKey("Content-Type"))
			{
				request.SetHeader("Content-Type", Form.get_headers()["Content-Type"]);
			}
			else
			{
				request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
			}
		}

		public override byte[] GetData()
		{
			return Form.get_data();
		}
	}
}
