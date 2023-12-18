using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerParticles : MonoBehaviour
    {
        [SerializeField] ParticleSystem runFx;
        [SerializeField] ParticleSystem jumpFx;

        public void ToggleRunFX(bool val)
        {
            if (val)
            {
                runFx.Play(true);
            }
            else
            {
                runFx.Stop(true);
            }

        }

        public void PlayJumpFX()
        {
            Instantiate(jumpFx, transform.position, jumpFx.transform.rotation);
        }
    }
}

