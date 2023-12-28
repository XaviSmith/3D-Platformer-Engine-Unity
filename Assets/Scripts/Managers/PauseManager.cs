using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PauseManager : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }
        public GameObject menu;
        public List<GameObject> containers;
        public InputReader input;

        [Header("Settings")]
        public float deadzone = 0.2f;

        int currIndex = 0;
        bool moving = false;

        void OnEnable()
        {
            input.Pause += OnPause;
            input.Navigate += SwitchContainer;
        }

        void OnPause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0 : 1;
            menu.SetActive(IsPaused);
        }

        void SwitchContainer(Vector2 direction)
        {
            if(IsPaused && !moving)
            {
                if (direction.x > deadzone)
                {
                    containers[currIndex].SetActive(false);
                    currIndex++;

                    if (currIndex >= containers.Count)
                    {
                        currIndex = 0;
                    }

                    containers[currIndex].SetActive(true);
                }
                else if (direction.x < -deadzone)
                {
                    containers[currIndex].SetActive(false);
                    currIndex--;

                    if (currIndex < 0)
                    {
                        currIndex = containers.Count - 1;
                    }

                    containers[currIndex].SetActive(true);
                }
            }
            
        }
    }
}

