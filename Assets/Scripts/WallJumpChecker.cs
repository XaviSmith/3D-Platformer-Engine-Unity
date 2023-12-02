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
    [SerializeField] Collider _collider;
    [SerializeField] public Vector3 WallNormal;
    public bool IsWallSliding { get; set; }

    bool CheckWallCast()
    {
        RaycastHit hit;
        /*
        Vector3 castCenter = origin.position + offset + (origin.forward * wallCastDistance);
        //Cast the sphere towards the front of the player 
        Collider[] hits = Physics.OverlapSphere(castCenter, checkRadius, wallLayers);

        if(hits.Length > 0)
        {
            wallDirection = hits[0].ClosestPoint(castCenter);
            wallDirection.y = castCenter.y;
            Debug.Log("WALL DIRECTION: " + wallDirection);
            return true;
        }

        wallDirection = Vector3.zero;
        return false;
        */
        bool hitWall = Physics.SphereCast(origin.position + offset, checkRadius, origin.forward, out hit, wallCastDistance, wallLayers);
        WallNormal = hitWall ? hit.normal.normalized : Vector3.zero;
        return hitWall;
    }

    void Update()
    {
        IsTouchingWall = CheckWallCast();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin.position + offset + origin.forward * wallCastDistance + (origin.forward * checkRadius), origin.position + offset + (origin.forward * checkRadius));
        if(WallNormal != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin.position + offset + (origin.forward * wallCastDistance), origin.position + offset + (origin.forward * wallCastDistance) - WallNormal);
        }
        
        Gizmos.color = IsTouchingWall ? Color.green : Color.yellow;

        //Gizmos.DrawWireSphere(origin.position + offset + (origin.forward * wallCastDistance), checkRadius);

        Gizmos.DrawWireSphere(origin.position + offset + (origin.forward * wallCastDistance), checkRadius);
    }
}
