//#define REBUILD_TEST
using System;
using UnityEngine.UI;
using UnityEngine;

public class UiText : Text
{
    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }
    protected override void Start()
    {
        base.Start();
    }

    private void Refresh(object o = null)
    {
        SetAllDirty();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public string GetBaseText()
    {
        return base.text;
    }
    
    public override float preferredHeight
    {
        get
        {
            return cachedTextGeneratorForLayout.GetPreferredHeight(text, GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f))) / pixelsPerUnit;
        }
    }

    public override float preferredWidth 
    {
        get
        {
            return cachedTextGeneratorForLayout.GetPreferredWidth(text, GetGenerationSettings(Vector2.zero)) / pixelsPerUnit;
        }
    }

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
    
    public override string text
    {
        get
        {
            return base.text;
        }
        set
        {
            if (value != base.text)
            {
                base.text = value;
                if (underLine)
                    SyncText();
            }
        }
    }

    public new int fontSize
    {
        get
        {
            return base.fontSize;
        }
        set
        {
            base.fontSize = value;
            if (underLine)
                SyncText();
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
    public new Color color
    {
        get
        {
            return base.color;
        }
        set
        {
            base.color = value;
            if (underLine)
                SyncText();
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

    public bool underLine
    {
        get { return m_underLine; }
        set
        {
            bool sync = m_underLine && !value;

            if (!m_underLine && value)
                sync = true;

            m_underLine = value;
            if (sync)
                SyncText();
        }
    }
    [SerializeField]
    protected bool m_underLine;
    protected Text m_textUnderLine;

    public void SyncText()
    {
        if (m_underLine)
        {
            Transform textT = transform.Find("underline");
            if (textT == null)
            {
                GameObject textGo = new GameObject();
                textGo.name = "underline";
                textGo.transform.SetParent(gameObject.transform);
                textGo.transform.SetAsLastSibling();
                textGo.layer = gameObject.layer;
                m_textUnderLine = textGo.AddComponent<UiText>();
                m_textUnderLine.fontSize = fontSize;
                m_textUnderLine.material = material;
                m_textUnderLine.alignment = alignment;
                RectTransform rt = m_textUnderLine.rectTransform;
                //设置下划线坐标和位置  
                rt.anchoredPosition3D = Vector3.zero;
                rt.offsetMax = Vector2.zero;
                rt.offsetMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchorMin = Vector2.zero;

                m_textUnderLine.color = color;
                m_textUnderLine.text = "_";
                float perlineWidth = m_textUnderLine.preferredWidth;      //单个下划线宽度  
                float width = preferredWidth;
                int lineCount = (int)Mathf.Round(width / perlineWidth);
                for (int i = 1; i < lineCount; i++)
                {
                    m_textUnderLine.text += "_";
                }

                m_textUnderLine.raycastTarget = false;
            }
            else
            {
                m_textUnderLine = textT.GetComponent<UiText>();
                if (m_textUnderLine != null)
                {
                    m_textUnderLine.fontSize = fontSize;
                    m_textUnderLine.material = material;
                    m_textUnderLine.alignment = alignment;
                    RectTransform rt = m_textUnderLine.rectTransform;
                    //设置下划线坐标和位置  
                    rt.anchoredPosition3D = Vector3.zero;
                    rt.offsetMax = Vector2.zero;
                    rt.offsetMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.anchorMin = Vector2.zero;
                    m_textUnderLine.color = color;
                    m_textUnderLine.text = "_";
                    float perlineWidth = m_textUnderLine.preferredWidth;      //单个下划线宽度  
                    float width = preferredWidth;
                    int lineCount = (int)Mathf.Round(width / perlineWidth);
                    for (int i = 1; i < lineCount; i++)
                    {
                        m_textUnderLine.text += "_";
                    }
                    m_textUnderLine.raycastTarget = false;
                }
            }
        }
        else
        {
            m_textUnderLine = null;
            Transform textT = transform.Find("underline");
            if (textT != null)
            {
                DestroyImmediate(textT.gameObject, true);
            }
        }
    }

#if REBUILD_TEST
    public void OnDirtyLayout()
    {
        Debug.Log(string.Format("{0} ___ OnDirtyLayout", gameObject.name));
    }

    private void OnDirtyMaterial()
    {
        Debug.Log(string.Format("{0} ___ OnDirtyMaterial", gameObject.name));
    }

    private void OnDirtyVertices()
    {
        Debug.Log(string.Format("{0} ___ OnDirtyVertices", gameObject.name));
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        Debug.Log(string.Format("{0} ___ OnPopulateMesh", gameObject.name));
    }
#endif

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

    public void AddOutLine()
    {
        var shadow = GetComponent<Shadow>();
        if (shadow == null)
            shadow = gameObject.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
        shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        shadow.useGraphicAlpha = true;

        var outLine = GetComponent<Outline>();
        if (outLine == null)
            outLine = gameObject.AddComponent<Outline>();
        outLine.effectDistance = new Vector2(1f, -1f);
        outLine.effectColor = new Color(0f, 0f, 0f, 0.7f);
        outLine.useGraphicAlpha = true;
    }

    public void RemoveOutLine()
    {
        var outLine = GetComponent<Outline>();
        if (outLine != null)
        {
            DestroyImmediate(outLine);
        }

        var shadow = GetComponent<Shadow>();
        if (shadow != null)
        {
            DestroyImmediate(shadow);
        }

    }
#endif
}
