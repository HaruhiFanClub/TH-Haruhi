

public static class GamePause
{
    private static float timeScale;
    public static bool pause;

    public static void Init()
    {
        timeScale = TimeScaleManager.GetTimeScale();
    }

    public static void PauseGame()
    {
        if (!pause)
        {
            timeScale = TimeScaleManager.GetTimeScale();
            TimeScaleManager.SetTimeScale(0f);
            pause = true;
        }
    }

    public static void DoContionueGame()
    {
        TimeScaleManager.SetTimeScale(timeScale);
        pause = false;
        
    }
}
