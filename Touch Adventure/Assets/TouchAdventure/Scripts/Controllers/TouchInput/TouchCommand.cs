using System;
using UnityEngine;

namespace TouchAdventure.Scripts.Controllers.TouchInput
{
    public class TouchCommand
    {
        public Guid InputId { get; set; }
        public Vector2 WorldPosition { get; set; }
        public bool FinishingInputCommand { get; set; }

        public TouchCommand(Guid inputId, Vector2 worldPosition, bool finishingInputCommand = false)
        {
            InputId = inputId;
            WorldPosition = worldPosition;
            FinishingInputCommand = finishingInputCommand;
        }
    }
}