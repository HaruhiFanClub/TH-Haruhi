
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UiRawImageUv : MonoBehaviour
{
    private RawImage target;
    public float SpeedX = 0f;
    public float SpeedY = 0f;

    private float ox;
    private float oy;
    private float wc;
    private float hc;

    private void Start()
    {
        if (target == null)
        {
            target = GetComponent<RawImage>();
        }

        wc = target.uvRect.width;
        hc = target.uvRect.height;
    }

    void Update()
    {
        if (target != null)
        {
            ox += Time.deltaTime * SpeedX;
            oy += Time.deltaTime * SpeedY;
            ox = ox % 1;
            oy = oy % 1;
            target.GetComponent<RawImage>().uvRect = new Rect(ox, oy, wc, hc);
        }
    }
}