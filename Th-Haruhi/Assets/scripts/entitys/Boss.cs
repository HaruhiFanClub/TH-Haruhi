

using UnityEngine;
/// <summary>
/// boss
/// 分阶段
/// 有圆圈血条
/// 有特殊UI展示
/// 有特殊立绘
/// </summary>
public class Boss : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        //debug
        Debug.LogError("Create boss");
    }
}

