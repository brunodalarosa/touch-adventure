using UnityEngine;

namespace TouchAdventure.Scripts.Controllers
{
    public class FloatingFallingGameController : MonoBehaviour
    {
        [field: SerializeField] private Camera MainCamera { get; set; }
        [field: SerializeField] private FloatingFallingCharacterController Egg { get; set; }
        [field: SerializeField] private MagicWandController MagicWandController { get; set; }

        private void Start()
        {
            MagicWandController.Init(MainCamera, Egg);
        }
    }
}