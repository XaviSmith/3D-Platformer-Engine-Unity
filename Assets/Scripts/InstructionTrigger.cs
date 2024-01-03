using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Platformer;

public class InstructionTrigger : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 1f;
    bool fadingIn;
    bool fadingOut;

    private void OnTriggerEnter(Collider other)
    {
        if(!GameManager.Instance.HideTexts)
        {
            FadeIn();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        FadeOut();
    }

    void FadeIn()
    {
        fadingOut = false;

        if(!fadingIn)
        {
            fadingIn = true;
            StartCoroutine(FadeInCoroutine());
        }
    }

    void FadeOut()
    {
        fadingIn = false;

        if(!fadingOut)
        {
            fadingOut = true;
            StartCoroutine(FadeOutCoroutine());
        }
    }

    IEnumerator FadeInCoroutine()
    {
        float timer = 0f;

        while(timer < fadeTime && fadingIn)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
        }

        fadingIn = false;
    }

    IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;

        while (timer < fadeTime && fadingOut)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeTime);
        }

        fadingOut = false;
    }

}
