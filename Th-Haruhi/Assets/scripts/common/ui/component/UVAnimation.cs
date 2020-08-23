
using UnityEngine;
using UnityEngine.UI;

public class UVAnimation : MonoBehaviour
{
    public float XSpeed;
    public float YSpeed;

    private Material _material;
    private void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    void Update()
    {
       if(_material != null)
       {
            var delteTime = Time.deltaTime;
            var t = _material.mainTextureOffset;
            t.x += delteTime * XSpeed;
            t.y += delteTime * YSpeed;
            _material.mainTextureOffset = t;
        }
    }
}