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
	public void PlayDeadEffect(Vector3 worldPos)
	{
		_cameraDeadEffect = MainCamera.gameObject.GetComponent<CameraPlayerDead>();
		if(_cameraDeadEffect == null)
		{
			_cameraDeadEffect = MainCamera.gameObject.AddComponent<CameraPlayerDead>();
		}

		var screenPos = MainCamera.WorldToScreenPoint(worldPos);
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
		UpdateShake();
	}

	private bool _inShake;
	private float _shakeElapsed;
	private float _shakeDuration;
	private float _shakeMagnitude;
	private Vector3 _originPos;

	public void Shake(float duration, float magnitude)
	{
		_originPos = transform.position;
		_inShake = true;
		_shakeElapsed = 0f;
		_shakeDuration = duration;
		_shakeMagnitude = magnitude;
	}

	private void UpdateShake()
	{
		if (!_inShake) return;

		if(_shakeElapsed < _shakeDuration)
		{
			float x = _originPos.x + Random.Range(-1f, 1f) * _shakeMagnitude;
			float y = _originPos.y + Random.Range(-1f, 1f) * _shakeMagnitude;
			transform.position = new Vector3(x, y, _originPos.z);
			_shakeElapsed += Time.deltaTime;
		}
		else
		{
			if(_inShake)
			{
				_inShake = false;
				_shakeElapsed = 0;
				_shakeDuration = 0;
				_shakeMagnitude = 0;
				transform.position = _originPos;
			}
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}