using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Platformer
{
    //Right click to move camera
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] InputReader input;
        [SerializeField] CinemachineFreeLook freeLookVcam;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 300f)] float speedMultiplier = 1f;

        bool isRMBPressed; //right mouse button;
        bool cameraMovementLock; //used for a frame when we disable movement

        void OnEnable()
        {
            //set our InputReader functions
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        void OnDisable()
        {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if(cameraMovementLock)
            {
                return;
            } else if(isDeviceMouse && !isRMBPressed)
            {
                return;
            } else
            {
                float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

                //set camera axis values
                freeLookVcam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier; //mouse uses fixedDeltaTime, everything else uses Time.deltaTime
                freeLookVcam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
            }
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
            freeLookVcam.m_XAxis.m_InputAxisValue = 0f;
            freeLookVcam.m_YAxis.m_InputAxisValue = 0f;


            StartCoroutine(DisableMouseForFrame()); //disable mouse for 1 frame to prevent weirdness
        }

        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;

        }
    }
}

