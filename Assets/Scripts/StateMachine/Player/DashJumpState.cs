using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DashJumpState : BaseState
{
    public DashJumpState(PlayerController _player, Animator _animator, PlayerParticles _particles) : base(_player, _animator, _particles) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(JumpHash, CROSSFADEDURATION);
        player.StartDashJump();
        particles.ToggleRunFX(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        particles.ToggleRunFX(false);
        player.ResetDashJumpVelocity();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleDashJump();
        player.HandleMovement();
    }
}
