using UnityEngine;


public static class CharacterMgr
{
    public static string Haruhi = "character/Haruhi.prefab";
    public static string Mikuru = "character/Mikuru.prefab";
    public static string Nagato = "character/Nagato.prefab";
    public static string Kyon   = "character/Kyon.prefab";
    public static string Koizumi = "character/Koizumi.prefab";

    private static Character _mainPlayer;

    public static void SwitchCharacter(string url)
    {
        Vector3 pos = Vector3.zero;
        if(_mainPlayer != null)
        {
            pos = _mainPlayer.transform.position;
            Clear();
        }
        _mainPlayer = CreateCharacter(url);
        _mainPlayer.transform.position = pos;
    }
    
    public static Character MainPlayer
    {
        get
        {
            if(_mainPlayer == null)
            {
                _mainPlayer = CreateCharacter(Haruhi);
            }
            return _mainPlayer;
        }
    }

    private static Character CreateCharacter(string chrUrl)
    {
        var obj = ResourceMgr.LoadImmediately(chrUrl);
        var chrObj = ResourceMgr.Instantiate(obj);
        chrObj.transform.SetParent(null);
        Object.DontDestroyOnLoad(chrObj);
        return chrObj.GetComponent<Character>();
    }

    public static void Clear()
    {
        EntityBase.DestroyEntity(_mainPlayer);
    }
}