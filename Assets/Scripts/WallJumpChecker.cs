using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// Handles Wall Sliding 
/// </summary>
public class WallJumpChecker : MonoBehaviour
{
    [SerializeField] LayerMask wallLayers;
    public bool IsTouchingWall { get; set; }
    [SerializeField] Transform origin;
    [SerializeField] float wallCastDistance = 0.08f; //How far to spherecast to detect ground
    [SerializeField] Vector3 offset;
    [SerializeField] float checkRadius;
    public bool IsWallSliding { get; set; }

    public bool wallSlideSpeed; //move to playercontroller

    bool CheckWallCast()
    {
        /*Collider[] hits = Physics.OverlapSphere(transform.forward + offset, checkRadius, wallLayer);

        if(hits.Length > 0)
        {
            return true;
        }

        return false;
        */
        return Physics.SphereCast(origin.position + offset, checkRadius, origin.forward, out _, wallCastDistance, wallLayers);
    }

    void Update()
    {
        IsTouchingWall = CheckWallCast();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin.position + offset + origin.forward * wallCastDistance + (origin.forward * checkRadius), origin.position + offset + (origin.forward * checkRadius));

        Gizmos.color = IsTouchingWall ? Color.green : Color.yellow;

        //Gizmos.DrawWireSphere(origin.position + offset, checkRadius);

        Gizmos.DrawWireSphere(origin.position + offset + (origin.forward * wallCastDistance), checkRadius);
    }
}
