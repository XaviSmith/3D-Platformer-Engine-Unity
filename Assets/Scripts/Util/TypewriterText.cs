using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmPro;
    [SerializeField] float textInterval = 0.02f;
    string textToType = null;
    bool adding;

    private void OnEnable()
    {
        if(tmPro == null)
        {
            try
            {
                tmPro = GetComponent<TextMeshProUGUI>();
            } catch
            {
                tmPro = null;
            }
            
        }
    }

    public void SetText(string _text)
    {
        textToType = _text;
        adding = false;
        StartCoroutine(TextVisible());
    }


    public void AddText(string _text)
    {
        textToType += _text;
        adding = true;
        StartCoroutine(TextVisible());
    }

    IEnumerator TextVisible()
    {
        //int totalVisibleCharacters = tmPro.textInfo.characterCount;
        int counter = adding ? tmPro.textInfo.characterCount : 0;
        tmPro.maxVisibleCharacters = counter;

        tmPro.text = textToType;

        while(counter <= textToType.Length)
        {
            counter++;
            tmPro.maxVisibleCharacters = counter;
            yield return new WaitForSeconds(textInterval);
        }

        adding = false;
    }
}
