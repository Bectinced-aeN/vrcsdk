using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2.scripts.Scenes
{
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

			public Layout layout;

			public Texture2D texture;
		}

		public Renderer rendererLeft;

		public Renderer rendererRight;

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

		private void NextPano()
		{
			data.data[dataIndex].valueInt++;
			if (data.data[dataIndex].valueInt >= panoramas.Count)
			{
				data.data[dataIndex].valueInt = 0;
			}
		}

		private void PrevPano()
		{
			data.data[dataIndex].valueInt--;
			if (data.data[dataIndex].valueInt < 0)
			{
				data.data[dataIndex].valueInt = panoramas.Count - 1;
			}
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
					panoramas[index].texture = www.get_texture();
				}
			}
			if (currentlyShown == index)
			{
				rendererLeft.get_material().set_mainTexture(panoramas[index].texture);
				rendererRight.get_material().set_mainTexture(panoramas[index].texture);
				switch (panoramas[index].layout)
				{
				case Layout.Mono:
					rendererLeft.get_material().set_mainTextureOffset(new Vector2(0f, 0f));
					rendererLeft.get_material().set_mainTextureScale(new Vector2(1f, 1f));
					rendererRight.get_material().set_mainTextureOffset(new Vector2(0f, 0f));
					rendererRight.get_material().set_mainTextureScale(new Vector2(1f, 1f));
					break;
				case Layout.TopBottom:
					rendererLeft.get_material().set_mainTextureOffset(new Vector2(0f, 0.5f));
					rendererLeft.get_material().set_mainTextureScale(new Vector2(1f, 0.5f));
					rendererRight.get_material().set_mainTextureOffset(new Vector2(0f, 0f));
					rendererRight.get_material().set_mainTextureScale(new Vector2(1f, 0.5f));
					break;
				case Layout.LeftRight:
					rendererLeft.get_material().set_mainTextureOffset(new Vector2(0f, 0f));
					rendererLeft.get_material().set_mainTextureScale(new Vector2(0.5f, 1f));
					rendererRight.get_material().set_mainTextureOffset(new Vector2(0.5f, 0f));
					rendererRight.get_material().set_mainTextureScale(new Vector2(0.5f, 1f));
					break;
				}
			}
		}
	}
}
