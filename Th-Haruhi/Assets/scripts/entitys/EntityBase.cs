using UnityEngine;

public class EntityBase : MonoBehaviour
{
    protected virtual void Awake() { }
    protected virtual void OnDestroy() { }
    protected virtual void Update() { }

    protected virtual void FixedUpdate() { }

    public static void DestroyEntity(EntityBase b)
    {
        if(b != null && b.gameObject != null)
        {
            Destroy(b.gameObject);
        }
    }
}