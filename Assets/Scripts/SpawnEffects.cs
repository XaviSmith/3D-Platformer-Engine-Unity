using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Platformer
{
    [RequireComponent(typeof(AudioSource))]
    public class SpawnEffects : MonoBehaviour
    {
        [SerializeField] GameObject spawnVFX;
        [SerializeField] float animationDuration = 1f;

        void Start()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);

            if(spawnVFX != null)
            {
                Instantiate(spawnVFX, transform.position, Quaternion.identity);
            }

            GetComponent<AudioSource>().Play();
        }
    }
}

