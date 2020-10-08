using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSceneBase : MonoBehaviour
{
    public float Speed = 70;
    private float _targetSpeed;
    private float _defaultSpeed;

    public void SpeedChange(float targetSpeed)
    {
        _targetSpeed = targetSpeed;
    }

    public void SpeedRevert()
    {
        _targetSpeed = _defaultSpeed;
    }

    protected virtual void Awake()
    {
        _targetSpeed = Speed;
        _defaultSpeed = Speed;
    }

    protected virtual void Update()
    {
        Speed = Mathf.Lerp(Speed, _targetSpeed, Time.deltaTime * 1f);
    }

    public static void ChangeSpeed(float speed)
    {
        var scene = FindObjectOfType<StageSceneBase>();
        if (scene != null)
        {
            scene.SpeedChange(speed);
        }
    }

    public static void RevertSpeed()
    {
        var scene = FindObjectOfType<StageSceneBase>();
        if (scene != null)
        {
            scene.SpeedRevert();
        }
    }
}
