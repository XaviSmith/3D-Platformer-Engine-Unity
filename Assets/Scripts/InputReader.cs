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
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };

        PlayerInputActions inputActions;
       
        public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();  //basically Input.GetAxis();

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

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnFire(InputAction.CallbackContext context)
        {
            //Not Used
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
    }
}

