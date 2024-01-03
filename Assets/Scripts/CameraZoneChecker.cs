using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// For moving the camera to specific spots when we enter CameraZones
    /// Best to have them on their own layers for efficiency
    /// </summary>
    public class CameraZoneChecker : MonoBehaviour
    {
        [SerializeField] OrbitCamera currCamera = null;
        Vector2 currOffset = Vector2.zero;

        void OnEnable()
        {
            if(currCamera == null && GameManager.Instance)
            {
                currCamera = GameManager.Instance.currCamera;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            try
            {
                CameraZone zone = other.GetComponent<CameraZone>();
                currCamera.SetCameraZoneOrientation(zone.desiredOffset, zone.lerpTime);
                currOffset = zone.desiredOffset;

            }
            catch
            {
                Debug.LogWarning("COULD NOT GET CAMERAZONE, try having these be on the cameraZone layers!");
            }
            
            
        }


    }
}

