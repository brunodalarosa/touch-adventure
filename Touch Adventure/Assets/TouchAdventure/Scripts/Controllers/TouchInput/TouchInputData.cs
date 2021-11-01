using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    public class TouchInputState
    {
        public int Id { get; }
        public Vector2 Position { get; }
        public bool Pressed { get; }

        private TouchInputState(Pointer pointer)
        {
            Id = pointer.deviceId;
            Position = pointer.position.ReadValue();
            Pressed = pointer.press.ReadValue() > 0;
        }

        private TouchInputState(TouchControl touch)
        {
            Id = touch.touchId.ReadValue();
            Position = touch.position.ReadValue();
            Pressed = touch.isInProgress;
            touch.phase.ReadValue().IsActive();
        }

        public static TouchInputState GetByCallbackContext(InputAction.CallbackContext callbackContext) =>
            callbackContext.control.parent switch
            {
                TouchControl touchControl => new TouchInputState(touchControl),
                Pointer pointer => new TouchInputState(pointer),
                _ => null,
            };
    }
}