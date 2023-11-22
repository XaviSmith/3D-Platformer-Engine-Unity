using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class AttackState : BaseState
{
    public AttackState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    //On Enter start the jump animation
    public override void OnEnter()
    {
        Debug.Log("AttackState.OnEnter");
        animator.CrossFade(AttackHash, CROSSFADEDURATION);
        player.Attack();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleJump();
        player.HandleMovement();
    }
}
