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
    [SerializeField] Vector3 hitboxSize = Vector3.one;
    [SerializeField] Vector3 offset;
    [SerializeField] float checkRadius;
    [SerializeField] Collider _collider;
    [SerializeField] public Vector3 WallNormal;
    public bool IsWallSliding { get; set; }

    [Header("Debug Settings")]
    [SerializeField] bool drawWallCheck = true;
    [SerializeField] bool drawWallNormal = true;

    bool CheckWallCast()
    {
        RaycastHit hit;

        //bool hitWall = Physics.SphereCast(origin.position + offset, checkRadius, origin.forward, out hit, wallCastDistance, wallLayers); //spherecast version
        bool hitWall = Physics.BoxCast(origin.position, hitboxSize, origin.forward, out hit, Quaternion.identity, wallCastDistance, wallLayers); //switched to boxcast for consistency
        WallNormal = hitWall ? hit.normal.normalized : Vector3.zero;
        return hitWall;
    }

    void Update()
    {
        IsTouchingWall = CheckWallCast();
    }

    private void OnDrawGizmos()
    {
        if(drawWallCheck)
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(origin.position + offset + origin.forward * wallCastDistance + (origin.forward * checkRadius), origin.position + offset + (origin.forward * checkRadius)); //spherecast line

            if (drawWallNormal && WallNormal != Vector3.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(origin.position + offset + (origin.forward * wallCastDistance), origin.position + offset + (origin.forward * wallCastDistance) - WallNormal);
            }

            Gizmos.color = IsTouchingWall ? Color.green : Color.yellow;

            //Gizmos.DrawWireSphere(origin.position + offset + (origin.forward * wallCastDistance), checkRadius);

            Gizmos.DrawWireCube(origin.position + offset + (origin.forward * wallCastDistance), new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2));
        }
        
    }
}
