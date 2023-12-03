using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyWanderState : EnemyBaseState
    {
        readonly NavMeshAgent agent;
        readonly Vector3 startPoint;
        readonly float wanderRadius;

        public EnemyWanderState(Enemy _enemy, Animator _animator, NavMeshAgent _agent, float _wanderRadius) : base(_enemy, _animator)
        {
            this.agent = _agent;
            this.startPoint = _enemy.transform.position;
            this.wanderRadius = _wanderRadius;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.CrossFade(WalkHash, CROSSFADEDURATION);
        }

        public override void Update()
        {
            //If we reached our destination or don't have one yet, find a random point on our navMesh to walk towards within our wanderRadius
            if (HasReachedDestination())
            {              
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += startPoint;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                Vector3 targetPosition = hit.position;

                agent.SetDestination(targetPosition);
            }
            base.Update();
        }

        //Also checks if we don't have a destination yet.
        bool HasReachedDestination()
        {
            //They've reached their dest if their path isn't calculating, they've reached the end of their radius, and they have no new path + have stopped moving
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}

