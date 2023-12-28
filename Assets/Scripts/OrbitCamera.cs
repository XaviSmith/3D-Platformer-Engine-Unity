using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

//Extended from tutorial here: https://catlikecoding.com/unity/tutorials/movement/orbit-camera/ 

namespace Platformer
{
    [RequireComponent(typeof(Camera))]
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] bool locked;
        [SerializeField] bool checkCollisions;
        [SerializeField] float collisionTimeout = 0.3f; //how long must something block us before we readjust
        float blockFrames = 0f;
        [SerializeField] LayerMask obstructionMask = -1;
        [Header("References")]
        [SerializeField] InputReader input;
        [SerializeField, Range(0f, 1f)] float mouseSensitivity = 0.15f;
        [SerializeField] Transform target;
        Vector3 targetPosition;
        [Header("Settings")]
        [SerializeField] bool invertXAxis;
        [SerializeField] bool invertYAxis;
        public void InvertXAxis(bool val) => invertXAxis = val;
        public void InvertYAxis(bool val) => invertYAxis = val;

        [SerializeField] float realignTime = 0.2f; //how long do we take to realign the camera for auto-aligns
        [SerializeField] Vector3 offset;
        [SerializeField, Range(1f, 200f)] float distance;
        [SerializeField, Min(0f)] float focusRadius = 1f; //How much can the player move before we move the cam with them
        [SerializeField, Range(0f, 1f)] float focusSmoothing = 0.5f;
        [SerializeField, Range(0f, 2f)] float autoRotationSensitivity = 0.45f;
        [SerializeField, Range(0f, 360f)] float rotationSpeed = 90f;
        [SerializeField, Min(0f)] float realignDelay = 5f; //how long before we start manually delaying the camera
        [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45; // what angle do we automatically realign at full speed
        
        Vector3 focusPoint; //Where we're looking
        Vector3 prevFocusPoint;

        [SerializeField] Vector2 camOrientation = new Vector2(45f, 0f); //camera orientation. x is vertical (0 = straight, 90 = straight up), y is horizontal
        [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f; 
        [SerializeField, Range(-89f, 89f)] float maxVerticalAngle = 60f;
        const float DeadZone = 0.001f;
        Vector2 defaultOrientation = new Vector2();

        Camera _camera;
        
        bool isRotatingCamera = false;
        bool isResetting = false;
        float lastManualRotationTime;

        CountdownTimer realignTimer;

        //mouse input
        bool isRMBPressed; //right mouse button;
        bool cameraMovementLock; //used for a frame when we disable movement    

        //HalfExtends for the boxcast to detect collisions 
        Vector3 CameraHalfExtends {
            get {
                Vector3 halfExtends;
                halfExtends.y = _camera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * _camera.fieldOfView);
                halfExtends.x = halfExtends.y * _camera.aspect;
                halfExtends.z = 0f;
                return halfExtends;
            }
        } 

        public void LockCamera()
        {
            locked = true;
        }

        public void UnlockCamera()
        {
            locked = false;
        }

        void Awake()
        {
            _camera = GetComponent<Camera>();
            targetPosition = target.position + offset;
            focusPoint = targetPosition;
            transform.localRotation = Quaternion.Euler(camOrientation);
            ClampAngles();
            defaultOrientation = camOrientation;
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
            input.ResetCamera += OnResetCamera;

            if(GameManager.Instance)
            {
                GameManager.Instance.currCamera = this;
            }
            
        }

        void OnDisable()
        {
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
            input.ResetCamera -= OnResetCamera;
        }

        void Start()
        {
            realignTimer = new CountdownTimer(realignTime);
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

        void OnResetCamera()
        {           
            if (!realignTimer.IsRunning)
            {
                isResetting = true;
                realignTimer.Start();
                ResetCam();
            }
            
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
            if(!realignTimer.IsRunning)
            {
                isResetting = false;
                return;
            }

            Quaternion currRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(defaultOrientation.x, target.transform.eulerAngles.y, transform.rotation.z), 1 - realignTimer.Progress);
            transform.SetPositionAndRotation(transform.position, currRotation);
            camOrientation.x = defaultOrientation.x;
            camOrientation.y = target.transform.eulerAngles.y;
        }

        public void ResetCamOverride()
        {
            OnResetCamera();
        }

        // Late update so it updates after player movement
        void LateUpdate()
        {
            if(!locked)
            {
                realignTimer.Tick(Time.unscaledDeltaTime);
                targetPosition = target.position + offset;
                UpdateFocusPoint();
                if (isResetting)
                {
                    ResetCam();
                }
                else
                {
                    ManualRotation();
                }


                Quaternion lookRotation;

                if (isRotatingCamera || AutomaticRotation())
                {
                    ClampAngles();
                    lookRotation = Quaternion.Euler(camOrientation);
                }
                else
                {
                    lookRotation = transform.rotation;
                }

                Vector3 lookDirection = lookRotation * Vector3.forward;
                Vector3 lookPosition = focusPoint - lookDirection * distance; //offset the camera by look direciton and distance

                // For if we want to make sure the camera never clips Geometry. Try raising the offset higher if this acts buggy on slopes
                if (checkCollisions)
                {
                    //ideal focus point
                    Vector3 rectOffset = lookDirection * _camera.nearClipPlane;
                    Vector3 rectPosition = lookPosition + rectOffset;
                    Vector3 castLine = rectPosition - targetPosition;
                    float castDistance = castLine.magnitude;
                    Vector3 castDirection = castLine / castDistance;

                    //Boxcast to prevent clipping through geometry. If we hit something pull camera in front if it
                    if (Physics.BoxCast(targetPosition, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
                    {
                        blockFrames += Time.deltaTime; //we use delta time here because we only care about blocking if the game is unpaused.
                        if (blockFrames >= collisionTimeout)
                        {
                            float t = Mathf.Pow(1f - focusSmoothing, Time.unscaledDeltaTime); //Unscaled because camera should always move at same speed
                            rectPosition = targetPosition + castDirection * hit.distance;
                            lookPosition = rectPosition - rectOffset;
                            Vector3 currPos = Vector3.Lerp(transform.position, lookPosition, t);
                            lookPosition = currPos;
                        }

                    }
                    else
                    {
                        blockFrames = 0f;
                    }

                }

                //*******

                transform.SetPositionAndRotation(lookPosition, lookRotation);
            }
        }

        void UpdateFocusPoint()
        {
            prevFocusPoint = focusPoint;
            Vector3 targetPoint = targetPosition + offset;

            //If we have a focus radius and we're outside of it, lerp back into it
            if (focusRadius > 0f)
            {
                float targetOffset = Vector3.Distance(targetPoint, focusPoint);
                float t = 1f; //for the lerp

                //TODO Look into the t stuff and how big of a difference reverting back to t being focusRadius/targetOffset makes
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
                focusPoint = targetPosition;
            }

        }


        void ManualRotation()
        {
            Vector2 camDirection = Vector2.zero;
  
            if(isRMBPressed) //Mouse controls
            {
                camDirection = new Vector2(input.MouseCamDirection.y, input.MouseCamDirection.x) * mouseSensitivity;
            } else //other controls
            {
                camDirection = new Vector2(input.CamDirection.y, input.CamDirection.x);
            }

            if (invertXAxis) { camDirection.y = -camDirection.y; }
            if (invertYAxis) { camDirection.x = -camDirection.x; }

            if (camDirection.x < -DeadZone || camDirection.x > DeadZone || camDirection.y < -DeadZone || camDirection.y > DeadZone)
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
            float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr) * autoRotationSensitivity;

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

