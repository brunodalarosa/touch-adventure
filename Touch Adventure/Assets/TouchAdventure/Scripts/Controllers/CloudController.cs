using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CloudController : MonoBehaviour
    {
        private ICloudPathPointerListener Listener { get; set; }
        public CircleCollider2D Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<CircleCollider2D>();
        }

        public void Init(ICloudPathPointerListener listener)
        {
            gameObject.SetActive(true);
            Listener = listener;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("FloatingFallingCharacter"))
            {
                Listener.OnFloatingFallingContact();
            }
        }

        public void DestroyCloud()
        {
            DestroyImmediate(gameObject);
        }
    }
}