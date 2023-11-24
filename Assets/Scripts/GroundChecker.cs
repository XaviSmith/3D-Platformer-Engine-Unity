using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float detectionRadius = 0.08f; //How far to spherecast to detect ground
        [SerializeField] float groundDistance = 0.08f; //How far to spherecast to detect ground
        [SerializeField] Vector3 offset = new Vector3(0f, 0.1f, 0f);
        [SerializeField] LayerMask groundLayers;

        [Header("Slope Settings")]
        [SerializeField] [Range(0f,360f)] float minSlopeAngle = 10f; //What is the smallest angle we can consider a slope
        [SerializeField] [Range(0f, 360f)] float maxSlopeAngle = 0f; //What is the smallest angle we can consider a slope
        [SerializeField] float slopeGroundDistance = 0.08f;

        float currSlopeAngle = 0f;
        RaycastHit slopeRaycast;

        public bool IsGrounded { get; set; }

        bool CheckSphereCast()
        {
            return Physics.SphereCast(transform.position - offset, detectionRadius, Vector3.down, out _, groundDistance, groundLayers);
        }

        //Shoot a ray under the player
        bool CheckSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeRaycast, slopeGroundDistance))
            {
                currSlopeAngle = Vector3.Angle(slopeRaycast.normal, Vector3.up);
                if (currSlopeAngle >= minSlopeAngle) { return true; }
                else return false;
            }
            else return false;
        }

        // Update is called once per frame
        void Update()
        {
            IsGrounded = CheckSphereCast();
        }

        //For Debugging the spherecast. Casts up to wherever the blue line is with volume as the sphere.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - offset + Vector3.down * groundDistance + (Vector3.down * detectionRadius), transform.position - offset + (Vector3.down * detectionRadius));
            if (CheckSphereCast())
            {
                Gizmos.color = Color.green;
            } else
            {
                Gizmos.color = Color.red;
            }
     
            Gizmos.DrawWireSphere(transform.position - offset + (Vector3.down * groundDistance), detectionRadius);
        }
    }
}

