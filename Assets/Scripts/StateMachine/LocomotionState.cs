using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class LocomotionState : BaseState
{
    public LocomotionState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    public override void OnEnter()
    {
        //play jump anim
        animator.CrossFade(LocomotionHash, CROSSFADEDURATION);
        Debug.Log("LocomotionState.OnEnter");
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
    }
}
