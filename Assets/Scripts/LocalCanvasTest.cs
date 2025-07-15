using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LocalCanvasTest : NetworkBehaviour
{
     [SerializeField] private PlayerController player;
    [SerializeField] private GameObject holder;
    public override void OnNetworkSpawn()
    {
        if(IsHost && IsOwner)
        {
            gameObject.SetActive(false);
        }
        if (!IsOwner && !IsServer && !IsHost) 
        {
            NetworkObject.NetworkHide(NetworkManager.LocalClientId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            MoveRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc (RpcParams rpcParams = default)
    {
        if (IsClient)
        {
            //player.TestConnection();
        }

    }

}
