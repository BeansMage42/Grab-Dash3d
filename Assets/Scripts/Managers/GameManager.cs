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
    [SerializeField] private Transform playerSpawn;

    [SerializeField] private GameObject trackerParent;
    [SerializeField] private GameObject levelParent;
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
        zManager.OnZEDReady += zManager.StartBodyTracking;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) { 
            m_NetworkManager.StartHost();
            levelParent.SetActive(true);
            trackerParent.SetActive(true);
        }
        if (GUILayout.Button("Client"))
        {
            m_NetworkManager.StartClient();
            Debug.Log("client");
        }
        //if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
    }

    private void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }


public float GetCamDistance()
    {
        return camDistance;
    }

   // [Rpc(SendTo.Server)]
    public GameObject SpawnPlayer( /*RpcParams rpcParams = default*/)
    {
        
        if (!m_NetworkManager.IsServer && !m_NetworkManager.IsHost) return null;
        
        GameObject temp = Instantiate(player, playerSpawn.position, Quaternion.identity);
        Debug.Log("playerCreated");
        
        return temp;
    }
}
