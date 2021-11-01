using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CloudController : MonoBehaviour
    {
        private CircleCollider2D Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
            
            Destroy(gameObject, 3.25f);
        }
    }
}