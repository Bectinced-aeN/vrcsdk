using System.Collections;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_SlideShow : MonoBehaviour
	{
		public bool autoplay;

		public bool shuffle;

		public Texture2D[] images;

		public float displayDuration = -1f;

		public Material imageMaterial;

		private int showingImage;

		public VRC_SlideShow()
			: this()
		{
		}

		private void Start()
		{
			if (images.Length > 0)
			{
				imageMaterial.set_mainTexture(images[0]);
			}
			if (autoplay && displayDuration > 0f)
			{
				this.StartCoroutine("StartAutoplayWithDuration", (object)displayDuration);
			}
		}

		private void ShowNextImage(int Instigator = 0)
		{
			Texture2D val = null;
			if (images.Length > 0)
			{
				if (shuffle)
				{
					showingImage = Random.Range(0, images.Length - 1);
				}
				else
				{
					showingImage = ++showingImage % images.Length;
				}
				val = images[showingImage];
				if (val == null)
				{
					Debug.LogError((object)"Loaded image is null. Did you add the image to the array in the inspector?");
				}
				imageMaterial.set_mainTexture(val);
			}
			else
			{
				Debug.LogError((object)"Image array length is zero.");
			}
		}

		private void ShowPreviousImage(int Instigator = 0)
		{
			Texture2D val = null;
			showingImage = --showingImage % images.Length;
			if (showingImage < 0)
			{
				showingImage = images.Length + showingImage;
			}
			Debug.Log((object)("showing prev image: " + showingImage));
			val = images[showingImage];
			if (val == null)
			{
				Debug.LogError((object)"Loaded image is null. Did you add the image to the array in the inspector?");
			}
			imageMaterial.set_mainTexture(val);
		}

		private IEnumerator StartAutoplayWithDuration(float duration)
		{
			while (autoplay)
			{
				yield return (object)new WaitForSeconds(duration);
				ShowNextImage();
			}
		}

		private void StopAutoplayWithDuration(float duration)
		{
			this.StopCoroutine("StartAutoplayWithDuration");
		}
	}
}
