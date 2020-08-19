#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public enum ERegionType
{
    Born,
    ChangeScene,
    CameraSide
}
public class RegionBase : MonoBehaviour
{
    public ERegionType Type;
    public string SceneUrl;
    public string BornId;


    private void Awake()
    {
        
    }
}
