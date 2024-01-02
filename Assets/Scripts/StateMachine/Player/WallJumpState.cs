using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class WallJumpState : BaseState
{
    PlayerSounds playerSounds;
    public WallJumpState(PlayerController _player, Animator _animator, PlayerParticles _particles, PlayerSounds _playerSounds) : base(_player, _animator, _particles) { this.playerSounds = _playerSounds; }

    public override void OnEnter()
    {
        base.OnEnter();
        playerSounds.PlaySound(playerSounds.WallJumpSound);
        animator.CrossFade(WallJumpHash, CROSSFADEDURATION);
        particles.PlayJumpFX();
    }

    public override void FixedUpdate()
    {
        //player.HandleMovement();
        player.HandleWallJump();
    }

}
