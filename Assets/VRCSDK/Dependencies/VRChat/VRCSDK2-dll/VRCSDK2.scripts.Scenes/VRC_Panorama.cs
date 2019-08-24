using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2.scripts.Scenes
{
	[RequireComponent(typeof(VRC_DataStorage))]
	internal class VRC_Panorama : MonoBehaviour
	{
		public enum Layout
		{
			Mono,
			TopBottom,
			LeftRight
		}

		[Serializable]
		public class PanoSpec
		{
			public string url;

			public Texture2D texture;
		}

		public Renderer renderer;

		public List<PanoSpec> panoramas;

		private int currentlyShown = -1;

		private VRC_DataStorage data;

		private int dataIndex = -1;

		public VRC_Panorama()
			: this()
		{
		}

		private void Start()
		{
			renderer = this.GetComponent<Renderer>();
			data = this.GetComponent<VRC_DataStorage>();
			for (int i = 0; i < data.data.Length; i++)
			{
				if (data.data[i].name == "display")
				{
					dataIndex = i;
				}
			}
		}

		private void Update()
		{
			int valueInt = data.data[dataIndex].valueInt;
			valueInt = ((valueInt >= 0) ? (valueInt % panoramas.Count) : (panoramas.Count - 1));
			if (currentlyShown != valueInt)
			{
				currentlyShown = valueInt;
				this.StartCoroutine(ShowPano(valueInt));
			}
		}

		public void NextPano()
		{
			data.data[dataIndex].valueInt++;
			if (data.data[dataIndex].valueInt >= panoramas.Count)
			{
				data.data[dataIndex].valueInt = 0;
			}
		}

		public void PrevPano()
		{
			data.data[dataIndex].valueInt--;
			if (data.data[dataIndex].valueInt < 0)
			{
				data.data[dataIndex].valueInt = panoramas.Count - 1;
			}
		}

		public void ShowPanoAt(int index)
		{
			index = ((index >= 0) ? ((index < panoramas.Count) ? index : (panoramas.Count - 1)) : 0);
			data.data[dataIndex].valueInt = index;
		}

		private IEnumerator ShowPano(int index)
		{
			if (panoramas[index].texture == null)
			{
				string url = panoramas[index].url;
				WWW www = new WWW(url);
				yield return (object)www;
				if (www.get_error() == null)
				{
					panoramas[index].texture = new Texture2D(www.get_texture().get_width(), www.get_texture().get_height());
					www.LoadImageIntoTexture(panoramas[index].texture);
				}
				www.Dispose();
			}
			if (currentlyShown == index)
			{
				renderer.get_material().set_mainTexture(panoramas[index].texture);
			}
		}
	}
}
