using UnityEngine;

public class MoveData
{
    public static MoveData New(Vector3 startPos, Vector3? forward = null, float speed = 0)
    {
        var data = new MoveData();
        if(forward != null)
        {
            data.Forward = (Vector3)forward;
        }
       
        data.StartPos = startPos;
        data.SpeedX = speed;
        data.SpeedY = speed;
        return data;
    }

    public enum EHelixToward
    { 
        None = 0,
        Right = 1,
        Left = -1
    }

    public float Speed
    {
        set
        {
            SpeedX = value;
            SpeedY = value;
        }
    }

    public float SpeedX;
    public float SpeedY;
    public Vector3 StartPos;
    public Vector3 Forward;
}
