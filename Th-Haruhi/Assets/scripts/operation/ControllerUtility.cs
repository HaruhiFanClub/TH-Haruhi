
using InControl;
using UnityEngine;

public class ControllerUtility
{
    public static Mouse GetMouseType(EControllerBtns e)
    {
        var actions = ControllerPc.GetActions();
        var btn = actions.Get(e);
        for (int i = 0; i < btn.Bindings.Count; i++)
        {
            var a = btn.Bindings[i];
            if (a.BindingSourceType == BindingSourceType.MouseBindingSource)
            {
                return ((MouseBindingSource)a).Control;
            }
        }
        return Mouse.None;
    }

    public static string GetKeyBoardType(EControllerBtns e)
    {
        var actions = ControllerPc.GetActions();
        var btn = actions.Get(e);
        for (int i = 0; i < btn.Bindings.Count; i++)
        {
            var a = btn.Bindings[i];
            if (a.BindingSourceType == BindingSourceType.KeyBindingSource)
            {
                var text = ((KeyBindingSource) a).Control.ToString();
                text = text.ToUpper();
                if (text.Contains("NUM "))
                    text = text.Replace("NUM ", "");
                if (text.Contains("APE"))
                    text = text.Replace("APE", "");
                return text;
            }
        }
        return "";
    }

    public static InputControlType GetControlType(EControllerBtns e)
    {
        var actions = ControllerPc.GetActions();
        var btn = actions.Get(e);
        for (int i = 0; i < btn.Bindings.Count; i++)
        {
            var a = btn.Bindings[i];
            if (a.BindingSourceType == BindingSourceType.DeviceBindingSource)
            {
                return ((DeviceBindingSource)a).Control;
            }
        }
        Debug.LogError("GetControlType Is None:"+e);
        return InputControlType.None;
    }

    public static string GetImageKeyboard(string key, out string text)
    {
        text = "";
        if (key == "Backspace") return "button_keyboard_BACK";
        if (key == "CapsLock") return "button_keyboard_caps";
        if (key == "Space") return "button_keyboard_SPACE";
        if (key.Length > 1)
        {
            text = key;
            return "button_keyboard_long";
        }

        text = key;
        return "button_keyboard_uni";
    }
    public static string GetImageMouse(Mouse mouse)
    {
        switch (mouse)
        {
            case Mouse.LeftButton:
                return "button_keyboard_lmouse";                
            case Mouse.RightButton:
                return "button_keyboard_rmouse";
            case Mouse.MiddleButton:
                return "button_keyboard_mmouse";
                break;
        }
        Debug.LogError("CantFind Mouse Button Image, type:" + mouse);
        return null;
    }

    public static string GetImagePlayStation(InputControlType type)
    {
        switch (type)
        {
            case InputControlType.Command:
            case InputControlType.Options:
                return "button_ps4_options";
            case InputControlType.Share:
                return "button_ps4_share";
            case InputControlType.TouchPadButton:
                return "button_ps4_touchpad";
            case InputControlType.Action1:
                return "button_ps4_x";
            case InputControlType.Action2:
                return "button_ps4_circle";
            case InputControlType.Action3:
                return "button_ps4_square";
            case InputControlType.Action4:
                return "button_ps4_triangle";
            case InputControlType.LeftTrigger:
                return "button_ps4_l2";
            case InputControlType.LeftBumper:
                return "button_ps4_l1";
            case InputControlType.RightTrigger:
                return "button_ps4_r2";
            case InputControlType.RightBumper:
                return "button_ps4_r1";
            case InputControlType.LeftStickButton:
                return "button_common_l";
            case InputControlType.RightStickButton:
                return "button_common_r";
            case InputControlType.DPadUp:
                return "button_common_up";
            case InputControlType.DPadDown:
                return "button_common_down";
            case InputControlType.DPadLeft:
                return "button_common_left";
            case InputControlType.DPadRight:
                return "button_common_right";

        }
        Debug.LogError("CantFind PlayStation Button Image, type:" + type);
        return null;
    }

    public static string GetImageXBox(InputControlType type)
    {
        switch (type)
        {
            case InputControlType.Command:
            case InputControlType.Start:
                return "button_xbox_menu";
            case InputControlType.Back:
                return "button_xbox_options";
            case InputControlType.Action1:
                return "button_xbox_a";
            case InputControlType.Action2:
                return "button_xbox_b";
            case InputControlType.Action3:
                return "button_xbox_x";
            case InputControlType.Action4:
                return "button_xbox_y";
            case InputControlType.LeftTrigger:
                return "button_xbox_lt";
            case InputControlType.LeftBumper:
                return "button_xbox_lb";
            case InputControlType.RightTrigger:
                return "button_xbox_rt";
            case InputControlType.RightBumper:
                return "button_xbox_rb";
            case InputControlType.LeftStickButton:
                return "button_common_l";
            case InputControlType.RightStickButton:
                return "button_common_r";
            case InputControlType.DPadUp:
                return "button_common_up";
            case InputControlType.DPadDown:
                return "button_common_down";
            case InputControlType.DPadLeft:
                return "button_common_left";
            case InputControlType.DPadRight:
                return "button_common_right";

        }
        Debug.LogError("CantFind XBox Button Image, type:" + type);
        return null;
    }

    public static string GetImageNintendo(InputControlType type)
    {
        switch (type)
        {
            case InputControlType.Command:
            case InputControlType.Start:
                return "button_switch_plus";
            case InputControlType.Back:
                return "button_switch_minus";
            case InputControlType.Action1:
                return "button_switch_b";
            case InputControlType.Action2:
                return "button_switch_a";
            case InputControlType.Action3:
                return "button_switch_y";
            case InputControlType.Action4:
                return "button_switch_x";
            case InputControlType.LeftTrigger:
                return "button_switch_l";
            case InputControlType.LeftBumper:
                return "button_switch_zl";
            case InputControlType.RightTrigger:
                return "button_switch_r";
            case InputControlType.RightBumper:
                return "button_switch_zr";
            case InputControlType.LeftStickButton:
                return "button_common_l";
            case InputControlType.RightStickButton:
                return "button_common_r";
            case InputControlType.DPadUp:
                return "button_common_up";
            case InputControlType.DPadDown:
                return "button_common_down";
            case InputControlType.DPadLeft:
                return "button_common_left";
            case InputControlType.DPadRight:
                return "button_common_right";

        }
        Debug.LogError("CantFind Switch Button Image, type:" + type);
        return null;
    }
}
