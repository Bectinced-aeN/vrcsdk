using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
	public class VRC_AvatarVariations : VRC_Behaviour
	{
		public enum VariationType
		{
			MeshEnable,
			ReplaceMaterial,
			MeshAndMaterial
		}

		[Serializable]
		public class Variation
		{
			public string name;

			public VariationType variationType;

			public MeshRenderer[] renderers;

			public Material replacementMaterial;
		}

		[Serializable]
		public class VariationCategory
		{
			public string name;

			public Variation[] variations;
		}

		public VariationCategory[] categories;

		private bool initialized;

		private string currentVariationSpec;

		private List<MeshRenderer> initialMeshes;

		private Material[] initialMaterials;

		private void Start()
		{
			RecordInitialState();
			initialized = true;
			RevertVariations();
			ApplyVariations();
		}

		private void RecordInitialState()
		{
			initialMeshes = new List<MeshRenderer>();
			VariationCategory[] array = categories;
			foreach (VariationCategory variationCategory in array)
			{
				Variation[] variations = variationCategory.variations;
				foreach (Variation variation in variations)
				{
					MeshRenderer[] renderers = variation.renderers;
					foreach (MeshRenderer item in renderers)
					{
						initialMeshes.Add(item);
					}
				}
			}
			initialMaterials = (Material[])new Material[initialMeshes.Count];
			for (int l = 0; l < initialMeshes.Count; l++)
			{
				initialMaterials[l] = initialMeshes[l].get_sharedMaterial();
			}
		}

		public void SetVariationSpec(string variationSpec)
		{
			currentVariationSpec = variationSpec;
			if (initialized)
			{
				RevertVariations();
				ApplyVariations();
			}
		}

		private void RevertVariations()
		{
			for (int i = 0; i < initialMeshes.Count; i++)
			{
				initialMeshes[i].set_sharedMaterial(initialMaterials[i]);
			}
			VariationCategory[] array = categories;
			foreach (VariationCategory variationCategory in array)
			{
				Variation[] variations = variationCategory.variations;
				foreach (Variation variation in variations)
				{
					if (variation.variationType == VariationType.MeshEnable || variation.variationType == VariationType.MeshAndMaterial)
					{
						MeshRenderer[] renderers = variation.renderers;
						foreach (MeshRenderer val in renderers)
						{
							val.set_enabled(false);
						}
					}
				}
			}
		}

		private void ApplyVariations()
		{
			string[] array = currentVariationSpec.Split('~');
			VariationCategory[] array2 = categories;
			foreach (VariationCategory variationCategory in array2)
			{
				string text = null;
				string[] array3 = array;
				foreach (string text2 in array3)
				{
					string[] array4 = text2.Split('_');
					if (array4.Length == 2 && array4[0] == variationCategory.name)
					{
						text = array4[1];
						break;
					}
				}
				bool flag = false;
				if (text != null)
				{
					Variation[] variations = variationCategory.variations;
					foreach (Variation variation in variations)
					{
						if (variation.name == text)
						{
							flag = true;
							ApplyVariation(variation);
						}
					}
				}
				if (!flag)
				{
					ApplyVariation(variationCategory.variations[0]);
				}
			}
		}

		private void ApplyVariation(Variation v)
		{
			if (v.variationType == VariationType.MeshEnable || v.variationType == VariationType.MeshAndMaterial)
			{
				MeshRenderer[] renderers = v.renderers;
				foreach (MeshRenderer val in renderers)
				{
					val.set_enabled(true);
				}
			}
			if (v.variationType == VariationType.ReplaceMaterial || v.variationType == VariationType.MeshAndMaterial)
			{
				MeshRenderer[] renderers2 = v.renderers;
				foreach (MeshRenderer val2 in renderers2)
				{
					val2.set_sharedMaterial(v.replacementMaterial);
				}
			}
		}
	}
}
