using UnityEngine;


public class MoveAI_LR : MoveAI_Base
{
    private float _nextMoveTime;
    private bool _textMoveLeft;

    protected override Vector2 BornPos => Vector2Fight.New(0, 130);

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time > _nextMoveTime)
        {
            _textMoveLeft = !_textMoveLeft;
            _nextMoveTime = Time.time + 5f;
            if (_textMoveLeft)
            {
                Master.Move(Vector2Fight.New(-75f, 80f), 0.3f);
            }
            else
            {
                Master.Move(Vector2Fight.New(75f, 80f), 0.3f);
            }
        }
    }
}