public abstract class IPool : Conditionable
{
    private bool inPool;

    public void SetInPool(bool b)
    {
        inPool = b;
    }

    public bool GetInPool()
    {
        return inPool;
    }
    public abstract void Init();
    public abstract void Reset();
    public abstract void Recycle();
    public abstract void OnDestroy();
}
