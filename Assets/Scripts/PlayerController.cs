using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;
using Utils;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody rb;
        [SerializeField] GroundChecker groundChecker;
        [SerializeField] WallJumpChecker wallJumpChecker;
        [SerializeField] Animator animator;
        [SerializeField] CinemachineFreeLook freeLookVCam;
        [SerializeField] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float airSpeed = 20f; //if we exceed this value, slow us down.
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f; //how fast the animator changes speed

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f; //how long can we hold the button.
        [SerializeField] float jumpCooldown = 0f;
        //[SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 1f; //for if we want to bring them to the ground faster or slower. NOTE: It's generally better to change the overall gravity for tweaks since this won't affect speed falling off a platform     
        bool isFalling = false;

        [Header("Wall Jump Settings")]
        [SerializeField] float wallSlideSpeed = 1f;
        [SerializeField] Vector2 wallJumpForce = new Vector2(24f, 20f);
        [SerializeField] float wallJumpCooldown = 0.1f;
        Vector3 lastWallDirection;

        [Header("Dash Settings")] //To dash we basically just multiply horizontal movespeed * dashVelocity
        [SerializeField] float dashDuration = 0.5f;
        [SerializeField] float dashCooldown = 0.2f;
        [SerializeField] float dashForce = 10f;
        bool isDashing = false;

        [Header("AirDash Settings")] //To dash we basically just multiply horizontal movespeed * dashVelocity
        [SerializeField] float airDashDuration = 0.25f;
        [SerializeField] float airDashForce = 10f;
        bool isAirDashing = false;

        [Header("Attacks")]
        [SerializeField] BaseAttack baseAttack;
        [SerializeField] SlideAttack slideAttack;

        [SerializeField] Transform mainCam; //The camera we use to determine our relative movement

        float currSpeed;
        float velocity; //output var for SmoothDamp
        float jumpVelocity;
        float dashVelocity = 1f;

        Vector3 movement; //taken from out input

        List<Timer> timers;

        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer; //if we want a cooldown on how long after landing before we can jump again.

        CountdownTimer wallJumpTimer;

        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;

        CountdownTimer airDashTimer;
        CountdownTimer airDashCooldownTimer;

        StateMachine stateMachine;
        //State Machine Helper methods, consider just moving this into the stateMachine class.
        void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        //Animator params
        static readonly int Speed = Animator.StringToHash("Speed"); //Speed for our animation blendTree so we know when to transition from idle walk and run
        static readonly int JumpVelocity = Animator.StringToHash("JumpVelocity");

        private void Awake()
        {
            //Set cameras
            if(mainCam == null)
            {
                mainCam = Camera.main.transform;
            }
            
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward); //For when our target warps to somewhere else, how do we update the camera to move to them

            rb.freezeRotation = true;

            SetupTimers();
            SetupStateMachine();

        }

        private void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            //Declare States
            LocomotionState locomotionState = new LocomotionState(this, animator);
            JumpState jumpState = new JumpState(this, animator);
            FallState fallState = new FallState(this, animator);
            WallSlideState wallSlideState = new WallSlideState(this, animator);
            WallJumpState wallJumpState = new WallJumpState(this, animator);
            DashState dashState = new DashState(this, animator);
            DashJumpState dashJumpState = new DashJumpState(this, animator);
            AttackState attackState = new AttackState(this, animator);

            //Define transitions
            AddTransition(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            AddTransition(locomotionState, attackState, new FuncPredicate(() => baseAttack.IsRunning));
            AddTransition(locomotionState, dashState, new FuncPredicate(() => groundChecker.IsGrounded && dashTimer.IsRunning));
            AddTransition(locomotionState, fallState, new FuncPredicate(() => !groundChecker.IsGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
            AddTransition(jumpState, locomotionState, new FuncPredicate(() => groundChecker.IsGrounded && !jumpTimer.IsRunning));
            AddTransition(jumpState, fallState, new FuncPredicate(() => !groundChecker.IsGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
            AddTransition(fallState, wallSlideState, new FuncPredicate(() => wallJumpChecker.IsTouchingWall && jumpVelocity <= 0f && !wallJumpTimer.IsRunning));
            AddTransition(wallSlideState, wallJumpState, new FuncPredicate(() => !groundChecker.IsGrounded && wallJumpTimer.IsRunning));
            AddTransition(wallSlideState, fallState, new FuncPredicate(() => !wallJumpTimer.IsRunning && !wallJumpChecker.IsTouchingWall));
            AddTransition(wallJumpState, fallState, new FuncPredicate(() => !jumpTimer.IsRunning && !wallJumpTimer.IsRunning));
            AddTransition(dashState, locomotionState, new FuncPredicate(() => groundChecker.IsGrounded && !dashTimer.IsRunning));
            AddTransition(dashState, jumpState, new FuncPredicate(() => !groundChecker.IsGrounded && !dashTimer.IsRunning));
            AddTransition(dashState, dashJumpState, new FuncPredicate(() => jumpTimer.IsRunning && dashTimer.IsRunning));
            AddTransition(attackState, locomotionState, new FuncPredicate(() => !baseAttack.IsRunning));

            AddAnyTransition(locomotionState, new FuncPredicate(() => groundChecker.IsGrounded && !baseAttack.IsRunning && !jumpTimer.IsRunning && !dashTimer.IsRunning));
            //AddAnyTransition(dashState, new FuncPredicate(() => dashTimer.IsRunning));

            //set initial state
            stateMachine.SetState(locomotionState);
        }

        void SetupTimers()
        {
            //Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            wallJumpTimer = new CountdownTimer(wallJumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce; //whenever timer starts we start out with jumpForce
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start(); //For if we want a cooldown on how long after landing we want to be able to jump again.

            wallJumpTimer.OnTimerStop += () => jumpVelocity = 0f;
            dashTimer.OnTimerStart += () =>
            {
                isDashing = true;
                dashVelocity = dashForce;

                //Cancel jump so we can halt vertical momentum
                if (jumpTimer.IsRunning)
                {
                    jumpTimer.Stop();
                }

            };
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                isDashing = false;
                dashCooldownTimer.Start();
            };


            timers = new List<Timer>(5) { jumpTimer, jumpCooldownTimer, wallJumpTimer, dashTimer, dashCooldownTimer}; //defining capacity for some optimization;
        }
        //*****************************Jumping (also see HandleJump lower down)*****************************

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
            input.Attack += OnAttack;
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
            input.Attack -= OnAttack;
        }

        void OnJump(bool performed)
        {
            if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpTimer.Start();
            } else if(!performed && jumpTimer.IsRunning) //We let go of the jump button early, shorthop
            {
                jumpTimer.Stop();
            }

            if(performed && !wallJumpTimer.IsRunning && wallJumpChecker.IsTouchingWall && !groundChecker.IsGrounded)
            {
                wallJumpTimer.Start();
            }
        }

        void OnDash(bool performed)
        {
            if(!isDashing)
            {
                if (performed && groundChecker.IsGrounded && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning && movement.magnitude > 0)
                {
                    dashTimer.Start();
                    slideAttack.StartAttack();
                } 
            }
            
        }

        void OnAttack()
        {
            //ground Attack
            if(groundChecker.IsGrounded)
            {
                baseAttack.StartAttack(); //Start the timer if we're not in cooldown and set the bool to transition our state.
            }     
            
            //air Attack
            if(!groundChecker.IsGrounded)
            {

            }
        }

        //TODO: Have one function for attacks or have the states call the attacks directly.
        public void Attack()
        {
            baseAttack.Attack(); //Called when we enter the Attack State to actually attack.
        }

        public void SlideAttack()
        {
            slideAttack.Attack();
        }

        //**********************************************************************

        // Start is called before the first frame update
        void Start()
        {
            input.EnablePlayerActions();
        }

        // Update is called once per frame
        void Update()
        {
            movement = new Vector3(input.Direction.x, 0, input.Direction.y);
            stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void HandleTimers()
        {
            foreach(Timer t in timers)
            {
                t.Tick(Time.deltaTime);
            }
        }

        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currSpeed);
            animator.SetFloat(JumpVelocity, jumpVelocity);
        }

        //Jump has an initial burst of vertical speed, then apply less velocity over time, then let gravity take over.
        public void HandleJump()
        {
            // If we're on the ground and not jumping, keep jump velocity at 0
            if(!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = 0f;
                return;
            }

            if(!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;         
            }

            //Apply the jump velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);

        }

        public void HandleWallSlide()
        {
            lastWallDirection = wallJumpChecker.WallNormal;
            jumpVelocity = 0f;
            rb.velocity = new Vector3(0f, -wallSlideSpeed, 0f);
        }

        public void HandleWallJump()
        {
            jumpVelocity = wallJumpForce.y * wallJumpTimer.Progress;

            rb.velocity = new Vector3(lastWallDirection.x * wallJumpForce.x, jumpVelocity, lastWallDirection.z * wallJumpForce.x);
        }

        public void HandleDashJump()
        {
            // If we're on the ground and not jumping, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = 0f;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            //Apply the jump velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
            dashVelocity = dashForce;

        }


        //*************************Movement*************************************

        //Called by LocomotionState and JumpState
        public void HandleMovement()
        {
            //Have movement direction be relative to our camera's rotation around the y axis (vector3.up)
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            adjustedDirection = Vector3.ProjectOnPlane(adjustedDirection, groundChecker.CurrSlopeNormal);
                   
            HandleRotation(adjustedDirection);

            //transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, groundChecker.currSlopeNormal));

            if (adjustedDirection.magnitude > 0)
            {   
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);

            }
            else
            {
                SmoothSpeed(0);

                //reset horizontal velocity so we can stop on a dime
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }

        void SmoothSpeed(float targetSpeed)
        {
            currSpeed = Mathf.SmoothDamp(currSpeed, targetSpeed, ref velocity, smoothTime);
        }

        public void ResetDashVelocity()
        {
            dashVelocity = 1f;
        }

        void HandleRotation(Vector3 adjustedDirection)
        {
            if(!groundChecker.IsGrounded)
            {
                //transform.rotation= new Quaternion(0f, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                adjustedDirection = new Vector3(wallJumpTimer.IsRunning ? adjustedDirection.x : transform.forward.x, adjustedDirection.y, wallJumpTimer.IsRunning ? adjustedDirection.z : transform.forward.z);
            } 

            //Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection); //Have player look where they're going
        }

        //Move the player
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            if(!groundChecker.IsGrounded)
            {
                HandleAirMovement(adjustedDirection);
                return;
            }

            Vector3 velocity = adjustedDirection * moveSpeed * dashVelocity * Time.fixedDeltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        void HandleAirMovement(Vector3 adjustedDirection)
        {
            //If we're above airspeed slow us down, otherwise speed us up.
            float targetSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude < airSpeed ? airSpeed : -airSpeed;

            Vector3 velocity = adjustedDirection * airSpeed * Time.fixedDeltaTime;
            rb.velocity += new Vector3(velocity.x, 0f, velocity.z);
        }

        public void HaltVerticalAirMomentum() //For airdashes since we don't want to rise or fall
        {
            if(!groundChecker.IsGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
            
        }

        public void FlipDirectionFromWall()
        {
            Vector3 adjustedDirection = wallJumpChecker.WallNormal;
            HandleRotation(adjustedDirection);
        }

        //*************************************************************************************************
    }
}

