using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Platformer
{
    public class GameTimer : MonoBehaviour
    {
        StopwatchTimer gameTimer;
        bool showTimer;
        [SerializeField] TMPro.TextMeshProUGUI tmPro;

        public string FormattedTime => gameTimer.GetFormattedTime();
        public float CurrTime => gameTimer.GetTime();

        void Awake()
        {
            gameTimer = new StopwatchTimer(0);
        }

        void OnEnable()
        {
            if(GameManager.Instance.gameTimer == null)
            {
                GameManager.Instance.gameTimer = this;
            }

            gameTimer.Start();
        }

        // Update is called once per frame
        void Update()
        {
            gameTimer.Tick(Time.deltaTime);
            tmPro.text = FormattedTime;

        }

        public void ShowTimer(bool value)
        {
            tmPro.enabled = value;
        }

        public void PauseTimer()
        {
            gameTimer.Pause();
        }

        public void StartTimer()
        {
            gameTimer.Start();
        }

        public void StopTimer()
        {
            gameTimer.Stop();
        }

        public void ResetTimer()
        {
            gameTimer.Reset();
        }
    }
}

