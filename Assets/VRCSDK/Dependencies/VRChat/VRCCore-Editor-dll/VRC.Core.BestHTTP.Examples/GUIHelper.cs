using System;
using UnityEngine;

namespace VRC.Core.BestHTTP.Examples
{
	internal static class GUIHelper
	{
		private static GUIStyle centerAlignedLabel;

		private static GUIStyle rightAlignedLabel;

		public static Rect ClientArea;

		private static void Setup()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			if (centerAlignedLabel == null)
			{
				centerAlignedLabel = new GUIStyle(GUI.get_skin().get_label());
				centerAlignedLabel.set_alignment(4);
				rightAlignedLabel = new GUIStyle(GUI.get_skin().get_label());
				rightAlignedLabel.set_alignment(5);
			}
		}

		public static void DrawArea(Rect area, bool drawHeader, Action action)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Setup();
			GUI.Box(area, string.Empty);
			GUILayout.BeginArea(area);
			if (drawHeader)
			{
				DrawCenteredText(SampleSelector.SelectedSample.DisplayName);
				GUILayout.Space(5f);
			}
			action?.Invoke();
			GUILayout.EndArea();
		}

		public static void DrawCenteredText(string msg)
		{
			Setup();
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(msg, centerAlignedLabel, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public static void DrawRow(string key, string value)
		{
			Setup();
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Label(key, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value, rightAlignedLabel, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
