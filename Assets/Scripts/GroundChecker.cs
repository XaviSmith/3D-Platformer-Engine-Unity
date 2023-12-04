using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float detectionRadius = 0.08f; //How far to spherecast to detect ground
        [SerializeField] float groundCastDistance = 0.08f; //How far to spherecast to detect ground
        [SerializeField] Vector3 offset = new Vector3(0f, 0.1f, 0f); //Consider just using a transform for this instead if the calculations start causing overhead
        [SerializeField] Vector3 slopeOffset = new Vector3(0f, 0.1f, 0f);
        [SerializeField] LayerMask groundLayers;

        [Header("Slope Settings")]
        [SerializeField] [Range(0f,360f)] float minSlopeAngle = 10f; //What is the smallest angle we can consider a slope
        [SerializeField] [Range(0f, 360f)] float maxSlopeAngle = 45f; //What is the smallest angle we can consider a slope
        [SerializeField] float slopeGroundDistance = 0.08f;

        float currSlopeAngle;      
        RaycastHit slopeRaycast;

        public bool IsGrounded { get; set; }
        public bool IsOnSlope { get; set; }
        public Vector3 CurrSlopeNormal { get; set; }

        [Header("Debug Settings")]
        [SerializeField] bool drawGroundCheck = true;
        [SerializeField] bool drawSlopeCheck = true;

        //Note: this works cuz we can do Quaternion * Vector3, but not Vector3 * Quaternion
        Vector3 CurrOffset { get { return transform.rotation * offset; } set { CurrOffset = value; } }

        bool CheckSphereCast()
        {
            //There is no transform.down so we use -transform.up
            return Physics.SphereCast(transform.position - CurrOffset, detectionRadius, -transform.up, out _, groundCastDistance, groundLayers);
        }

        //Shoot a ray under the player and get the normal to get the angle of our slope
        bool CheckSlope()
        {
            //We don't rotate the slope check to match player orientation otherwise it'd be useless
            if (Physics.Raycast(transform.position - slopeOffset, Vector3.down, out slopeRaycast, slopeGroundDistance))
            {
                
                currSlopeAngle = Vector3.Angle(slopeRaycast.normal, Vector3.up);
                if (currSlopeAngle >= minSlopeAngle && currSlopeAngle <= maxSlopeAngle) {
                    CurrSlopeNormal = slopeRaycast.normal;
                    return true;
                }
                else
                {
                    CurrSlopeNormal = Vector3.zero;
                    return false;
                }
            }
            else
            {
                currSlopeAngle = 0f;
                CurrSlopeNormal = Vector3.zero;
                return false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            IsGrounded = CheckSphereCast();
            IsOnSlope = CheckSlope();
        }

        //For Debugging the spherecast and slopeCast. SphereCast casts up to wherever the blue line is with volume as the sphere.
        //SlopeCast is the yellow line
        private void OnDrawGizmos()
        {
            if(drawSlopeCheck)
            {
                //Slope raycast
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position - slopeOffset, transform.position - slopeOffset + (Vector3.down * slopeGroundDistance));
            }
            
            if(drawGroundCheck)
            {
                //Ground SphereCast line
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position - CurrOffset + (-transform.up * groundCastDistance) + (-transform.up * detectionRadius), transform.position - (transform.rotation * offset) + (-transform.up * detectionRadius));
                
                //Ground Spherecast
                Gizmos.color = CheckSphereCast() ? Color.green : Color.red;

                Gizmos.DrawWireSphere(transform.position - CurrOffset + (-transform.up * groundCastDistance), detectionRadius);
            }

            //framesSinceLastGrounded line
            Gizmos.color = Color.black;
            //Gizmos.DrawLine(transform.position - offset, transform.position - offset + Vector3.down * snapCastDistance);
        }
    }
}

