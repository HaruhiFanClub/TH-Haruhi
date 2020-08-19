﻿

using InControl;
using UnityEngine;

public class ControllerPc : MonoBehaviour
{
    public static ControllerPc Instance;
    private ControllerActions _actions;

    void Awake()
    {
        Instance = this;
        _actions = ControllerActions.CreateWithDefaultBindings();
        InputManager.OnActiveDeviceChanged += OnDeviceChanged;
        InputManager.OnDeviceDetached += OnDeviceChanged;
    }

    void OnDestroy()
    {
        InputManager.OnActiveDeviceChanged -= OnDeviceChanged;
        InputManager.OnDeviceDetached -= OnDeviceChanged;
    }


    private static void OnDeviceChanged(InputDevice device)
    {
        Debug.Log("Controller OnDeviceChanged");
    }

    public static ControllerActions GetActions()
    {
        return Instance._actions;
    }


    private static bool _inShake;
    public static void StartShake(float intensity)
    {
        _inShake = true;
        var devicesCount = InputManager.Devices.Count;
        for (var i = 0; i < devicesCount; i++)
        {
            var inputDevice = InputManager.Devices[i];
            inputDevice.Vibrate(intensity);
        }
    }

    public static void StopShake()
    {
        if (!_inShake) return;
        _inShake = false;
        var devicesCount = InputManager.Devices.Count;
        for (var i = 0; i < devicesCount; i++)
        {
            var inputDevice = InputManager.Devices[i];
            inputDevice.StopVibration();
        }
    }

    
    protected void Update()
    {
        if(GameWorld.InGameing)
        {
            UpdateMove();
        }
        else
        {
            UpdateMenu();
        }
    }

    private void UpdateMenu()
    {
        if(_actions.Get(EControllerBtns.Up).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Up);
        }

        if (_actions.Get(EControllerBtns.Down).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Down);
        }

        if (_actions.Get(EControllerBtns.Left).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Left);
        }

        if (_actions.Get(EControllerBtns.Right).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Right);
        }

        if (_actions.Get(EControllerBtns.Sure).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Sure);
        }

        if (_actions.Get(EControllerBtns.Cancel).WasReleased)
        {
            GameEventCenter.Send(GameEvent.UI_Cancel);
        }
    }

    private void UpdateMove()
    {
        if(_actions.Move.IsPressed)
        {
            GameEventCenter.Send(GameEvent.StartMove, _actions.Move.Value);
        }
        else if (_actions.Move.WasReleased)
        {
            GameEventCenter.Send(GameEvent.StopMove);
        }
    }
}