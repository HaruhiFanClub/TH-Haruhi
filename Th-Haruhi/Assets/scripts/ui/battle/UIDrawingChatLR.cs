
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIDrawingChatLR : MonoBehaviour
{
    public RectTransform Transform;
    public RawImage Draw;
    public UiImage Dialog;
    public UiText Text;
    public bool IsLeft;

    public DialogDeploy Deploy;

    private Vector3 _dialogOriginScale;
    private Vector2 _dialogOriginSize;
    private Vector2 _textOriginSize;

    private Vector2 StartPos
    {
        get
        {
            return new Vector2(IsLeft ? -600 : 600, -300);
        }
    }

    private void Awake()
    {
        _dialogOriginScale = Dialog.transform.localScale;
        _dialogOriginSize = Dialog.rectTransform.sizeDelta;
        _textOriginSize = Text.rectTransform.sizeDelta;
        gameObject.SetActiveSafe(false);
    }

    public void CloseFadeOut(float aniTime)
    {
        //先淡出对话框
        Dialog.DOFade(0f, 0.2f);
        Text.DOFade(0f, 0.2f);
        Draw.rectTransform.DOAnchorPos(StartPos, aniTime);
        Draw.DOFade(0f, aniTime);
    }

    public void FadeOut()
    {
        var aniTime = 0.5f;
        Dialog.DOFade(0f, 0.2f);
        Text.DOFade(0f, 0.2f);
        Draw.DOColor(new Color(0.4f, 0.4f, 0.4f, 1f), aniTime);
        var pos = IsLeft ? new Vector2(-50, -30) : new Vector2(50, -30);
        Draw.rectTransform.DOAnchorPos(pos, aniTime);
    }

    public void Show(DialogDeploy info, bool isFirst)
    {
        Deploy = info;
        gameObject.SetActiveSafe(true);
        StartCoroutine(DoShow(info, isFirst));
    }

    private IEnumerator DoShow(DialogDeploy info, bool isFirst)
    {
        //设置立绘信息
        SetDrawImage(Draw, info);

        //设置对话框信息
        Dialog.Alpha = 1f;
        Dialog.transform.localScale = new Vector3(0, _dialogOriginScale.y, _dialogOriginScale.z);
        Dialog.rectTransform.sizeDelta = _dialogOriginSize;
        Text.rectTransform.sizeDelta = _textOriginSize;
        Text.Alpha = 1f;
        SetDialogText(Text, Dialog, info.text);

        if (isFirst)
        {
            //首次显示动画
            Draw.rectTransform.anchoredPosition = StartPos;
            Draw.color = new Color(1f, 1f, 1f, 0f);
            Draw.rectTransform.DOAnchorPos(Vector2.zero, 0.3f);
            Draw.DOFade(1f, 1f);

            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            Draw.DOColor(Color.white, 0.3f);
            Draw.rectTransform.DOAnchorPos(Vector2.zero, 0.3f);

            yield return new WaitForSeconds(0.15f);
        }

        Sound.PlayUiAudioOneShot(1006);
        Dialog.transform.DOScaleX(_dialogOriginScale.x, 0.2f);
    }


    //自动设置对话框文字、长度、背景宽度
    private void SetDialogText(UiText text, UiImage dialogBg, string str)
    {
        var dialogAdd = dialogBg.rectTransform.sizeDelta.x - text.rectTransform.sizeDelta.x;

        //得到单行文字宽度
        Font font = text.font;
        int fontsize = text.fontSize;
        font.RequestCharactersInTexture(str, fontsize, FontStyle.Normal);
        float width = 0f;
        for (int i = 0; i < str.Length; i++)
        {
            font.GetCharacterInfo(str[i], out CharacterInfo characterInfo, fontsize);
            width += characterInfo.advance;
        }

        //默认是显示2行的
        width = width / 2;

        var txtSize = text.rectTransform.sizeDelta;
        if (txtSize.x < width)
        {
            txtSize.x = width;
            text.rectTransform.sizeDelta = txtSize;

            var dialogSize = dialogBg.rectTransform.sizeDelta;
            dialogSize.x = width + dialogAdd;
            dialogBg.rectTransform.sizeDelta = dialogSize;
        }
        text.text = str;
    }

    //设置立绘信息
    private void SetDrawImage(RawImage image, DialogDeploy info)
    {
        //读取立绘
        image.SetRawImageTexture(info.drawing);
        image.SetNativeSize();

        var h = 584;
        var size = image.rectTransform.sizeDelta;
        var xRatio = h / size.y;
        image.rectTransform.sizeDelta = new Vector2(size.x * xRatio, h);


        var euler = Draw.transform.eulerAngles;
        euler.y = info.revertImage ? 180 : 0;
        Draw.transform.eulerAngles = euler;
    }

}