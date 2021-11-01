using UnityEngine;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    public interface ITouchInputListener
    {
        void OnTouch(TouchCommand touchCommand);
    }
}