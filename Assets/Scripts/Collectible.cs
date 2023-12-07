using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] Events eventToTrigger;
        [SerializeField] int value = 10; //FIXME set using Factory
        [SerializeField] GameObject pickupVFX;
        [SerializeField] float pickupVFXScale = 1f;

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                Collect();                
                Destroy(gameObject);
            }
        }

        protected virtual void Collect()
        {
            EventManager<int>.TriggerEvent(eventToTrigger.ToString(), value);
            if(pickupVFX != null)
            {
                GameObject spawn = Instantiate(pickupVFX, transform.position, Quaternion.identity);
                spawn.transform.localScale *= pickupVFXScale;
            }
        }

    }
}
