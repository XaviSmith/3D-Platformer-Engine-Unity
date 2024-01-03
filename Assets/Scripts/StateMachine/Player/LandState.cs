using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class LandState : BaseState
{
    public LandState(PlayerController _player, Animator _animator, PlayerParticles _particles, PlayerSounds _playerSounds) : base(_player, _animator, _particles, _playerSounds) { }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(LandHash, CROSSFADEDURATION);
        player.StartLandStateTimer();
        particles.PlayJumpFX();
        player.CheckJumpBuffer();
        playerSounds.PlaySound(playerSounds.LandSound);
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
        player.StopLandStateTimer();
    }
}
