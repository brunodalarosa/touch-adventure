using System.Collections.Generic;
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
        [field: SerializeField] private float InputInterval { get; set; } = 0.04f;
    
        private Dictionary<int, TouchInputHandler> InputHandlers { get; set; }
        private bool InputEnabled { get; set; }
        
        private Camera MainCamera { get; set; }
        private ITouchInputListener Listener { get; set; }
        private TouchCommand TouchCommand { get; set; }
        
        
        private float CurrentInputInterval { get; set; }

        public void Init(Camera mainCamera, ITouchInputListener listener)
        {
            MainCamera = mainCamera;
            Listener = listener;
            
            InputHandlers = new Dictionary<int, TouchInputHandler>();
            
            TouchInputAction.action.performed += OnTouchInput;
            TouchMoveAction.action.performed += OnTouchInput;

            TouchCommand = null;
            InputEnabled = true;

            CurrentInputInterval = InputInterval;
        }

        private void Update()
        {
            CurrentInputInterval -= Time.deltaTime;

            if (CurrentInputInterval > 0) return;
            if (TouchCommand == null) return;

            Listener.OnTouch(TouchCommand);
            TouchCommand = null;
            CurrentInputInterval = InputInterval;
        }

        private void OnTouchInput(InputAction.CallbackContext callbackContext)
        {
            if (TouchCommand != null) return;
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
                        TouchCommand = new TouchCommand(currentHandler.HandlerId, GetWorldPosition(dragState.Position));
                    }
                }
                else // Se não tivermos, criamos um novo handler para esse input
                {
                    InputHandlers.Add(inputState.Id, new TouchInputHandler(inputState));
                }
            }
            else
            {
                // Se jà temos um handler com esse Id
                if (currentHandler != null)
                {
                    if (!currentHandler.Finished)
                    {
                        var activeTouch = currentHandler.Finish(inputState);
                        if (activeTouch != null)
                        {
                            TouchCommand = new TouchCommand(currentHandler.HandlerId, GetWorldPosition(activeTouch.Position), true);
                        }
                    }

                    InputHandlers.Remove(inputState.Id);
                }
            }
        }
        
        private Vector3 GetWorldPosition(Vector2 InputPosition)
        {
            return MainCamera.ScreenToWorldPoint(new Vector3(InputPosition.x, InputPosition.y, 0));
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