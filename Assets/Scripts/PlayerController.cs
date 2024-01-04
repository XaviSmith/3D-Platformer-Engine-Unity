using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;
using Utils;

//Extended from git-amend's basic tutorial here https://youtu.be/--_CH5DYz0M?si=jgfx6KJpbuye1jUH 
namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public static Transform Transform { get; private set; } //We can move this to GameManager and add players to a list if we decide to have multiple, but this is good enough for now.
        [SerializeField] bool canMove = true;
        [Header("References")]
        [SerializeField] Rigidbody rb;
        [SerializeField] GroundChecker groundChecker;
        [SerializeField] WallJumpChecker wallJumpChecker;
        [SerializeField] Animator animator;
        [SerializeField] CinemachineFreeLook freeLookVCam;
        [SerializeField] InputReader input;
        [SerializeField] PlayerParticles particles;
        [SerializeField] PlayerSounds playerSounds;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float airAcceleration = 8f;
        [SerializeField] float airMaxSpeed = 20f; //if we exceed this value, slow us down.
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f; //how fast the animator changes speed

        bool playingRunSound;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float bounceForce = 10f; //for when we bounce off things
        [SerializeField] float jumpDuration = 0.5f; //how long can we hold the button.
        [SerializeField] float bounceDuration = 0.4f;
        [SerializeField] float jumpCooldown = 0f;
        //[SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 1f; //for if we want to bring them to the ground faster or slower. NOTE: It's generally better to change the overall gravity for tweaks since this won't affect speed falling off a platform     
        [SerializeField] float coyoteTime = 0.2f;
        [SerializeField] float jumpBuffer = 0.5f;
        [SerializeField] float landStateDuration = 0.458f;

        [Header("Wall Jump Settings")]
        [SerializeField] float wallSlideSpeed = 1f;
        [SerializeField] Vector2 wallJumpForce = new Vector2(24f, 20f);
        [SerializeField] float wallJumpDuration = 0.1f;
        [SerializeField] float wallSlideLockout = 0.1f; //how long are we locked out of wallsliding after cancelling it
        Vector3 lastWallDirection;

        [Header("Dash Settings")] //To dash we basically just multiply horizontal movespeed * dashVelocity
        [SerializeField] float dashDuration = 0.5f;
        [SerializeField] float dashCooldown = 0.2f;
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashJumpForce = 11f;

        [Header("Air Dive Settings")] //To dash we basically just multiply horizontal movespeed * dashVelocity
        [SerializeField] float airDashLiftForce = 5f; //for the initial part
        [SerializeField] float airDashLiftTime = 0.1f;
        [SerializeField] Vector2 airDashForce = new Vector2(10f, -2f); //x axis is horizontal movement, y axis is vertical movement
        [SerializeField] float diveLandStateDuration = 1f;
        [SerializeField] float diveLandSoundDuration = 1f;
        [SerializeField] float diveLandLockout = 0.1f; //How long after diving before we can regain movement

        bool diveFlag = false;

        [Header("Attacks")]
        [SerializeField] BaseAttack baseAttack;
        [SerializeField] SlideAttack dashAttack;
        [SerializeField] BaseAttack diveAttack;


        [SerializeField] Transform mainCam; //The camera we use to determine our relative movement

        float currSpeed;
        float velocity; //output var for SmoothDamp
        float jumpVelocity;
        float dashVelocity = 1f;
        float dashJumpVelocity = 1f;
        Vector2 airDashVelocity = new Vector2(1f, 0f);

        Vector3 movement; //taken from out input

        List<Timer> timers;

        CountdownTimer jumpTimer;
        CountdownTimer bounceTimer;
        CountdownTimer jumpCooldownTimer; //if we want a cooldown on how long after landing before we can jump again.
        CountdownTimer coyoteTimer;
        CountdownTimer jumpBufferTimer;
        CountdownTimer landStateTimer;

        CountdownTimer wallJumpTimer;
        CountdownTimer wallSlideCancelTimer;

        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;

        CountdownTimer airDashLiftTimer;
        CountdownTimer diveLandTimer;
        CountdownTimer diveLandLockoutTimer;
        CountdownTimer diveLandSoundTimer;

        //State Machine stuff
        StateMachine stateMachine;
        bool ShouldFall => !groundChecker.IsGrounded && !groundChecker.ShouldSnapToGround && !groundChecker.IsOnSlope && groundChecker.TimeSinceLastGrounded >= coyoteTime && !jumpTimer.IsRunning && !dashTimer.IsRunning;
        bool isGrounded => groundChecker.IsGrounded || groundChecker.IsOnSlope;

        //State Machine Helper methods, consider just moving this into the stateMachine class.
        void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        //Animator params
        static readonly int Speed = Animator.StringToHash("Speed"); //Speed for our animation blendTree so we know when to transition from idle walk and run
        static readonly int JumpVelocity = Animator.StringToHash("JumpVelocity");

        private void Awake()
        {
            //Set transform instance
            Transform = transform;
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
            LocomotionState locomotionState = new LocomotionState(this, animator, particles);
            JumpState jumpState = new JumpState(this, animator, particles, playerSounds);
            BounceState bounceState = new BounceState(this, animator, particles, playerSounds);
            FallState fallState = new FallState(this, animator);
            LandState landState = new LandState(this, animator, particles, playerSounds);
            WallSlideState wallSlideState = new WallSlideState(this, animator, particles, playerSounds);
            WallJumpState wallJumpState = new WallJumpState(this, animator, particles, playerSounds);
            DashState dashState = new DashState(this, animator, particles, playerSounds);
            DashJumpState dashJumpState = new DashJumpState(this, animator, particles, playerSounds);
            AttackState attackState = new AttackState(this, animator);
            DiveState diveState = new DiveState(this, animator, particles, playerSounds);
            DiveLandState diveLandState = new DiveLandState(this, animator, particles, playerSounds);

            //Define transitions
            AddTransition(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning || jumpBufferTimer.IsRunning));
            AddTransition(locomotionState, attackState, new FuncPredicate(() => baseAttack.IsRunning));
            AddTransition(locomotionState, dashState, new FuncPredicate(() => isGrounded && dashTimer.IsRunning));
            AddTransition(locomotionState, fallState, new FuncPredicate(() => ShouldFall));

            AddTransition(jumpState, fallState, new FuncPredicate(() => !isGrounded && !jumpTimer.IsRunning && !dashTimer.IsRunning));
            AddTransition(jumpState, landState, new FuncPredicate(() => isGrounded && !dashTimer.IsRunning && !jumpTimer.IsRunning));

            AddTransition(landState, locomotionState, new FuncPredicate(() => isGrounded && !landStateTimer.IsRunning));
            AddTransition(landState, fallState, new FuncPredicate(() => !isGrounded));
            AddTransition(landState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning || jumpBufferTimer.IsRunning));

            AddTransition(bounceState, fallState, new FuncPredicate(() => !bounceTimer.IsRunning));
            AddTransition(bounceState, landState, new FuncPredicate(() => isGrounded && !bounceTimer.IsRunning));

            AddTransition(fallState, wallSlideState, new FuncPredicate(() => wallJumpChecker.IsTouchingWall && jumpVelocity <= 0f && !wallJumpTimer.IsRunning));
            AddTransition(fallState, diveState, new FuncPredicate(() => diveAttack.IsRunning));
            AddTransition(fallState, landState, new FuncPredicate(() => isGrounded && !dashTimer.IsRunning));

            AddTransition(diveState, wallSlideState, new FuncPredicate(() => wallJumpChecker.IsTouchingWall && !wallJumpTimer.IsRunning && !isGrounded));
            AddTransition(diveState, diveLandState, new FuncPredicate(() => isGrounded));

            AddTransition(diveLandState, locomotionState, new FuncPredicate(() => !diveFlag && !diveLandTimer.IsRunning));
            AddTransition(diveLandState, fallState, new FuncPredicate(() => !isGrounded));
            AddTransition(diveLandState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));

            AddTransition(wallSlideState, wallJumpState, new FuncPredicate(() => !isGrounded && wallJumpTimer.IsRunning));
            AddTransition(wallSlideState, locomotionState, new FuncPredicate(() => isGrounded));
            AddTransition(wallSlideState, fallState, new FuncPredicate(() => wallSlideCancelTimer.IsRunning || (!wallJumpTimer.IsRunning && !wallJumpChecker.IsTouchingWall)));

            AddTransition(wallJumpState, fallState, new FuncPredicate(() => !jumpTimer.IsRunning && !wallJumpTimer.IsRunning));
            AddTransition(wallJumpState, landState, new FuncPredicate(() => isGrounded));

            AddTransition(dashState, locomotionState, new FuncPredicate(() => isGrounded && !dashTimer.IsRunning));
            AddTransition(dashState, dashJumpState, new FuncPredicate(() => jumpTimer.IsRunning && dashTimer.IsRunning));
            AddTransition(dashState, jumpState, new FuncPredicate(() => !isGrounded && !coyoteTimer.IsRunning)); //jumpState for momentum

            AddTransition(dashJumpState, diveState, new FuncPredicate(() => diveAttack.IsRunning));
            AddTransition(dashJumpState, wallSlideState, new FuncPredicate(() => wallJumpChecker.IsTouchingWall && !wallJumpTimer.IsRunning && !isGrounded));
            AddTransition(dashJumpState, landState, new FuncPredicate(() => isGrounded && !jumpTimer.IsRunning));

            AddTransition(attackState, locomotionState, new FuncPredicate(() => !baseAttack.IsRunning));

            AddAnyTransition(locomotionState, new FuncPredicate(() => !canMove)); // || ( groundChecker.IsGrounded && !baseAttack.IsRunning && !jumpTimer.IsRunning && !bounceTimer.IsRunning && !dashTimer.IsRunning && !diveFlag && !diveLandTimer.IsRunning && !landStateTimer.IsRunning)));
            AddAnyTransition(bounceState, new FuncPredicate(() => bounceTimer.IsRunning));
            //set initial state
            stateMachine.SetState(locomotionState);
        }

        void SetupTimers()
        {
            //Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            bounceTimer = new CountdownTimer(bounceDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            landStateTimer = new CountdownTimer(landStateDuration);
            coyoteTimer = new CountdownTimer(coyoteTime);
            jumpBufferTimer = new CountdownTimer(jumpBuffer);
            dashTimer = new CountdownTimer(dashDuration);
            airDashLiftTimer = new CountdownTimer(airDashLiftTime);
            diveLandTimer = new CountdownTimer(diveLandStateDuration);
            diveLandLockoutTimer = new CountdownTimer(diveLandLockout);
            diveLandSoundTimer = new CountdownTimer(diveLandSoundDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);
            wallJumpTimer = new CountdownTimer(wallJumpDuration);
            wallSlideCancelTimer = new CountdownTimer(wallSlideLockout);

            jumpTimer.OnTimerStart += () =>
            {
                if(coyoteTimer.IsRunning)
                {
                    coyoteTimer.Stop();
                }

                if(jumpBufferTimer.IsRunning)
                {
                    jumpBufferTimer.Stop();
                }
                
                jumpVelocity = jumpForce; //whenever timer starts we start out with jumpForce            
            };

            jumpTimer.OnTimerStop += () =>
            {
                jumpCooldownTimer.Start(); //For if we want a cooldown on how long after landing we want to be able to jump again.
            };

            bounceTimer.OnTimerStart += () => jumpVelocity = bounceForce;

            wallJumpTimer.OnTimerStop += () => jumpVelocity = 0f;

            dashTimer.OnTimerStart += () =>
            {
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
                dashCooldownTimer.Start();
            };

            diveLandTimer.OnTimerStart += () =>
            {
                diveLandSoundTimer.Start();
            };

            diveLandTimer.OnTimerStop += () =>
            {
                diveLandSoundTimer.Stop();
            };

            diveLandSoundTimer.OnTimerStop += () =>
            {
                playerSounds.StopSound();
            };

            timers = new List<Timer>(14) { jumpTimer, bounceTimer, jumpCooldownTimer, coyoteTimer, jumpBufferTimer, landStateTimer, wallJumpTimer, wallSlideCancelTimer, dashTimer, airDashLiftTimer, diveLandTimer, diveLandLockoutTimer, diveLandSoundTimer, dashCooldownTimer}; //defining capacity for some optimization;
        }
        //*****************************Jumping (also see HandleJump lower down)*****************************

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
            input.Attack += OnAttack;

            EventManager.StartListening(Events.PAUSED.ToString(), PauseRunAudio);
            EventManager.StartListening(Events.UNPAUSED.ToString(), UnPauseRunAudio);
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
            input.Attack -= OnAttack;

            EventManager.StopListening(Events.PAUSED.ToString(), PauseRunAudio);
            EventManager.StopListening(Events.UNPAUSED.ToString(), UnPauseRunAudio);
        }

        void PauseRunAudio()
        {
            playerSounds.PauseSound();
        }

        void UnPauseRunAudio()
        {
            playerSounds.UnPauseSound();
        }

        public void StopRunSound()
        {
            playingRunSound = false;
            playerSounds.StopRunSound();
        }

        void OnJump(bool performed)
        {
            if(performed && !jumpTimer.IsRunning && !bounceTimer.IsRunning && !jumpCooldownTimer.IsRunning && (isGrounded || coyoteTimer.IsRunning) && !diveLandLockoutTimer.IsRunning)
            {
                jumpTimer.Start();
            } else if(!performed) //We let go of the jump button early, shorthop
            {
                jumpTimer.Stop();
            }

            if (performed && !jumpTimer.IsRunning)
            {
                if(jumpBufferTimer.IsRunning)
                {
                    jumpBufferTimer.Reset();
                } else
                {
                    jumpBufferTimer.Start();
                }
                
            }

            if(performed && !wallJumpTimer.IsRunning && wallJumpChecker.IsTouchingWall && !isGrounded)
            {
                wallJumpTimer.Start();
            }
        }

        void OnDash(bool performed)
        {
            if(!dashTimer.IsRunning)
            {
                if (performed && isGrounded && !dashCooldownTimer.IsRunning && movement.magnitude > 0)
                {
                    dashTimer.Start();
                    dashAttack.StartAttack();
                } 
            }
            
        }

        void OnAttack()
        {
            //ground Attack
            if(isGrounded)
            {
                //Slide attack
                if(movement.magnitude > 0)
                {
                    dashTimer.Start();
                    dashAttack.StartAttack();
                } else
                {
                    baseAttack.StartAttack(); //Start the timer if we're not in cooldown and set the bool to transition our state.
                }
                
            }     
            
            //air Attack
            if(ShouldFall)
            {
                diveAttack.StartAttack();
            }
        }

        //TODO: Have one function for attacks or have the states call the attacks directly.
        public void Attack()
        {
            baseAttack.Attack(); //Called when we enter the Attack State to actually attack.
        }

        public void DashAttack()
        {
            dashAttack.Attack();
        }


        public void StartDive()
        {
            SetDiveFlag(true);
            airDashVelocity = airDashForce;
            airDashLiftTimer.Start();

            transform.LookAt(transform.position + (Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement));
            diveAttack.Attack();
        }

        public void ResetDiveVelocity()
        {
            airDashVelocity = new Vector2(1f,0f);
        }

        public void SetDiveFlag(bool value)
        {
            diveFlag = value;
        }

        public void StartDashJump()
        {
            dashJumpVelocity = dashJumpForce;
        }

        public void ResetDashJumpVelocity()
        {
            dashJumpVelocity = 1f;
        }

        public void StartLandStateTimer()
        {
            if (!landStateTimer.IsRunning)
            {
                landStateTimer.Start();
            }
        }

        public void StopLandStateTimer()
        {
            if (landStateTimer.IsRunning)
            {
                landStateTimer.Stop();
            }
        }

        public void StartDiveLandTimers()
        {
            if(!diveLandTimer.IsRunning)
            {
                diveLandTimer.Start();
            }

            if(!diveLandLockoutTimer.IsRunning)
            {
                diveLandLockoutTimer.Start();
            }
        }

        public void StopDiveLandTimers()
        {
            if (diveLandTimer.IsRunning)
            {
                diveLandTimer.Stop();
            }

            if(diveLandLockoutTimer.IsRunning)
            {
                diveLandLockoutTimer.Stop();
            }
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
            if (canMove)
            {
                movement = new Vector3(input.Direction.x, 0, input.Direction.y);
            }
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

        public void CheckCoyoteTime()
        {
            if(coyoteTimer.IsRunning)
            {
                HandleVerticalMovement();
            }

            if(!isGrounded && !jumpTimer.IsRunning && !coyoteTimer.IsRunning)
            {
                coyoteTimer.Start();
            }

            if(coyoteTimer.IsRunning && (isGrounded) && !jumpTimer.IsRunning)
            {
                coyoteTimer.Stop();
            }
        }

        public void CheckJumpBuffer()
        {
            if(jumpBufferTimer.IsRunning)
            {
                jumpTimer.Start();
                jumpBufferTimer.Stop();
            }
        }

        //Jump has an initial burst of vertical speed, then apply less velocity over time, then let gravity take over.
        public void HandleVerticalMovement()
        {        
            if(!jumpTimer.IsRunning && !bounceTimer.IsRunning)
            {
                if (isGrounded || coyoteTimer.IsRunning)
                {
                    jumpVelocity = 0f;
                }
                else if(airDashLiftTimer.IsRunning && !isGrounded) //beginning part of an airDash where we lift off a little
                {
                    jumpVelocity = airDashLiftForce;
                }
                else //falling
                {
                    jumpVelocity += airDashVelocity.y + Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
                }             
            }

            //Apply the jump velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);

        }

        public void HandleWallSlide()
        {
            lastWallDirection = wallJumpChecker.WallNormal;
            jumpVelocity = 0f;
            rb.velocity = new Vector3(0f, -wallSlideSpeed, 0f);

            if (movement.z < 0 && !wallSlideCancelTimer.IsRunning)
            {
                wallSlideCancelTimer.Start();
            }
        }

        public void HandleWallJump()
        {
            jumpVelocity = wallJumpForce.y * wallJumpTimer.Progress;

            rb.velocity = new Vector3(lastWallDirection.x * wallJumpForce.x, jumpVelocity, lastWallDirection.z * wallJumpForce.x);
        }

        public void BounceJump() //jumping off something
        {
            bounceTimer.Start();
        }

        public void HandleDashJump()
        {
            // If we're on the ground and not jumping, keep jump velocity at 0
            if (!jumpTimer.IsRunning && isGrounded)
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

        }   

        public void HandleDive()
        {
            Vector3 adjustedDirection = transform.forward + (Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement);
            HandleAirMovement(adjustedDirection);
        }

        //*************************Movement*************************************

        public void HandleMovement()
        {
            //Have movement direction be relative to our camera's rotation around the y axis (vector3.up)
            //movement = Vector3.ProjectOnPlane(movement, groundChecker.CurrSlopeNormal);
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            adjustedDirection = Vector3.ProjectOnPlane(adjustedDirection, groundChecker.CurrSlopeNormal);
            
            

            //transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, groundChecker.currSlopeNormal));

            if (adjustedDirection.magnitude > 0)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);

            }
            else
            {
                SmoothSpeed(0);
                StopRunSound();

                //reset horizontal velocity so we can stop on a dime
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }

        void SmoothSpeed(float targetSpeed)
        {
            currSpeed = Mathf.SmoothDamp(currSpeed, targetSpeed, ref velocity, smoothTime);
        }
      
        void HandleRotation(Vector3 adjustedDirection)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.position, transform.position + adjustedDirection);

            if(wallJumpTimer.IsRunning)
            {
                transform.LookAt(transform.position + adjustedDirection); //Have player look where they're going
            } else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(adjustedDirection), rotationSpeed * Time.deltaTime);
            }
        }

        //For if we manually want to make the player look at something (e.g. during a cutscene)
        public void LookAtTarget(Transform target)
        {
            transform.LookAt(target);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        //Move the player
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            if(!isGrounded)
            {
                StopRunSound();

                HandleAirMovement(adjustedDirection);
                return;
            }
           
            //if(groundChecker.ShouldSnapToGround) { Debug.Log("SNAPPING"); }
            //Vector3 velocity = adjustedDirection * moveSpeed * (groundChecker.ShouldSnapToGround ? (-transform.up * 2f).y : 1f) * dashVelocity * Time.fixedDeltaTime;

            Vector3 velocity = adjustedDirection * moveSpeed * dashVelocity * Time.fixedDeltaTime;
            if (currSpeed > 0.8 && dashVelocity <= 1)
            {
                if(!playingRunSound)
                {
                    playingRunSound = true;
                    playerSounds.PlaySound(playerSounds.RunSound, true);
                }
                
            }

            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        void HandleAirMovement(Vector3 adjustedDirection)
        {
            float speedChange = airAcceleration * airDashVelocity.x * Time.deltaTime;

            Vector3 velocity = adjustedDirection * airMaxSpeed * airDashVelocity.x * dashJumpVelocity * Time.fixedDeltaTime;
            velocity.x = Mathf.MoveTowards(rb.velocity.x, velocity.x, speedChange);
            velocity.z = Mathf.MoveTowards(rb.velocity.z, velocity.z, speedChange);

            //rb.velocity += new Vector3(velocity.x, 0f, velocity.z);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        public void HaltVerticalAirMomentum() //For airdashes since we don't want to rise or fall
        {
            if(!isGrounded)
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

        public void LockMovement()
        {
            canMove = false;
        }

        public void UnlockMovement()
        {
            canMove = true;
        }

    }
}

