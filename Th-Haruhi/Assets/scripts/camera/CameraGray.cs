using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraGray : MonoBehaviour
{
	
	public float GrayScaleAmount = 1.0f;

	private Shader _shader;
	private Material _material;

	public Material Material
	{
		get
		{
			if (_material == null)
			{
				
				_material = new Material(_shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}
			return _material;
		}
	}

	// Use this for initialization
	void Start()
	{
		_shader = Shader.Find("Haruhi/CameraGray");
		if (_shader != null && _shader.isSupported == false)
		{
			enabled = false;
		}
	}

	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (_shader != null)
		{
			Material.SetFloat("_LuminosityAmount", GrayScaleAmount);

			Graphics.Blit(sourceTexture, destTexture, Material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	void Update()
	{
		GrayScaleAmount = Mathf.Clamp(GrayScaleAmount, 0.0f, 1.0f);
	}

	void OnDisable()
	{
		if (_material != null)
		{
			DestroyImmediate(_material);
		}
	}
}