using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class EnemyDeathState : EnemyBaseState
    {
        [SerializeField] Vector3 hitDirection;

        public EnemyDeathState(Enemy _enemy, Animator _animator) : base(_enemy, _animator){ }

        public override void OnEnter()
        {
            base.OnEnter();
            enemy.Die();          
            animator.CrossFade(DieHash, CROSSFADEDURATION);
        }

        public override void Update()
        {
            base.Update();
            enemy.HandleDeathMovement();
        }
    }

}
