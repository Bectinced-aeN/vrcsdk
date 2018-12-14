using System;
using UnityEngine;
using VRC.Core.BestHTTP.Caching;

namespace VRC.Core.BestHTTP.Examples
{
	internal sealed class CacheMaintenanceSample : MonoBehaviour
	{
		private enum DeleteOlderTypes
		{
			Days,
			Hours,
			Mins,
			Secs
		}

		private DeleteOlderTypes deleteOlderType = DeleteOlderTypes.Secs;

		private int value = 10;

		private int maxCacheSize = 5242880;

		public CacheMaintenanceSample()
			: this()
		{
		}

		private void OnGUI()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Delete cached entities older then", (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label(value.ToString(), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.MinWidth(50f)
				});
				value = (int)GUILayout.HorizontalSlider((float)value, 1f, 60f, (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.MinWidth(100f)
				});
				GUILayout.Space(10f);
				deleteOlderType = (DeleteOlderTypes)GUILayout.SelectionGrid((int)deleteOlderType, new string[4]
				{
					"Days",
					"Hours",
					"Mins",
					"Secs"
				}, 4, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Max Cache Size (bytes): ", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.Width(150f)
				});
				GUILayout.Label(maxCacheSize.ToString("N0"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.Width(70f)
				});
				maxCacheSize = (int)GUILayout.HorizontalSlider((float)maxCacheSize, 1024f, 1.048576E+07f, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (GUILayout.Button("Maintenance", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					TimeSpan deleteOlder = TimeSpan.FromDays(14.0);
					switch (deleteOlderType)
					{
					case DeleteOlderTypes.Days:
						deleteOlder = TimeSpan.FromDays((double)value);
						break;
					case DeleteOlderTypes.Hours:
						deleteOlder = TimeSpan.FromHours((double)value);
						break;
					case DeleteOlderTypes.Mins:
						deleteOlder = TimeSpan.FromMinutes((double)value);
						break;
					case DeleteOlderTypes.Secs:
						deleteOlder = TimeSpan.FromSeconds((double)value);
						break;
					}
					HTTPCacheService.BeginMaintainence(new HTTPCacheMaintananceParams(deleteOlder, (ulong)maxCacheSize));
				}
			});
		}
	}
}
