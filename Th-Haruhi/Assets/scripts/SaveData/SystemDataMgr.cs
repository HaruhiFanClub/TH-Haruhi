
using LitJson;
using UnityEngine;

public static class SystemDataMgr
{
    private static SystemData _data;
    public static SystemData Data
    {
        get
        {
            if(_data == null)
            {
                LoadSystemData();
            }
            return _data;
        }
    }

    public static void SaveSystemData()
    {
        LocalStorage.Write(SaveDataMgr.DataNameSystem, JsonMapper.ToJson(Data), LocalStorage.EStorageType.SaveData);
        Debug.Log("Save SystemData!");
    }

    public static void LoadSystemData()
    {
        var source = LocalStorage.Read<string>(SaveDataMgr.DataNameSystem, LocalStorage.EStorageType.SaveData);
        if (string.IsNullOrEmpty(source))
        {
            CreateSystemData();
            return;
        }
        _data = JsonMapper.ToObject<SystemData>(source);
    }

    public static void CreateSystemData()
    {
        _data = new SystemData
        {
            LatestFileNo = 1,
            ContiuneFileNo = 1,
            SettingData = new GameSettingData
            {
                MusicVolume = 1f,
                AudioVolume = 1f,
                DefaultQuality = SystemInfoUtils.GetDefaultQuailty(),
                Resolution = SystemInfoUtils.GetDefaultResolution(),
            }
        };
        SaveSystemData();
    }

    public static void Save(this GameSettingData settingData)
    {
        SaveSystemData(); 
    }

    public static void Save(this SystemData data)
    {
        SaveSystemData();
    }
}


