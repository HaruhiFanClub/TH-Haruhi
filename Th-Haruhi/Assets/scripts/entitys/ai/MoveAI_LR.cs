using UnityEngine;


public class MoveAI_LR : MoveAI_Base
{
    private float _nextMoveTime;
    private bool _textMoveLeft;

    protected override Vector2 BornPos => Vector2Fight.NewWorld(0, 144f);

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time > _nextMoveTime)
        {
            _textMoveLeft = !_textMoveLeft;
            _nextMoveTime = Time.time + 5f;
           
        }
    }
}