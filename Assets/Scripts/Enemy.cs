using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Platformer
{
    public class Enemy : Entity
    {
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Health health;

        [SerializeField] protected PlayerDetector playerDetector;
        [SerializeField] protected float wanderRadius = 20f;
        [SerializeField] protected float timeBetweenAttacks = 2f;

        [Header("Death Settings")]
        [SerializeField] protected bool drawDeathCollider;
        [SerializeField] protected float deathTime;
        [SerializeField] protected Vector3 deathDirection;
        [SerializeField] protected float deathMoveSpeed;
        [SerializeField] protected AudioClip deathSFX;
        [SerializeField] protected GameObject deathVFX;
        [SerializeField] protected float deathCollisionRadius = 0.5f; //When we're dead check a sphere within this radius to see if we hit something. If we did, explode early.
        [SerializeField] protected Vector3 deathColliderOffset = Vector3.zero;

        protected StateMachine stateMachine;
        protected void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        [Header("Attacks")]
        [SerializeField] protected BaseAttack baseAttack;

        protected CountdownTimer attackTimer; //how long should an attack take
        protected CountdownTimer deathTimer; //How long after dying before we poof away;

        protected virtual void OnEnable()
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
        protected void Start()
        {
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            deathTimer = new CountdownTimer(deathTime);

            deathTimer.OnTimerStop += () =>
            {
                DieSFX();
                DieVFX();             
            };

            //************State machine************
            stateMachine = new StateMachine();
            EnemyWanderState wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            EnemyChaseState chaseState = new EnemyChaseState(this, animator, agent, GameManager.Instance.MainPlayer); //Refactor this to use playerController to get player
            EnemyAttackState attackState = new EnemyAttackState(this, animator, agent, GameManager.Instance.MainPlayer);
            EnemyDeathState deathState = new EnemyDeathState(this, animator);
            EnemySquashedState squashedState = new EnemySquashedState(this, animator);

            AddTransition(wanderState, chaseState, new FuncPredicate(playerDetector.CanDetectPlayer));
            AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            AddTransition(chaseState, attackState, new FuncPredicate(playerDetector.CanAttackPlayer));
            AddTransition(attackState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer() && !baseAttack.IsRunning));

            AddAnyTransition(deathState, new FuncPredicate(() => health.IsDead));
            AddAnyTransition(squashedState, new FuncPredicate(() => health.IsSquashed));

            //Set initial state
            stateMachine.SetState(wanderState);
            //****************************************
        }

        // Update is called once per frame
        protected void Update()
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
            deathTimer.Tick(Time.deltaTime);
        }

        protected void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if(baseAttack.IsRunning)
            {
                return;
            }

            baseAttack.StartAttack();
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

        public void DieSFX()
        {
            if(deathSFX != null)
            {
                SoundManager.Instance.PlaySFX(deathSFX);
            }
        }

        public void DieVFX()
        {
            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
                Destroy(gameObject);
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

        protected void OnDrawGizmos()
        {
            if(drawDeathCollider)
            {
                Gizmos.color = Color.black;

                Gizmos.DrawWireSphere(transform.position - deathColliderOffset, deathCollisionRadius);
            }
            
        }
    }
}

