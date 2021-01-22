using Assets.Models;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public class PlayerPositionNetworkBehaviour : GUIDNetworkBehaviourBase, IOnPlayerSpawnedServer, IPlayerSpawnSuccess
    {
        public bool IsDoneServerSpawn { get; set; }

        private PlayerPostionPacketClientProcessor playerPostionClient;

        private INetworkSerializer<Guid> identifier;

        private Vector3 cachedPosition;

        private void Awake()
        {
            identifier = new IdentityGUIDNetworkSerializer();

            playerPostionClient = NetworkEntity.GetClient<PlayerPostionPacketClientProcessor, Vector3>();
            playerPostionClient.OnPacketParsedUnityThread += OnPositionClient;
        }

        private void OnPositionClient(Vector3 pos, Guid from)
        {
            Vector3 difference = pos - transform.position;

            // if the difference is too high then snap it into place
            /*
            if(difference.magnitude > 1)
            {
                transform.position = pos;
            }

            // else interpolate

            else
            {
                transform.position += difference * 0.1f;
            }
            */

            transform.position = pos;

        }

        private void Update()
        {
            if (!IsServer)
                return;

            cachedPosition = transform.position;

            byte[] Packet = PacketBuilder.BuildPacket(NetworkID, cachedPosition, identifier, Vector3NetworkSerializer.Instance);

            if (IsServer && !IsClient)
            {
                SocketServer.BroadcastUDP(Packet);
            }
            else if (IsServer && IsClient)
            {
                SocketServer.BroadcastUDP(Packet, id => !id.Equals(SocketClient<Guid>.CurrentNetworkID));
            }
        }

        void IOnPlayerSpawnedServer.OnPlayerSpawnedServer()
        {
            StartCoroutine(CrtSpawnPlayerOnMap());
        }

        private IEnumerator CrtSpawnPlayerOnMap()
        {
            Vector3 initPos = Vector3.zero;

            Vector3 offset = new Vector3(0, 0, 0);

            Ray upRay = new Ray(Vector3.down * 100, Vector3.up);
            Ray downRay = new Ray(Vector3.up * 100, Vector3.down);


            // check if ground is on top

            List<RaycastHit> hits = new List<RaycastHit>();

            hits.AddRange(Physics.RaycastAll(upRay));
            hits.AddRange(Physics.RaycastAll(downRay));


            foreach (RaycastHit h in hits)
            {
                if (h.collider.gameObject.CompareTag("Terrain"))
                {
                    initPos = h.point;
                    break;
                }
            }

            Vector3 onFloorValue = initPos + offset;

            transform.position = onFloorValue;

            Rigidbody rb = transform.GetComponent<Rigidbody>();

            yield return new WaitForSeconds(3f);

            yield return new WaitUntil(() => rb.velocity == Vector3.zero && rb.angularVelocity == Vector3.zero);

            IsDoneServerSpawn = true;
        }

        void IPlayerSpawnSuccess.OnPlayerSpawnSuccess(GUIDPlayerSpawnSuccess spawnInfo)
        {
            byte[] positionPacket = PacketBuilder.BuildPacket(NetworkID, transform.position, SocketServer.IdentifierSerializer, Vector3NetworkSerializer.Instance);

            SocketServer.ClientConnexionManager.ClientConnexions[spawnInfo.ClientThePlayerSpawnedIn].SendTCP(positionPacket);
        }
    }
}
