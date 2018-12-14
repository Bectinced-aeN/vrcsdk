using System;
using System.IO;
using UnityEngine;

public class CameraImageCapture : MonoBehaviour
{
	public int resWidth = 1200;

	public int resHeight = 900;

	public Camera shotCamera;

	public CameraImageCapture()
		: this()
	{
	}

	private string ImageName(int width, int height, string name, string savePath)
	{
		return string.Format("{0}/{1}_{2}x{3}_{4}.png", savePath, name, width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public string TakePicture(string name = "image", string savePath = "")
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture targetTexture = shotCamera.get_targetTexture();
		float fieldOfView = shotCamera.get_fieldOfView();
		float aspect = shotCamera.get_aspect();
		RenderTexture val = new RenderTexture(resWidth, resHeight, 24);
		shotCamera.set_targetTexture(val);
		shotCamera.set_aspect((float)resWidth / (float)resHeight);
		shotCamera.set_fieldOfView(75f);
		Texture2D val2 = new Texture2D(resWidth, resHeight, 3, false);
		shotCamera.Render();
		RenderTexture.set_active(val);
		val2.ReadPixels(new Rect(0f, 0f, (float)resWidth, (float)resHeight), 0, 0);
		shotCamera.set_targetTexture(targetTexture);
		RenderTexture.set_active(null);
		shotCamera.set_fieldOfView(fieldOfView);
		shotCamera.set_aspect(aspect);
		Object.Destroy(val);
		byte[] bytes = val2.EncodeToPNG();
		if (string.IsNullOrEmpty(savePath))
		{
			savePath = Application.get_temporaryCachePath();
		}
		string text = ImageName(resWidth, resHeight, name, savePath);
		File.WriteAllBytes(text, bytes);
		Debug.Log((object)$"Took screenshot to: {text}");
		return text;
	}
}
