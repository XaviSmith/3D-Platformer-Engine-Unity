using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    /// <summary>
    /// Pair with a Generic Event Listener.
    /// </summary>
    public class UpdateText : MonoBehaviour
    {
        [SerializeField] Text textbox;
        [SerializeField] TMPro.TextMeshProUGUI textMeshPro;

        public void UpdateTextBox(string ok)
        {

        }

        public void UpdateTextBox(int val)
        {
            textMeshPro.text = val.ToString();
        }
      
    }
}
