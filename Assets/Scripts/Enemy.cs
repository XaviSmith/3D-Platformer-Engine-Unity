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

        [SerializeField] PlayerDetector playerDetector;
        [SerializeField] float wanderRadius = 20f;
        [SerializeField] float timeBetweenAttacks = 2f;

        StateMachine stateMachine;
        void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        CountdownTimer attackCooldownTimer; //how long should an attack take

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
            attackCooldownTimer = new CountdownTimer(timeBetweenAttacks);

            //************State machine************
            stateMachine = new StateMachine();
            EnemyWanderState wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            EnemyChaseState chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player); //Refactor this to use playerController to get player
            EnemyAttackState attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);

            AddTransition(wanderState, chaseState, new FuncPredicate(playerDetector.CanDetectPlayer));
            AddTransition(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            AddTransition(chaseState, attackState, new FuncPredicate(playerDetector.CanAttackPlayer));
            AddTransition(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

            //Set initial state
            stateMachine.SetState(wanderState);
            //****************************************
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.Update();
            attackCooldownTimer.Tick(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if(attackCooldownTimer.IsRunning)
            {
                return;
            }

            attackCooldownTimer.Start();

            Debug.Log("Enemy.Attack");
        }
    }
}

