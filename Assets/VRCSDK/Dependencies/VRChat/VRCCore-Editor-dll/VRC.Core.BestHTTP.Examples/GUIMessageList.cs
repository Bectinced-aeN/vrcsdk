using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core.BestHTTP.Examples
{
	internal class GUIMessageList
	{
		private List<string> messages = new List<string>();

		private Vector2 scrollPos;

		public void Draw()
		{
			Draw((float)Screen.get_width(), 0f);
		}

		public void Draw(float minWidth, float minHeight)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.MinHeight(minHeight)
			});
			for (int i = 0; i < messages.Count; i++)
			{
				GUILayout.Label(messages[i], (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.MinWidth(minWidth)
				});
			}
			GUILayout.EndScrollView();
		}

		public void Add(string msg)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			messages.Add(msg);
			scrollPos = new Vector2(scrollPos.x, 3.40282347E+38f);
		}

		public void Clear()
		{
			messages.Clear();
		}
	}
}
