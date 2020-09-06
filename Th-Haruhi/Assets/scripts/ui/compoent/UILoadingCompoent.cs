using CameraTransitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UILoadingCompoent : MonoBehaviour
{
    public UiImage Image;
    public List<CameraTransitionsAssistant> OpenEffects;
    public CanvasGroup CanvasGroup;
    public Camera CameraBg;
    public Camera CameraMain;
    public Camera CameraLoading;


    public IEnumerator Open()
    {
        gameObject.SetActiveSafe(true);
        CanvasGroup.alpha = 1f;
        float waitTime = 0;
        for (int i = 0; i < OpenEffects.Count; i++)
        {
            waitTime = OpenEffects[i].transitionTime;
            OpenEffects[i].ExecuteTransition();
        }
        yield return new WaitForSeconds(waitTime);
    }

    public IEnumerator Hide()
    {
        CameraLoading.gameObject.SetActiveSafe(false);
        CameraMain.gameObject.SetActiveSafe(true);
        CameraBg.gameObject.SetActiveSafe(true);
        CanvasGroup.DOFade(0f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActiveSafe(false);
    }
}
