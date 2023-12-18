using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

/// <summary>
/// Default State in our state machine
/// Tailored for Player right now. See LocomotionState and JumpState as an example.
/// </summary>
public abstract class BaseState : IState
{
    protected readonly PlayerController player;
    protected readonly Animator animator;
    protected readonly PlayerParticles particles;

    //Animation Hashes
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int DiveHash = Animator.StringToHash("Dive");
    protected static readonly int DiveLandHash = Animator.StringToHash("DiveLand");
    protected static readonly int FallHash = Animator.StringToHash("Fall");
    protected static readonly int WallSlideHash = Animator.StringToHash("WallStick");
    protected static readonly int WallJumpHash = Animator.StringToHash("Jump");
    protected static readonly int DashHash = Animator.StringToHash("Dash");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");

    protected const float CROSSFADEDURATION = 0.1f;

    protected BaseState(PlayerController _player, Animator _animator, PlayerParticles _particles = null)
    {
        this.player = _player;
        this.animator = _animator;
        this.particles = _particles;
    }

    public virtual void OnEnter()
    {
        //Debug.Log(this.GetType().Name + ".OnEnter");
    }

    public virtual void Update()
    {
        //Not Used
    }

    public virtual void FixedUpdate()
    {
        //Not Used
    }

    public virtual void OnExit()
    {
        //Debug.Log(this.GetType().Name + ".OnExit");
    }

    
}