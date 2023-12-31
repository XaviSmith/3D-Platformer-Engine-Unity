using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] Image fadeImg;
        [SerializeField] Color32 fadeInColor;
        [SerializeField] Color32 fadeOutColor;
        [SerializeField] float fadeInTime;
        [SerializeField] float fadeDelay;
        [SerializeField] float fadeOutTime;

        public bool FadeInFinished;
        public bool IsFinished;

        public float FadeInTime => fadeInTime; 


        public void FadeInAndOut()
        {
            FadeInFinished = false;
            IsFinished = false;
            StartCoroutine(FadeScreen());
        }

        IEnumerator FadeScreen()
        {
            float timer = 0f;

            while (timer < fadeInTime)
            {              
                timer += Time.deltaTime;
                fadeImg.color = Color32.Lerp(fadeInColor, fadeOutColor, timer / fadeInTime);
                yield return null;
            }

            FadeInFinished = true;

            yield return new WaitForSeconds(fadeDelay);

            timer = 0f;

            while (timer < fadeOutTime)
            {
                timer += Time.deltaTime;
                fadeImg.color = Color32.Lerp(fadeOutColor, fadeInColor, timer / fadeOutTime);
                yield return null;
            }
            IsFinished = true;
        }
    }
}
