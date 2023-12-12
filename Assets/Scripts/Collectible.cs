using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    //NOTE: Have these be on their own layer as there are potentially many and we don't want to waste time checking a bunch of unimportant collisions
    public class Collectible : Entity
    {
        [SerializeField] Events eventToTrigger;
        [SerializeField] int value = 10; //FIXME set using Factory
        [SerializeField] GameObject pickupVFX;
        [SerializeField] float pickupVFXScale = 1f;
        bool collected = false; //to ensure we don't keep collecting stars if we enter and exit a collectible rapidly

        void OnTriggerEnter(Collider other)
        {
            Collect();                
            Destroy(gameObject);
        }

        protected virtual void Collect()
        {
            if(!collected)
            {
                collected = true;
                EventManager<int>.TriggerEvent(eventToTrigger.ToString(), value);
                if (pickupVFX != null)
                {
                    GameObject spawn = Instantiate(pickupVFX, transform.position, Quaternion.identity);
                    spawn.transform.localScale *= pickupVFXScale;
                }
            }
            
        }

    }
}
