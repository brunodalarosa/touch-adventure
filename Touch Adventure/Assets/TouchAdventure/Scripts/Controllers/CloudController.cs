using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CloudController : MonoBehaviour
    {
        public CircleCollider2D Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
        }

        public void InstantPoof()
        {
            DestroyImmediate(gameObject);
        }
    }
}