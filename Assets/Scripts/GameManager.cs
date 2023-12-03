using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        List<PlayerManager> Players { get; set; } = new List<PlayerManager>();
        public Transform MainPlayer { get; private set; }
        public int Score { get; private set; }

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
        }

        public void AddScore(int score)
        {
            Debug.Log("Adding " + score + " to score.");
            Score += score;
        }
    }

}
