using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class WallJumpState : BaseState
{
    public WallJumpState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(WallJumpHash, CROSSFADEDURATION);
    }

    public override void FixedUpdate()
    {
        base.OnExit();
        player.HandleMovement();
        player.HandleJump();
    }

}
