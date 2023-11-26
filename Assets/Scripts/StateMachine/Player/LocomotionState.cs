using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class LocomotionState : BaseState
{
    public LocomotionState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(LocomotionHash, CROSSFADEDURATION);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
    }
}
