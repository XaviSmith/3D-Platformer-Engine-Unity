using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PauseManager : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }
        public GameObject menu;
        public InputReader input;

        void OnEnable()
        {
            input.Pause += OnPause;
        }

        void OnPause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0 : 1;
            menu.SetActive(IsPaused);
        }
    }
}

