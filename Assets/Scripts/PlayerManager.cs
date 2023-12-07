using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;

        //For if we want it to automatically 
        private void Awake()
        {
            if(GameManager.Instance == null)
            {
                Debug.LogError("NO GAME MANAGER INSTANCE! Make sure GameManager comes before this in script execution order!");
            }
            GameManager.Instance.AddPlayer(this);
        }

        void OnEnable()
        {
            EventManager.StartListening(Events.PLAYERDIE.ToString(), PlayerDie);
        }

        void PlayerDie()
        {
            //Play any sounds/animations
            Respawn();
        }

        void Respawn()
        {
            transform.position = spawnPoint.position;
        }
    }
}

