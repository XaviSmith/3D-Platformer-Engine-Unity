using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Platformer
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Animator animator;
        [SerializeField] Health health;

        [SerializeField] PlayerDetector playerDetector;
        [SerializeField] float wanderRadius = 20f;
        [SerializeField] float timeBetweenAttacks = 2f;

        [Header("Death Settings")]
        [SerializeField] float deathTime;
        [SerializeField] Vector3 deathDirection;
        [SerializeField] float deathMoveSpeed;
        [SerializeField] GameObject deathVFX;
        [SerializeField] float deathCollisionRadius = 0.5f; //When we're dead check a sphere within this radius to see if we hit something. If we did, explode early.
        [SerializeField] Vector3 deathColliderOffset = Vector3.zero;

        StateMachine stateMachine;
        void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        [Header("Attacks")]
        [SerializeField] BaseAttack baseAttack;

        CountdownTimer attackTimer; //how long should an attack take
        CountdownTimer deathTimer; //How long after dying before we poof away;

        private void OnEnable()
        {
            if (playerDetector == null)
            {
                playerDetector = GetComponent<PlayerDetector>();
            }

            if(agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            deathTimer = new CountdownTimer(deathTime);

            deathTimer.OnTimerStop += () =>
            {
                if (deathVFX != null)
                {
                    Instantiate(deathVFX, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            };

            //************State machine************
            stateMachine = new StateMachine();
            EnemyWanderState wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            EnemyChaseState chaseState = new EnemyChaseState(this, animator, agent, GameManager.Instance.MainPlayer); //Refactor this to use playerController to get player
            EnemyAttackState attackState = new EnemyAttackState(this, animator, agent, GameManager.Instance.MainPlayer);
            EnemyDeathState deathState = new EnemyDeathState(this, animator);

            AddTransition(wanderState, chaseState, new FuncPredicate(playerDetector.CanDetectPlayer));
            AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            AddTransition(chaseState, attackState, new FuncPredicate(playerDetector.CanAttackPlayer));
            AddTransition(attackState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer() && !baseAttack.IsRunning));

            AddAnyTransition(deathState, new FuncPredicate(() => health.IsDead));

            //Set initial state
            stateMachine.SetState(wanderState);
            //****************************************
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
            deathTimer.Tick(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if(baseAttack.IsRunning)
            {
                return;
            }

            baseAttack.StartAttackTimer();
            baseAttack.Attack();
            //playerDetector.PlayerHealth?.TakeDamage(10);
            //Debug.Log("Enemy.Attack");
        }

        public void Die()
        {
            if(!deathTimer.IsRunning)
            {
                agent.enabled = false;
                transform.LookAt(GameManager.Instance.MainPlayer);
                deathTimer.Start();
            }
            
        }

        public void HandleDeathMovement()
        {
            //Check for collisions, if we get one explode early. Cheaper than using a rigidbody for this one thing.
            //Make sure our sphere pos isn't already touching the ground when we start this check.
            Collider[] collisions = Physics.OverlapSphere(transform.position - deathColliderOffset, deathCollisionRadius);

            foreach (Collider collision in collisions)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
                {
                    deathTimer.Stop();
                    return;
                }
            }

            //Otherwise keep flying.
            transform.position += deathDirection * deathMoveSpeed * Time.deltaTime;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position - deathColliderOffset, deathCollisionRadius);
        }
    }
}

