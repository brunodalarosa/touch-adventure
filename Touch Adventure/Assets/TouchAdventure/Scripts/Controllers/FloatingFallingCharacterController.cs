using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FloatingFallingCharacterController : MonoBehaviour
    {
        private const float MaxYVelocity = 1f;
        private const float MaxXVelocity = 1f;
        
        private Rigidbody2D Rigidbody2D { get; set; }
        
        [field: SerializeField] public CircleCollider2D NoTouchArea { get; private set; }

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var velocity = Rigidbody2D.velocity;
            
            var velocityX = Mathf.Abs(velocity.x);
            var velocityY = Mathf.Abs(velocity.y);

            float adjustedXVelocity = velocity.x;
            float adjustedYVelocity = velocity.y;

            if (velocityX > MaxXVelocity)
            {
                adjustedXVelocity = velocityX > MaxXVelocity ? MaxXVelocity : velocityX;
                adjustedXVelocity *= velocity.x < 0 ? -1 : 1;
            }

            if (velocityY > MaxYVelocity)
            {
                adjustedYVelocity = velocityY > MaxYVelocity ? MaxYVelocity : velocityY;
                adjustedYVelocity *= velocity.y < 0 ? -1 : 1;
            }

            Rigidbody2D.velocity = new Vector2(adjustedXVelocity, adjustedYVelocity);
        }
    }
}
