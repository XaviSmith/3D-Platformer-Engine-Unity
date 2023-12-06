using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class DiveLandState : BaseState
{
    public DiveLandState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    public override void OnEnter()
    {
        Debug.Log("ENTERED DIVELAND STATE");
        base.OnEnter();
        animator.CrossFade(DiveLandHash, CROSSFADEDURATION);
        player.StartDiveLandTimers();
        player.SetDiveFlag(false);
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
        player.StopDiveLandTimers();
    }
}
