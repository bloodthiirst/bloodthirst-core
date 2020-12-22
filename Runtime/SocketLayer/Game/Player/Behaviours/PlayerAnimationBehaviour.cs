using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Game.Player.Behaviours
{
    public class PlayerAnimationBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputNetworkBehaviour InputBehaviour;

        [SerializeField]
        private Animator animator;

        private int isWalkingHash;

        private int horizontalWalking;

        private int verticalWalking;

        private int dance;

        [SerializeField]
        [ReadOnly]
        private bool isWalking;

        [SerializeField]
        [ReadOnly]
        private Vector2 cachedInput;

        public void TriggerDance()
        {
            animator.SetTrigger(dance);
        }

        private void Awake()
        {
            InputBehaviour = GetComponent<PlayerInputNetworkBehaviour>();

            isWalkingHash = Animator.StringToHash(nameof(isWalking));

            horizontalWalking = Animator.StringToHash(nameof(horizontalWalking));

            verticalWalking = Animator.StringToHash(nameof(verticalWalking));

            dance = Animator.StringToHash(nameof(dance));

        }

        private void Update()
        {
            cachedInput = InputBehaviour.PlayerInput;

            animator.SetBool(isWalkingHash, cachedInput.magnitude != 0);

            animator.SetFloat(horizontalWalking, cachedInput.x);

            animator.SetFloat(verticalWalking, cachedInput.y);
        }

    }
}
