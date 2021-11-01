using System;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    public class TouchInputHandler
    {
        public Guid HandlerId { get; private set; }
        public bool Finished { get; private set; }
        private TouchInputState CurrentInput { get; set; }

        public TouchInputHandler(TouchInputState inputState)
        {
            HandlerId = Guid.NewGuid();
            Finished = false;
            CurrentInput = inputState;
        }

        private bool IsSamePointer(TouchInputState otherInput)
        {
            return CurrentInput.Id == otherInput.Id;
        }

        public TouchInputState Drag(TouchInputState inputValue)
        {
            if (!IsSamePointer(inputValue))
                return null;

            CurrentInput = inputValue;

            return CurrentInput;
        }

        public TouchInputState Finish(TouchInputState inputValue)
        {
            Finished = true;

            if (IsSamePointer(inputValue))
            {
                CurrentInput = inputValue;
            }

            return CurrentInput;
        }
    }
}