using System;

public static class UiFrameTemplate
{
    static UiColorTemplate colorTemplate;

    public static UiColorTemplate GetColorTemplate()
    {
        if (colorTemplate == null)
            colorTemplate = UnityEngine.Object.FindObjectOfType<UiColorTemplate>();
        return colorTemplate;
    }
}
