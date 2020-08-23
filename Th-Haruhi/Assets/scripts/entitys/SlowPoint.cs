
using UnityEngine;
using DG.Tweening;

public class SlowPoint : MonoBehaviour
{
    public Transform Circle1;
    public Transform Circle2;

    private bool _inShow = true;
    private bool _inTween;
    private Tweener _t1;
    private Tweener _t2;
    private float _curEuler1;
    private float _curEuler2;
    private float _turnSpeed = 220f;

    public void SetVisible(bool b)
    {
        if (_inShow == b) return;
        _inShow = b;

        if(_inShow)
        {
            KillTween();
            gameObject.SetActiveSafe(true);
            _inTween = true;
            _t1 = transform.DOScale(1.2f, 0.1f);
            _t1.onComplete = () =>
            {
                _inTween = false;
                _t2 = transform.DOScale(1f, 0.1f);
            };
        }
        else
        {
            gameObject.SetActiveSafe(false);
        }
    }

    private void KillTween()
    {
        if (_t1 != null)
            _t1.Kill();
        _t1 = null;

        if (_t2 != null)
            _t2.Kill();
        _t2 = null;

        _inTween = false;
    }

    private void Update()
    {
        if (!_inShow || _inTween) return;

        _curEuler1 += Time.deltaTime * _turnSpeed;
        if (_curEuler1 > 360) _curEuler1 = 0;

        _curEuler2 -= Time.deltaTime * _turnSpeed;
        if (_curEuler2 < 0) _curEuler2 = 360;

        Circle1.eulerAngles = new Vector3(0, 0, _curEuler1);
        Circle2.eulerAngles = new Vector3(0, 0, _curEuler2);
    }

    private void OnDestroy()
    {
        KillTween();
    }
}