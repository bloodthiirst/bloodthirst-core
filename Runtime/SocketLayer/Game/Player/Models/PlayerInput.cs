using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Models
{
    [Serializable]
    public struct PlayerInput
    {
        /// <summary>
        /// 1 byte = 8 bits
        /// 2 bits for horizontal
        /// 2 bits for vertical
        /// </summary>
        public byte input;

        public PlayerInput(int horizontal, int vertical)
        {
            input = 0;

            switch (horizontal)
            {
                case 1:
                    input |= 1;
                    break;
                case -1:
                    input |= 2;
                    break;
                default:
                    break;
            }

            switch (vertical)
            {
                case 1:
                    input |= (1 << 2);
                    break;
                case -1:
                    input |= (2 << 2);
                    break;
                default:
                    break;
            }
        }

        [ShowInInspector]
        public int x
        {
            get
            {
                switch (input & 3)
                {
                    case 1:
                        return 1;
                    case 2:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        [ShowInInspector]
        public int y
        {
            get
            {
                switch (input >> 2)
                {
                    case 1:
                        return 1;
                    case 2:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        public static implicit operator Vector2(PlayerInput rhs)
        {
            Vector2 vec = new Vector2(rhs.x, rhs.y);
            return vec;
        }

    }
}

