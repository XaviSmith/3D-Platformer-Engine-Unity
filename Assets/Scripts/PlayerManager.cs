using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerManager : MonoBehaviour
    {
        //For if we want it to automatically 
        private void Awake()
        {
            if(GameManager.Instance == null)
            {
                Debug.LogError("NO GAME MANAGER INSTANCE! Make sure GameManager comes before this in script execution order!");
            }
            GameManager.Instance.AddPlayer(this);
        }
    }
}

