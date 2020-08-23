using UnityEngine;

public class EntityBase : MonoBehaviour
{
    private Rigidbody2D _rigidBody2d;
    public Rigidbody2D Rigid2D
    { 
        get 
        {
            if (_rigidBody2d == null)
            {
                var r = GetComponent<Rigidbody2D>();
                if(r != null)
                {
                    _rigidBody2d = r;
                }
                else
                {
                    _rigidBody2d = gameObject.AddComponent<Rigidbody2D>();
                    _rigidBody2d.bodyType = RigidbodyType2D.Dynamic;
                    _rigidBody2d.simulated = true;
                    _rigidBody2d.useAutoMass = false;
                    _rigidBody2d.mass = 10;
                    _rigidBody2d.drag = 30;
                    _rigidBody2d.freezeRotation = true;
                    _rigidBody2d.gravityScale = 0f;
                    _rigidBody2d.angularDrag = 0f;
                }
            }
            return _rigidBody2d;
        }
    }


    public bool InCache { private set; get; }
    public void SetInCache(bool b)
    {
        InCache = b;
    }
    protected virtual void Awake() {

        
    }
    protected virtual void OnDestroy() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    public virtual void OnRecycle() { }

    public static void DestroyEntity(EntityBase b)
    {
        if(b != null && b.gameObject != null)
        {
            Destroy(b.gameObject);
        }
    }
}