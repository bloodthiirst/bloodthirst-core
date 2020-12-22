using Assets.Scripts.SocketLayer.Game.Player;
using Assets.Scripts.SocketLayer.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMovementBehaviour : MonoBehaviour
    {

        [SerializeField]
        [Range(0, 10)]
        private float baseSpeed;

        [SerializeField]
        [ReadOnly]
        private Rigidbody rb;

        [SerializeField]
        [InfoBox("Values used to multiply the overall speed \n- x : speed when going sideways \n- y : speed when going backwards \n- z : speed when going forward")]
        private Vector3 speedMultipliers;

        [SerializeField]
        [ReadOnly]
        private float speedSum;

        [SerializeField]
        private PlayerInputNetworkBehaviour playerInputBehaviour;

        private Vector2 zero = Vector2.zero;

        private Vector3 InputToTranslation(PlayerInput playerInput)
        {
            Vector3 vec = Vector3.zero;

            vec += transform.right * playerInput.x;
            vec += transform.forward * playerInput.y;

            return vec.normalized;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            MoveUsingInput(playerInputBehaviour.PlayerInput);
        }

        public void MoveUsingInput(PlayerInput playerInput)
        {
            Vector2 cached = playerInput;

            if (cached == zero)
            {
                speedSum = 0;
                return;
            }

            speedSum = 0;

            int axis = 0;

            if (cached.x != 0)
            {
                axis++;

                speedSum += Mathf.Abs(cached.x) * speedMultipliers.x;
            }

            if (cached.y != 0)
            {
                axis++;

                if (cached.y > 0)
                {
                    speedSum += Mathf.Abs(cached.y) * speedMultipliers.z;
                }
                else
                {
                    axis = 1;
                    speedSum = Mathf.Abs(cached.y) * speedMultipliers.y;
                }
            }

            speedSum /= axis;

            var addedMovement = InputToTranslation(playerInput) * baseSpeed * speedSum * Time.deltaTime;
            rb.position += addedMovement;
        }
    }
}
