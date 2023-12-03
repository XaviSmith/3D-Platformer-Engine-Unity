using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyAttackState : EnemyBaseState
    {
        readonly NavMeshAgent agent;
        readonly Transform target;

        public EnemyAttackState(Enemy _enemy, Animator _animator, NavMeshAgent _agent, Transform _target) : base(_enemy, _animator)
        {
            this.agent = _agent;
            this.target = _target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            enemy.Attack();
            animator.CrossFade(AttackHash, CROSSFADEDURATION);
        }

        public override void Update()
        {
            base.Update();
            agent.SetDestination(target.position);         
        }
    }
}

