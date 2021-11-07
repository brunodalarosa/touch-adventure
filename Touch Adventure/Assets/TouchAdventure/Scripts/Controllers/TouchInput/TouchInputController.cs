using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    [RequireComponent(typeof(Rect))]
    public class TouchInputController : MonoBehaviour
    {
        [field: Header("Input")]
        [field: SerializeField] private InputActionReference TouchInputAction { get; set; }
        [field: SerializeField] private InputActionReference TouchMoveAction { get; set; }
    
        private Dictionary<int, TouchInputHandler> InputHandlers { get; set; }
        private bool InputEnabled { get; set; }
        
        private Camera MainCamera { get; set; }
        private ITouchInputListener Listener { get; set; }
        
        private Dictionary<Guid, TouchCommand> ActiveTouchCommands { get; set; }

        public void Init(Camera mainCamera, ITouchInputListener listener)
        {
            MainCamera = mainCamera;
            Listener = listener;
            
            InputHandlers = new Dictionary<int, TouchInputHandler>();
            
            TouchInputAction.action.performed += OnTouchInput;
            TouchMoveAction.action.performed += OnTouchInput;

            ActiveTouchCommands = new Dictionary<Guid, TouchCommand>();
            InputEnabled = true;

        }

        private void Update()
        {
            if (ActiveTouchCommands.Count == 0) return;

            var touchCommandsToRemove = new List<Guid>();

            foreach (var kvp in ActiveTouchCommands)
            {
                var activeTouchCommand = kvp.Value;
                
                activeTouchCommand.PrepareWorldPosition(MainCamera);
                Listener.OnTouch(activeTouchCommand);

                if (activeTouchCommand.FinishingInputCommand)
                {
                    touchCommandsToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in touchCommandsToRemove)
            {
                ActiveTouchCommands.Remove(key);
            }
        }

        private void OnTouchInput(InputAction.CallbackContext callbackContext)
        {
            if (InputEnabled == false) return;
            
            // Se não é touch, cancela a ação
            if (!(callbackContext.control.device is Pointer)) return;

            var inputState = TouchInputState.GetByCallbackContext(callbackContext);

            if (!InputHandlers.TryGetValue(inputState.Id, out var currentHandler))
                currentHandler = null;

            if (inputState.Pressed)
            {
                if (currentHandler != null) // Se jà temos um handler com esse Id
                {
                    if (currentHandler.Finished) return; // Se já foi finalizado não faz nada

                    var dragState = currentHandler.Drag(inputState);

                    if (dragState != null)
                    {
                        ActiveTouchCommands[currentHandler.HandlerId].UpdateScreenPosition(dragState.Position);
                    }
                }
                else // Se não tivermos, criamos um novo handler para esse input
                {
                    var touchInputHandler = new TouchInputHandler(inputState);
                    InputHandlers.Add(inputState.Id, touchInputHandler);
                    ActiveTouchCommands.Add(touchInputHandler.HandlerId, new TouchCommand(inputState.Position));
                }
            }
            else
            {
                // Se jà temos um handler com esse Id
                if (currentHandler != null)
                {
                    if (!currentHandler.Finished)
                    {
                        currentHandler.Finish(inputState);
                        ActiveTouchCommands[currentHandler.HandlerId].FinishTouchCommand();
                    }

                    InputHandlers.Remove(inputState.Id);
                }
            }
        }

        private void OnDestroy()
        {
            InputHandlers?.Clear();
            TouchInputAction.action.performed -= OnTouchInput;
            TouchMoveAction.action.performed -= OnTouchInput;
        }

        private void OnEnable()
        {
            TouchInputAction.action.Enable();
            TouchMoveAction.action.Enable();
        }

        private void OnDisable()
        {
            TouchInputAction.action.Disable();
            TouchMoveAction.action.Disable();
        }
        
        public void DisableInput()
        {
            InputEnabled = false;
        }

        public void EnableInput()
        {
            InputEnabled = true;
        }
    }
}