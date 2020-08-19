using UnityEngine;

public class UITitleCharacter : MonoBehaviour
{
    private void Awake()
    {
        GameEventCenter.AddListener(GameEvent.OnSceneChange, OnSceneChange);
    }

    private void Start()
    {
        var characters = GetComponentsInChildren<Character>();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].MoveDir = EMoveDir.Down;
        }
    }
    private void OnDestroy()
    {
        GameEventCenter.RemoveListener(GameEvent.OnSceneChange, OnSceneChange);
    }
    private void OnSceneChange(object o)
    {
        Destroy(gameObject);
    }
}
