﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public abstract class NetworkEntitySpawnerBase<TIdentity> : MonoBehaviour where TIdentity : IComparable<TIdentity>
    {
        [SerializeField]
        protected abstract NetworkPlayerEntityBase<TIdentity> PlayerPrefab { get; }

        [ShowInInspector]
        [HorizontalGroup(Width = 1f, MinWidth = 1)]
        [PreviewField(Height = 250)]
        [ReadOnly]
        private GameObject prefabPreview;

        private void OnValidate()
        {
            if (PlayerPrefab != null)
                prefabPreview = PlayerPrefab.gameObject;
        }

        protected ISocketClient<TIdentity> SocketClient;

        protected ISocketServer<TIdentity> SocketServer;


    }
}
