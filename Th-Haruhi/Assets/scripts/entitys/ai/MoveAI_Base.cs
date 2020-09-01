using UnityEngine;


public abstract class MoveAI_Base
{
    protected Enemy Master;

    protected abstract Vector2 BornPos { get; }

    public virtual void Init(Enemy enemy)
    {
        Master = enemy;
        enemy.transform.position = BornPos;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnDestroy()
    {

    }
}