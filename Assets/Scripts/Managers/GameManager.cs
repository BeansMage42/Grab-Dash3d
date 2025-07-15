using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private NetworkManager m_NetworkManager;


    [SerializeField] private float camDistance;
    //ZED ASSETS
     public ZEDBodyTrackingManager btm;
     public ZEDManager zManager;

    public GameObject handL, handR;

    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 playerSpawn;
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        m_NetworkManager = GetComponent<NetworkManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCamDistance()
    {
        return camDistance;
    }

    [Rpc(SendTo.Server)]
    public NetworkObject SpawnPlayerRpc(ulong clientId, RpcParams rpcParams = default)
    {
        if (!m_NetworkManager.IsServer) return null;
        GameObject temp = Instantiate(player, playerSpawn, Quaternion.identity);
        NetworkObject newPlayer = temp.GetComponent<NetworkObject>();
        newPlayer.SpawnWithOwnership(clientId);
        Debug.Log("playerSpawned");
        
        return newPlayer;
    }
}
