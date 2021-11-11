using System;
using System.Collections;
using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    
    public class CloudPathPoint : MonoBehaviour, ICloudPathPointerListener
    {
        private const int SecondsToIdlePoof = 60;
        private const int SecondsToContactPoof = 2;
        
        [field: SerializeField] public CloudController Controller { get; private set; }

        public Vector2 Point { get; private set; }
        public bool CloudEnabled { get; private set; }
        public bool Poofed { get; private set; }

        public void Init(Vector2 point, bool enableCloud)
        {
            Point = point;
            CloudEnabled = enableCloud;
            Poofed = false;

            if (enableCloud)
            {
                Controller.Init(this);
                StartCoroutine(IdlePoofCoroutine());
            }
            else
            {
                Controller.gameObject.SetActive(false);
            }
        }

        public void Poof()
        {
            if (Poofed) throw new InvalidOperationException("Trying to instant poof an already poofed cloud!");

            StopCoroutine(IdlePoofCoroutine());
            Controller.DestroyCloud();
            Poofed = true;
        }

        private IEnumerator IdlePoofCoroutine()
        {
            yield return new WaitForSeconds(SecondsToIdlePoof);
            if (!Poofed) Poof();
        }

        public void OnFloatingFallingContact()
        {
            StartCoroutine(ContactPoofCoroutine());
        }

        private IEnumerator ContactPoofCoroutine()
        {
            yield return new WaitForSeconds(SecondsToContactPoof);
            if (!Poofed) Poof();
        }
    }

    public interface ICloudPathPointerListener
    {
        void OnFloatingFallingContact();
    }
}