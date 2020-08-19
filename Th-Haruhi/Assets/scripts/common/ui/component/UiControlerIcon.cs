
using InControl;
using UnityEngine;

public class UiControlerIcon : MonoBehaviour
{
    public EControllerBtns ButtonType;
    private UiImage _image;
    private UiText _text;
    void Awake()
    {
        GameEventCenter.AddListener(GameEvent.OnControllerDeviceChanged, OnDeviceChanged);
    }
    void OnDestroy()
    {
        GameEventCenter.RemoveListener(GameEvent.OnControllerDeviceChanged, OnDeviceChanged);
    }

    private void Start()
    {
        RefreshImage();
    }
    private void OnDeviceChanged(object argument)
    {
        RefreshImage();
    }

    private bool _activeState = true;
    public void SetActive(bool b)
    {
        _activeState = b;
        gameObject.SetActiveSafe(b);

    }
    public void RefreshImage()
    {
        if (ButtonType == EControllerBtns.None)
        {
            gameObject.SetActiveSafe(false);
            return;
        }
        if(_activeState)
            gameObject.SetActiveSafe(true);


        var style = InputDeviceStyle.Unknown;
        if(InputManager.Devices.Count > 0)
        {
            style = InputManager.Devices[InputManager.Devices.Count - 1].DeviceStyle;
        }

        _image = GetComponentInChildren<UiImage>();
        _text = GetComponentInChildren<UiText>();
        _text.text = "";
        switch (style)
        {
            case InputDeviceStyle.Unknown:
                //Pc显示方案
                var mouse = ControllerUtility.GetMouseType(ButtonType);
                if (mouse != Mouse.None)
                {
                    var mouseImg = ControllerUtility.GetImageMouse(mouse);
                    _image.SetSprite(EImagePath.Public, mouseImg);
                }
                else
                {
                    var key = ControllerUtility.GetKeyBoardType(ButtonType);
                    string text = "";
                    var img = ControllerUtility.GetImageKeyboard(key, out text);
                    _image.SetSprite(EImagePath.Public, img);
                    _text.text = text;
                }
                break;
            case InputDeviceStyle.Ouya:
            case InputDeviceStyle.AppleMFi:
            case InputDeviceStyle.AmazonFireTV:
            case InputDeviceStyle.NVIDIAShield:
            case InputDeviceStyle.Steam:
            case InputDeviceStyle.Xbox360:
            case InputDeviceStyle.XboxOne:
            case InputDeviceStyle.Vive:
            case InputDeviceStyle.Oculus:
                //xbox 显示方案
                var imgStr = ControllerUtility.GetImageXBox(ControllerUtility.GetControlType(ButtonType));
                _image.SetSprite(EImagePath.Public, imgStr);
                break;
            case InputDeviceStyle.PlayStation2:
            case InputDeviceStyle.PlayStation3:
            case InputDeviceStyle.PlayStation4:
            case InputDeviceStyle.PlayStationVita:
            case InputDeviceStyle.PlayStationMove:
                //ps 显示方案
                var psImageStr = ControllerUtility.GetImagePlayStation(ControllerUtility.GetControlType(ButtonType));
                _image.SetSprite(EImagePath.Public, psImageStr);
                break;
            case InputDeviceStyle.NintendoNES:
            case InputDeviceStyle.NintendoSNES:
            case InputDeviceStyle.Nintendo64:
            case InputDeviceStyle.NintendoGameCube:
            case InputDeviceStyle.NintendoWii:
            case InputDeviceStyle.NintendoWiiU:
            case InputDeviceStyle.NintendoSwitch:
                //switch 显示方案
                var nintendoImageStr = ControllerUtility.GetImageNintendo(ControllerUtility.GetControlType(ButtonType));
                _image.SetSprite(EImagePath.Public, nintendoImageStr);
                break;
        }
    }
}

