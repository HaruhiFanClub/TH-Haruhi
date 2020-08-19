using UnityEngine;

public static class SaveDataMgr
{
    public const string DataNameSystem = "DATA_SYSATEM_01";
    public const string DataNamePlayer = "DATA_PLAYER_01";

    public static void ClearAllData()
    {
        PlayerDataMgr.CreatePlayerData();
        SystemDataMgr.CreateSystemData();
        Debug.LogError("ClearAllSaveData!!");
    }

    public static void PreloadGameData()
    {
        Debug.Log("Start Preload GameData:" + Time.realtimeSinceStartup);
        SystemDataMgr.LoadSystemData();
        PlayerDataMgr.LoadPlayerData();
        Debug.Log("Preload GameData Finished:" + Time.realtimeSinceStartup);
    }

    public static void Update()
    {
        PlayerDataMgr.Update();
    }
}
