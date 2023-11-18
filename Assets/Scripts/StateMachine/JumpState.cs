using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class JumpState : BaseState
{
    public JumpState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        Debug.Log("JumpState.OnEnter");
        animator.CrossFade(JumpHash, CROSSFADEDURATION);
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleJump();
        player.HandleMovement();
    }
}
