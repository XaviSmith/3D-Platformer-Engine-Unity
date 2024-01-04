using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// For detecting a player near to us e.g. enemy AI.
/// </summary>
public class PlayerDetector : MonoBehaviour
{
    [SerializeField] bool drawGizmos;
    [SerializeField] float detectionAngle = 60f;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] float innerDetectionRadius = 3f; //For if we want to detect them when they're within a certain proximity
    [SerializeField] float detectionCooldown = 1f; //We check if we can still detect the player every x seconds.
    [SerializeField] float attackRange = 2f;

    public Transform Player { get; private set; }

    CountdownTimer detectionTimer;

    IDetectionStrategy detectionStrategy;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform; //Find isn't great, but keeping this generic and separated from Platformer namespace for github version.
    }

    void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown); //No need for an OnStop or Reset as this gets reset in detectionStrategy.Execute() whenever we call Timer.Start()
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
    }

    private void Update()
    {
        detectionTimer.Tick(Time.deltaTime);
    }

    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
    }

    public bool CanAttackPlayer()
    {
        Vector3 directionToPlayer = Player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    public void SetDetectionStrategy(IDetectionStrategy detectionStrategy)
    {
        this.detectionStrategy = detectionStrategy;
    }


    //For Cone Detection
    public void OnDrawGizmos()
    {
        if(drawGizmos)
        {
            Gizmos.color = Color.red;

            //Draw the detection radii
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

            //calculate cone directions
            Vector3 coneLeftLine = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
            Vector3 coneRightLine = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

            Gizmos.color = Color.blue;

            //Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + coneLeftLine);
            Gizmos.DrawLine(transform.position, transform.position + coneRightLine);
        }
        
    }
}
