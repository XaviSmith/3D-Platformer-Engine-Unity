using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        List<PlayerManager> Players { get; set; } = new List<PlayerManager>();
        public Transform MainPlayer { get; private set; }
        [SerializeField] int starsNeeded;
        public int Score { get; private set; }
        public int StarCount { get; private set; }
        public int CoinCount { get; private set; }
        public GameTimer gameTimer;
        public PlayerController GetPlayerController => MainPlayer?.GetComponent<PlayerController>();
        public OrbitCamera currCamera;
        [SerializeField] float starJingleTime = 0.4f;

        public bool HideTexts;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayer(PlayerManager player)
        {
            if(!Players.Contains(player))
            {
                Players.Add(player);
            }  
            
            if(MainPlayer == null)
            {
                MainPlayer = Players[0].transform;
            }
        }

        private void OnEnable()
        {
            //EventManager<int>.StartListening(Events.UPDATESCORE.ToString(), AddScore);
            EventManager<int>.StartListening(Events.GETSTAR.ToString(), GetStar);
            EventManager<int>.StartListening(Events.GETCOIN.ToString(), GetCoin);
        }

        private void OnDisable()
        {
            //EventManager<int>.StartListening(Events.UPDATESCORE.ToString(), AddScore);
            EventManager<int>.StopListening(Events.GETSTAR.ToString(), GetStar);
            EventManager<int>.StopListening(Events.GETCOIN.ToString(), GetCoin);
        }
        public void AddScore(int score)
        {
            Score += score;
        }

        public void GetStar(int val)
        {
            SoundManager.Instance.LowerMusicVolume(0.5f, starJingleTime);
            StarCount += val;
            EventManager<int>.TriggerEvent(Events.UPDATESTAR.ToString(), StarCount);

            if(StarCount >= starsNeeded)
            {
                EventManager.TriggerEvent(Events.ENDGAME.ToString());
            }
        }

        public void GetCoin(int val)
        {
            CoinCount += val;
            EventManager<int>.TriggerEvent(Events.UPDATECOIN.ToString(), CoinCount);
        }

    }

}
