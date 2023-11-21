using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ConeDetectionStrategy : IDetectionStrategy
{
    readonly float detectionAngle;
    readonly float detectionRadius;
    readonly float innerDetectionRadius;

    public ConeDetectionStrategy(float _detectionAngle, float _detectionRadius, float _innerDetectionRadius)
    {
        this.detectionAngle = _detectionAngle;
        this.detectionRadius = _detectionRadius;
        this.innerDetectionRadius = _innerDetectionRadius;
    }

    public bool Execute(Transform target, Transform detector, CountdownTimer cooldownTimer)
    {
        if (cooldownTimer.IsRunning)
        {
            return false;
        }

        Vector3 directionToTarget = target.position - detector.position;
        float angleToTarget = Vector3.Angle(directionToTarget, detector.forward);

        //if the player isn't within our detection angle + outer radius and isn't within the inner detection radius we didn't find anything
        if ((!(angleToTarget < detectionAngle / 2f) || !(directionToTarget.magnitude < detectionRadius))
            && !(directionToTarget.magnitude < innerDetectionRadius))
        {
            return false;
        }

        cooldownTimer.Start();
        return true;
    }

    //DEBUG CONE - Put this in the MonoBehaviour's OnGizmos call! Can't do it here because this doesn't inherit from a monobehaviour
    /*
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Draw the detection radii
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

        //calculate cone directions
        Vector3 coneLeftLine = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
        Vector3 coneRightLine = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

        //Draw lines to represent the cone
        Gizmos.DrawLine(transform.position, transform.position + coneLeftLine);
        Gizmos.DrawLine(transform.position, transform.position + coneRightLine);
    }
    */
}
