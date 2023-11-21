using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public abstract class EnemyBaseState : IState
    {
        protected readonly Enemy enemy;
        protected readonly Animator animator;

        //Animation Hashes
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int AttackHash = Animator.StringToHash("Attack_Front");
        protected static readonly int DieHash = Animator.StringToHash("Die");

        protected const float CROSSFADEDURATION = 0.1f;

        protected EnemyBaseState(Enemy _enemy, Animator _animator)
        {
            this.enemy = _enemy;
            this.animator = _animator;
        }

        public virtual void OnEnter()
        {
            //Not Used
        }

        public virtual void Update()
        {
            //Not Used
        }

        public void FixedUpdate()
        {
            //Not Used
        }

        public virtual void OnExit()
        {
            //Not Used
        }
        
    }

}
