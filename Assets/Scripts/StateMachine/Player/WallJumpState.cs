using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class WallJumpState : BaseState
{
    public WallJumpState(PlayerController _player, Animator _animator, PlayerParticles _particles) : base(_player, _animator, _particles) { }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(WallJumpHash, CROSSFADEDURATION);
        particles.ToggleRunFX(false);
        particles.PlayJumpFX();
    }

    public override void FixedUpdate()
    {
        //player.HandleMovement();
        player.HandleWallJump();
    }

}
