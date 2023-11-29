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
        [SerializeField, Range(1f, 200f)] float distance;
        [SerializeField, Min(0f)] float focusRadius = 1f; //How much can the player move before we move the cam with them
        [SerializeField, Range(0f, 1f)] float focusSmoothing = 0.5f;
        [SerializeField, Range(0f, 360f)] float rotationSpeed = 90f;

        Vector3 focusPoint; //Where we're looking
        [SerializeField] Vector2 camOrientation = new Vector2(45f, 0f); //camera orientation. x is vertical (0 = straight, 90 = straight up), y is horizontal
        const float DeadZone = 0.001f;

        //mouse input
        bool isRMBPressed; //right mouse button;
        bool cameraMovementLock; //used for a frame when we disable movement

        void Awake()
        {
            focusPoint = target.position;
        }

        //*****HANDLE MOUSE INPUT****************************
        void OnEnable()
        {
            //set our InputReader functions
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        void OnDisable()
        {
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
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

            //reset mouse pos to center screen
            //freeLookVcam.m_XAxis.m_InputAxisValue = 0f;
            //freeLookVcam.m_YAxis.m_InputAxisValue = 0f;


            StartCoroutine(DisableMouseForFrame()); //disable mouse for 1 frame to prevent weirdness
        }

        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;

        }
        //**************************************************
        // Late update so it updates after player movement
        void LateUpdate()
        {
            UpdateFocusPoint();
            ManualRotation();
            Quaternion lookRotation = Quaternion.Euler(camOrientation);
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = focusPoint - lookDirection * distance; //offset the camera by look direciton and distance
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        void UpdateFocusPoint()
        {
            Vector3 targetPoint = target.position;

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

            //Mouse controls
            if(isRMBPressed)
            {
                camDirection = new Vector2(input.MouseCamDirection.y, -input.MouseCamDirection.x);
            } else //other controls
            {
                camDirection = new Vector2(input.CamDirection.y, -input.CamDirection.x);
            }

            
            if(camDirection.x < -DeadZone || camDirection.x > DeadZone || camDirection.y < -DeadZone || camDirection.y > DeadZone)
            {
                camOrientation += rotationSpeed * Time.unscaledDeltaTime * camDirection;
            }

        }
    }
}

