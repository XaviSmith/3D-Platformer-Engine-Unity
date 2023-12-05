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
        player.CheckJumpBuffer();
    }

    public override void Update()
    {
        player.CheckCoyoteTime();
        base.Update();
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
    }

    public override void OnExit()
    {
        base.OnExit();
        //player.StopCoyoteTime();
    }
}
