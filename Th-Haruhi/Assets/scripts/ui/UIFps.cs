﻿//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Globalization;

public class UIFps : UiInstance
{
    private UIFpsComponent _component;
    private float _fps;
    private int _frames;
    private float _lasttime;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _component = GetComponent<UIFpsComponent>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateFps();
    }

    private void UpdateFps()
    {
        if (GameSystem.PauseStatus)
        {
            return;
        }

        ++_frames;
        var currtime = Time.realtimeSinceStartup;
        if (!(currtime - _lasttime > 1f)) return;

        _fps = _frames / (currtime - _lasttime);
        _fps = Mathf.Ceil(_fps);
        _frames = 0;
        _lasttime = currtime;
        _component.Fps.text = _fps.ToString(CultureInfo.InvariantCulture) + "fps" + " 子弹数量:" + Bullet.TotalBulletCount;
    }
}
