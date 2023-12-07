using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// Goes on the player.
    /// Makes moving platforms parent of the player so we move along with them.
    /// </summary>
    public class PlatformCollisionHandler : MonoBehaviour
    {
        Transform platform;
        Vector3 scale; //to prevent any weirdness with platforms rescaling us

        private void OnEnable()
        {
            scale = transform.localScale;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                //make sure we're landing on the platform from above
                ContactPoint contact = other.GetContact(0);
                if(contact.normal.y < 0.5f)
                {
                    return;
                }

                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if(other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                transform.localScale = scale;
                platform = null;
            }
        }
    }
}

