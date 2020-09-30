using UnityEngine;
using System.Collections;

public class StageCamera2D : MonoBehaviour
{
	public static StageCamera2D Instance;
	public Camera MainCamera { private set; get; }

	private void Awake()
	{
		MainCamera = GetComponent<Camera>();
		Instance = this;
	}

	private CameraPlayerDead _cameraDeadEffect;
	public void PlayDeadEffect(Vector3 wordPos)
	{
		_cameraDeadEffect = MainCamera.gameObject.GetComponent<CameraPlayerDead>();
		if(_cameraDeadEffect == null)
		{
			_cameraDeadEffect = MainCamera.gameObject.AddComponent<CameraPlayerDead>();
		}

		var screenPos = MainCamera.WorldToScreenPoint(wordPos);
		_cameraDeadEffect.Radius = 0f;
		_cameraDeadEffect.CenterX = screenPos.x / Screen.width;
		_cameraDeadEffect.CenterY = screenPos.y / Screen.height;
	}
	private void Update()
	{
		if(_cameraDeadEffect != null)
		{
			_cameraDeadEffect.Radius += Time.deltaTime * 0.85f;
			if(_cameraDeadEffect.Radius >= 1.5f)
			{
				Destroy(_cameraDeadEffect);
				_cameraDeadEffect = null;
			}
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}