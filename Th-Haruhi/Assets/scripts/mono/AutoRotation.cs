
using UnityEngine;
using DG.Tweening;

public class AutoRotation : MonoBehaviour
{
    private float _curEuler;
    public float RotaFoward = -1;
    public float TurnSpeed = 300f;

    private void Update()
    {
        if(RotaFoward > 0)
        {
            _curEuler += Time.deltaTime * TurnSpeed;
            if (_curEuler > 360) _curEuler = 0;
        }
        else
        {
            _curEuler -= Time.deltaTime * TurnSpeed;
            if (_curEuler < 0) _curEuler = 360;
        }

        transform.eulerAngles = new Vector3(0, 0, _curEuler);
    }
}