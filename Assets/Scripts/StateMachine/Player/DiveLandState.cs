using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DiveLandState : BaseState
{

    public DiveLandState(PlayerController _player, Animator _animator, PlayerParticles _particles, PlayerSounds _playerSounds) : base(_player, _animator, _particles, _playerSounds) { }

    public override void OnEnter()
    {
        base.OnEnter();
        playerSounds.PlaySound(playerSounds.WallSlideSound);
        animator.CrossFade(DiveLandHash, CROSSFADEDURATION);
        player.StartDiveLandTimers();
        player.SetDiveFlag(false);
        particles.PlayJumpFX();
        //player.CheckJumpBuffer();
    }

    public override void Update()
    {
        player.CheckCoyoteTime();
        base.Update();
    }

    public override void FixedUpdate()
    {
        player.HandleDive();
        //player.HandleMovement();
    }

    public override void OnExit()
    {
        base.OnExit();
        playerSounds.StopSound();
        player.StopDiveLandTimers();
    }
}
