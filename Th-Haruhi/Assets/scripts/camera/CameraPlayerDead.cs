using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraPlayerDead : MonoBehaviour
{
	[Header("Radius")]
	[Range(0f, 2f)]
	public float Radius = 0.2f;
	[Header("CenterX")]
	[Range(0f, 1f)]
	public float CenterX = 1.0f;
	[Header("CenterY")]
	[Range(0f, 1f)]
	public float CenterY = 1.0f;

	private Shader _shader;
	private Material _material;
	private float _screenRadio;

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

	private void Awake()
	{
		_screenRadio = (float)Screen.height / Screen.width;
	}

	void Start()
	{
		_shader = Shader.Find("Haruhi/PlayerDeadEffect");

		if (_shader != null && _shader.isSupported == false)
		{
			enabled = false;
		}
	}

	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (Material != null)
		{
			Material.SetFloat("_ScreenRatio", _screenRadio);
			Material.SetFloat("_Radius", Radius);
			Material.SetFloat("_CenterX", CenterX);
			Material.SetFloat("_CenterY", CenterY);

			Graphics.Blit(sourceTexture, destTexture, Material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	void Update()
	{
	}

	void OnDisable()
	{
		if (_material != null)
		{
			DestroyImmediate(_material);
		}
	}
}