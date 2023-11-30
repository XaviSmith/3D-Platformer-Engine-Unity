using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based on tutorial here: https://catlikecoding.com/unity/tutorials/movement/orbit-camera/ 

namespace Platformer
{
    [RequireComponent(typeof(Camera))]
    public class OrbitCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] InputReader input;
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;
        [SerializeField, Range(1f, 200f)] float distance;
        [SerializeField, Min(0f)] float focusRadius = 1f; //How much can the player move before we move the cam with them
        [SerializeField, Range(0f, 1f)] float focusSmoothing = 0.5f;
        [SerializeField, Range(0f, 360f)] float rotationSpeed = 90f;
        [SerializeField, Min(0f)] float realignDelay = 5f; //how long before we start manually delaying the camera
        [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45; // what angle do we automatically realign at full speed

        Vector3 focusPoint; //Where we're looking
        Vector3 prevFocusPoint;

        [SerializeField] Vector2 camOrientation = new Vector2(45f, 0f); //camera orientation. x is vertical (0 = straight, 90 = straight up), y is horizontal
        [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f; 
        [SerializeField, Range(-89f, 89f)] float maxVerticalAngle = 60f;
        const float DeadZone = 0.001f;

        bool isRotatingCamera = false;
        float lastManualRotationTime;

        //mouse input
        bool isRMBPressed; //right mouse button;
        bool cameraMovementLock; //used for a frame when we disable movement    

        void Awake()
        {
            focusPoint = target.position + offset;
            transform.localRotation = Quaternion.Euler(camOrientation);
            ClampAngles();
        }

        //Editor only function
        void OnValidate()
        {
            if(maxVerticalAngle < minVerticalAngle)
            {
                maxVerticalAngle = minVerticalAngle;
            }
        }

        //*****HANDLE MOUSE INPUT****************************
        void OnEnable()
        {
            //set our InputReader functions
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
            input.ResetCamera += ResetCam;
        }

        void OnDisable()
        {
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
            input.ResetCamera -= ResetCam;
        }

        void OnEnableMouseControlCamera()
        {
            isRMBPressed = true;

            //lock cursor to center of screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame()); //disable mouse for 1 frame to prevent weirdness
        }

        void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;

            //lock cursor to center of screen and hide it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            StartCoroutine(DisableMouseForFrame()); //disable mouse for 1 frame to prevent weirdness
        }

        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;

        }
        //**************************************************

        //Reset Camera to behind the Player.
        void ResetCam()
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(transform.eulerAngles.x, target.transform.eulerAngles.y, transform.eulerAngles.z));
            camOrientation.y = target.transform.eulerAngles.y;
        }

        // Late update so it updates after player movement
        void LateUpdate()
        {
            UpdateFocusPoint();
            ManualRotation();

            Quaternion lookRotation;

            if(isRotatingCamera || AutomaticRotation())
            {
                ClampAngles();
                lookRotation = Quaternion.Euler(camOrientation);
            } else
            {
                lookRotation = transform.rotation;
            }

            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = focusPoint - lookDirection * distance; //offset the camera by look direciton and distance
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        void UpdateFocusPoint()
        {
            prevFocusPoint = focusPoint;
            Vector3 targetPoint = target.position + offset;

            //If we have a focus radius and we're outside of it, lerp back into it
            if (focusRadius > 0f)
            {
                float targetOffset = Vector3.Distance(targetPoint, focusPoint);
                float t = 1f; //for the lerp

                //TODO See if removing the T stuff and reverting back to t being focusRadius/targetOffset makes any difference
                if (targetOffset > 0.01f && focusSmoothing > 0f)
                {
                    t = Mathf.Pow(1f - focusSmoothing, Time.unscaledDeltaTime); //Unscaled because camera should always move at same speed
                }

                if (targetOffset > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / targetOffset);
                }

                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);

            }
            else
            {
                focusPoint = target.position;
            }

        }


        void ManualRotation()
        {
            Vector2 camDirection = Vector2.zero;
  
            if(isRMBPressed) //Mouse controls
            {
                camDirection = new Vector2(input.MouseCamDirection.y, -input.MouseCamDirection.x);
            } else //other controls
            {
                camDirection = new Vector2(input.CamDirection.y, -input.CamDirection.x);
            }
            
            if(camDirection.x < -DeadZone || camDirection.x > DeadZone || camDirection.y < -DeadZone || camDirection.y > DeadZone)
            {
                isRotatingCamera = true;
                camOrientation += rotationSpeed * Time.unscaledDeltaTime * camDirection;
                lastManualRotationTime = Time.unscaledTime;
            } else
            {
                isRotatingCamera = false;
            }

        }

        //Keep cam angle within range
        void ClampAngles()
        {
            camOrientation.x = Mathf.Clamp(camOrientation.x, minVerticalAngle, maxVerticalAngle);

            //horizontal rotation doesn't loop between 0-360 by default, so manually do so.
            if(camOrientation.y < 0f)
            {
                camOrientation.y += 360f; 
            }

            if(camOrientation.y >= 360f)
            {
                camOrientation.y -= 360f;
            }
        }

        bool AutomaticRotation()
        {
            if(Time.unscaledTime - lastManualRotationTime < realignDelay) { return false; }

            Vector2 movementDelta = new Vector2(focusPoint.x - prevFocusPoint.x, focusPoint.z - prevFocusPoint.z); //How much did we move from our last established focus
            float movementDeltaSqr = movementDelta.sqrMagnitude; //sqrMagnitude is faster to get than magnitude since we don't have to sqrt anything so we just cache it.

            if (movementDeltaSqr < 0.001f) 
            {
                return false;
            }

            //Realign to default angle.
            float headingAngle = GetCurrentHorizontalAngle(movementDelta / Mathf.Sqrt(movementDeltaSqr)); //pass in movementDelta's unit vector
            float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);

            //Smooth the rotation according to how much we have left to rotate
            float angleDelta = Mathf.Abs(Mathf.DeltaAngle(camOrientation.y, headingAngle)); //How much rotation left

            if(angleDelta < alignSmoothRange)
            {
                rotationChange = rotationChange * angleDelta/alignSmoothRange;
            } else if (180f - angleDelta < alignSmoothRange) //case where we're moving towards the camera
            {
                rotationChange = rotationChange * (180f - angleDelta)/alignSmoothRange;
            }
            //

            camOrientation.y = Mathf.MoveTowardsAngle(camOrientation.y, headingAngle, rotationChange);

            return true;
        }

        //Gets the horizontal angle of the vector passed in
        float GetCurrentHorizontalAngle(Vector2 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x < 0f ? 360f - angle : angle; //if x is negative we got the counterclockwise angle

        }
    }  
}

