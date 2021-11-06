using System;
using UnityEngine;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    public class TouchCommand
    {
        public int InputId { get; set; }
        public Vector2 ScreenPosition { get; set; }
        public bool FinishingInputCommand { get; set; }
        public Vector2 WorldPosition { get; set; }

        public TouchCommand(int inputId, Vector2 screenPosition, bool finishingInputCommand = false)
        {
            InputId = inputId;
            ScreenPosition = screenPosition;
            FinishingInputCommand = finishingInputCommand;
            
            WorldPosition = Vector2.negativeInfinity;
        }

        public void UpdateScreenPosition(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }

        public void PrepareWorldPosition(Camera camera)
        {
            WorldPosition = camera.ScreenToWorldPoint(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));
        }
    }
}