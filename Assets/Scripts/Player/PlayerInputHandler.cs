using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInputHandler : NetworkBehaviour
{

    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject holder;
    private NetworkVariable<Vector3> moveDir = new NetworkVariable<Vector3>(Vector3.zero,NetworkVariableReadPermission.Everyone ,NetworkVariableWritePermission.Owner);
    private bool enableKeyboardControls = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost && IsOwner)
        {
            gameObject.SetActive(false);
        }
        if (!IsOwner && !IsServer && !IsHost)
        {
            gameObject.SetActive(false);
        }
        if (IsHost)
        {
            holder.SetActive(false);
        }
        if(IsClient && !IsHost && IsOwner) CreatePlayerRpc();
    }
    [Rpc(SendTo.Server)]
    private void CreatePlayerRpc()
    {
        player = GameManager.Instance.SpawnPlayer().GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKeyDown(KeyCode.T)) enableKeyboardControls = !enableKeyboardControls;

        if (enableKeyboardControls)
        {
            moveDir.Value = new( Input.GetAxis("Horizontal"),0,0);
            if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)) SubmitJumpRpc();
        }
        Debug.Log(moveDir.Value);
        SubmitMoveRequestRpc();
    }

    public void HorizontalButton(Vector3 dir)
    {
        moveDir.Value += dir;
    }

    

        [Rpc(SendTo.Server)]
    private void SubmitMoveRequestRpc()
    {
        
        player.Move(moveDir.Value.normalized);
    }

    [Rpc(SendTo.Server)]
    public void SubmitJumpRpc()
    {
        player.Jump();
    }

}
