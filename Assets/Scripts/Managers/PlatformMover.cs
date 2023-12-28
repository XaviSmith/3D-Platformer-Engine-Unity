using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Relies on DoTween package
/// </summary>
public class PlatformMover : MonoBehaviour
{
    [SerializeField] Vector3 moveTo= Vector3.zero;
    [SerializeField] float moveTime = 1f;
    [SerializeField] Ease ease = Ease.InOutQuad;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        Move();
    }

    void Move()
    {
        //Move to the moveTo pos, then move back on completion, then call the function again recursively.
        transform.DOMove(startPos + moveTo, moveTime)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }

}
