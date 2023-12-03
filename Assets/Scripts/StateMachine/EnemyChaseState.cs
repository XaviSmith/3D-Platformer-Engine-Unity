using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyChaseState : EnemyBaseState
    {
        readonly NavMeshAgent agent;
        readonly Transform target;

        public EnemyChaseState(Enemy _enemy, Animator _animator, NavMeshAgent _agent, Transform _target) : base(_enemy, _animator)
        {
            this.agent = _agent;
            this.target = _target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.CrossFade(WalkHash, CROSSFADEDURATION);
        }

        // Update is called once per frame
        public override void Update()
        {
            agent.SetDestination(target.position);
            base.Update();
        }
    }
}

