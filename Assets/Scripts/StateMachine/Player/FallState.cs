using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class FallState : BaseState
{
    public FallState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(JumpHash, CROSSFADEDURATION);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.HandleJump();
    }

}
