using UnityEngine;
using UnityEngine.UI;


public class UiImage : Image
{
    private bool _useRaycast;
    private RectTransform _rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (Alpha <= 0)
        {
            type = Type.Simple;
        }
        _useRaycast = raycastTarget;
    }

    public void DisableRayCast()
    {
        _useRaycast = false;
        raycastTarget = false;
        GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
    }
    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        if (!Application.isPlaying) return;
        
        if (!_useRaycast)
            GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
    }


    protected override void OnCanvasHierarchyChanged()
    {
        base.OnCanvasHierarchyChanged();
        if (!Application.isPlaying) return;
       
        if (!_useRaycast)
            GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!Application.isPlaying) return;
       
        if (!_useRaycast)
            GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
    }

    private bool _unregistered = false;
    protected override void OnCanvasGroupChanged()
    {
        base.OnCanvasGroupChanged();
        if (!Application.isPlaying) return;
        if (_useRaycast)
        {
            var block = GetComponentInParent<CanvasGroup>().blocksRaycasts;
            raycastTarget = block;
            if (!block)
            {
                _unregistered = true;
                GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
            }
            else
            {
                if(_unregistered)
                    GraphicRegistry.RegisterGraphicForCanvas(canvas, this);
            }
        }
    }

    public Sprite Sprite
    {
        get { return sprite; }
        set
        {
            sprite = value;
            Alpha = 1f;
        }
    }
    public float Alpha
    {
        get { return color.a; }
        set
        {
            Color c = color;
            c.a = value;
            color = c;
        }
    }
    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = this.CreateCanvasGroup();
            }
            return _canvasGroup;
        }
    }
    public void SetActiveByCanvasGroup(bool b)
    {
        CanvasGroup.SetActiveByCanvasGroup(b);
    }

    public float Width
    {
        get { return rectTransform.sizeDelta.x; }
        set
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x = value;
            rectTransform.sizeDelta = sizeDelta;
        }
    }

    public float Height
    {
        get { return rectTransform.sizeDelta.y; }
        set
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = value;
            rectTransform.sizeDelta = sizeDelta;
        }
    }

    private bool _isGray = false;
    private Material _defaultMaterial;
    public void SetGray(bool b)
    {
        if (_isGray == b) return;
        _isGray = b;
        if (b)
        {
            _defaultMaterial = material;
            material = GameSystem.DefaultRes.UiGray;
        }
        else
        {
            material = _defaultMaterial;
        }
    }
    protected override void OnDestroy()
    {
        _rectTransform = null;
        base.OnDestroy();
    }

#if UNITY_EDITOR
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        if (raycastTarget)
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.GetWorldCorners(fourCorners);
            Gizmos.color = Color.green;
            for (int i = 0; i < 4; i++)
                Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
        }
    }
#endif
}
