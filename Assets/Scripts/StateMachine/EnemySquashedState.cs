using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class EnemySquashedState : EnemyBaseState
    {
        public EnemySquashedState(Enemy _enemy, Animator _animator) : base(_enemy, _animator){ }

        public override void OnEnter()
        {
            base.OnEnter();
            enemy.DieVFX();        //We don't play dieSFX since the bounce sound will already play   
            animator.CrossFade(DieHash, CROSSFADEDURATION);
        }

    }

}
