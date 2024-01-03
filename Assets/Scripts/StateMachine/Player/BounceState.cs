using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class BounceState : BaseState
{
    public BounceState(PlayerController _player, Animator _animator, PlayerParticles _particles, PlayerSounds _playerSounds) : base(_player, _animator, _particles, _playerSounds) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        playerSounds.PlaySound(playerSounds.BounceSound);
        //playerSounds.PlaySound(playerSounds.JumpSound);
        base.OnEnter();
        animator.CrossFade(JumpHash, CROSSFADEDURATION);
        particles.PlayJumpFX();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleVerticalMovement();
        player.HandleMovement();
    }
}
