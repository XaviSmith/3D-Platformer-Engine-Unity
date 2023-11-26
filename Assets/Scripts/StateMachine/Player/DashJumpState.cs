using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DashJumpState : BaseState
{
    public DashJumpState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        Debug.Log("DashJumpState.OnEnter");
        animator.CrossFade(JumpHash, CROSSFADEDURATION);
    }

    public override void OnExit()
    {
        player.ResetDashVelocity();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleDashJump();
        player.HandleMovement();
    }
}
