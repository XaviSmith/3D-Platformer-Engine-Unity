using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class WallSlideState : BaseState
{
    public WallSlideState(PlayerController _player, Animator _animator, PlayerParticles _particles) : base(_player, _animator, _particles) { }

    public override void OnEnter()
    {
        base.OnEnter();        
        animator.CrossFade(WallSlideHash, CROSSFADEDURATION);
        player.SetDiveFlag(false);
        particles.ToggleRunFX(true);
    }

    public override void FixedUpdate()
    {
        player.HandleWallSlide();
    }

    public override void OnExit()
    {
        base.OnExit();
        player.FlipDirectionFromWall();
    }

}
