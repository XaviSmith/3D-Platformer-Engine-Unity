using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

public class AttackState : BaseState
{
    public AttackState(PlayerController _player, Animator _animator) : base(_player, _animator) { }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.CrossFade(AttackHash, CROSSFADEDURATION);
        player.Attack();
    }

    public override void FixedUpdate()
    {
        //call Player's jump logic and move logic
        player.HandleVerticalMovement();
        player.HandleMovement();
    }
}
