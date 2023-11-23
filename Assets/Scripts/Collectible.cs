using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] int score = 10; //FIXME set using Factory

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                EventManager<int>.TriggerEvent(Events.UPDATESCORE.ToString(), score);
                EventManager.TriggerEvent(Events.TEST.ToString());
                Destroy(gameObject);
            }
        }
    }
}
