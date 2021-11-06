using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    public class CloudPathPoint
    {
        private const int SecondsToIdlePoof = 60;
        public Vector2 Point { get; private set; }
        public CloudController Controller { get; private set; }

        public bool HasCloud { get; private set; }

        public bool Poofed { get; private set; }

        public CloudPathPoint(Vector2 point, [CanBeNull] CloudController controller)
        {
            Point = point;
            Controller = controller;

            HasCloud = controller != null;
            Poofed = false;

            if (Controller != null) Controller.StartCoroutine(IdlePoofCoroutine());
        }

        public void InstantPoof()
        {
            if (Poofed) throw new InvalidOperationException("Trying to instant poof an already poofed cloud!");

            Controller.StopCoroutine(IdlePoofCoroutine());
            Controller.InstantPoof();
            Poofed = true;
        }

        private IEnumerator IdlePoofCoroutine()
        {
            yield return new WaitForSeconds(SecondsToIdlePoof);
            
            if (Controller != null) InstantPoof();
        }
    }
}