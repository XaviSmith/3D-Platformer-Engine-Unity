using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DiveState : BaseState
{
    public DiveState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(DiveHash, CROSSFADEDURATION);
        player.StartDive();
    }

    public override void OnExit()
    {
        base.OnExit();
        player.ResetDiveVelocity();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleDive();
        player.HandleVerticalMovement();
    }
}
