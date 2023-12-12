using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
///  See https://www.youtube.com/watch?v=--_CH5DYz0M 
///  Class to read Inputs from Settings/PlayerInputActions.inputActions and its generated.cs (which is where PlayerInputActions.IPlayerActions comes from).
///  Used by PlayerController and CameraController
/// </summary>

namespace Platformer
{
    
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {

        //Actions to call for each input in PlayerInputActions
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<Vector2> MouseLook = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction ResetCamera = delegate { };
        public event UnityAction Pause = delegate { };

        PlayerInputActions inputActions;
       
        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();  //basically Input.GetAxis();
        public Vector3 CamDirection => inputActions.Player.Look.ReadValue<Vector2>();
        public Vector3 MouseCamDirection => inputActions.Player.MouseLook.ReadValue<Vector2>();

        //instantiate playerInputActions and set the callbacks
        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }

        //Called in PlayerController Start once everything is ready
        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnMouseLook(InputAction.CallbackContext context)
        {
            MouseLook.Invoke(context.ReadValue<Vector2>());
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnFire(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
            {
                Attack.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch(context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;

                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }



        public void OnRun(InputAction.CallbackContext context)
        {
            //Not Used
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
            }
        }

        public void OnResetCamera(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
            {
                ResetCamera.Invoke();
            }
            
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
            {
                Pause.Invoke();
            }
            
        }
    }
}

