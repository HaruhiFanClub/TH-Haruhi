

using LitJson;
using UnityEngine;

public static class PlayerDataMgr
{
    private static PlayerData _data;
    public static PlayerData Data
    {
        get
        {
            if (_data == null)
            {
                LoadPlayerData();
            }
            return _data;
        }
        set
        {
            _data = value;
            DoSaveData();
        }
    }

    private static bool _needSave;
    public static void SavePlayerData()
    {
        _needSave = true;
    }

    private static readonly RelayInterval SaveInterval = new RelayInterval(1f);
    public static void Update()
    {
        if (SaveInterval.NextTime())
        {
            if (_needSave)
            {
                _needSave = false;
                DoSaveData();
            }
        }
    }

    private static void DoSaveData()
    {
        _data.LastSaveTime = TimeUtility.GetCurrentSeconds() ;
        LocalStorage.Write(SaveDataMgr.DataNamePlayer, JsonMapper.ToJson(Data), LocalStorage.EStorageType.SaveData);
        Debug.Log("SavePlayerData!");
    }

    public static void LoadPlayerData()
    {
        var source = LocalStorage.Read<string>(SaveDataMgr.DataNamePlayer, LocalStorage.EStorageType.SaveData);
        if (string.IsNullOrEmpty(source))
        {
            CreatePlayerData();
            return;
        }
        _data = JsonMapper.ToObject<PlayerData>(source);
    }

    public static void CreatePlayerData()
    {
        _data = new PlayerData();
        _data.DeviceId = ApplicationUtility.GetDeviceId();
        _data.FileNo = SystemDataMgr.Data.LatestFileNo;
        _data.Nick = "Haruhi";

        SystemDataMgr.Data.LatestFileNo++;
        SystemDataMgr.Data.ContiuneFileNo = _data.FileNo;
        SavePlayerData();
    }
}