using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DashState : BaseState
{
    public DashState(PlayerController _player, Animator _animator, PlayerParticles _particles) : base(_player, _animator, _particles) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(DashHash, CROSSFADEDURATION);
        player.DashAttack();
        particles.ToggleRunFX(true);
    }

    public override void FixedUpdate()
    {
        base.OnExit();
        //call Player's move logic since dashing is just a speed modifier for now.
        player.HandleMovement();
        player.HaltVerticalAirMomentum();
    }

    public override void Update()
    {
        player.CheckCoyoteTime();
    }

    public override void OnExit()
    {
        base.OnExit();
        particles.ToggleRunFX(false);
    }
}