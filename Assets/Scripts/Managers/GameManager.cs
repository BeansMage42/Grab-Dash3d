using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private NetworkManager m_NetworkManager;
    private SoundManager soundManager;


    [SerializeField] GameObject hiddenText;
    [SerializeField] GameObject canvas;

    [SerializeField] private float camDistance;
    //ZED ASSETS
     public ZEDBodyTrackingManager btm;
     public ZEDManager zManager;

    public GameObject handL, handR;


    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerSpawn;

    [SerializeField] private GameObject trackerParent;
    [SerializeField] private GameObject levelParent;

    private static int numPlayers;
    private static int playersAcrossFinish;
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
        soundManager = GetComponent<SoundManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        zManager.OnZEDReady += zManager.StartBodyTracking;

    }

    public void StartAsHost()
    {

        m_NetworkManager.StartHost();
        levelParent.SetActive(true);
        trackerParent.SetActive(true);
        soundManager.GameStart();
    }

    public void StartAsClient()
    {
        m_NetworkManager.StartClient();
    }
    public float GetCamDistance()
    {
        return camDistance;
    }

   // [Rpc(SendTo.Server)]
    public GameObject SpawnPlayer()
    {
        
        if (!m_NetworkManager.IsServer && !m_NetworkManager.IsHost) return null;
        GameObject temp = Instantiate(player, playerSpawn.position, Quaternion.identity);
        Debug.Log("playerCreated");
        
        return temp;
    }

    public int GetNumPlayers()
    {
        return m_NetworkManager.ConnectedClientsList.Count -1;
    }
    public void PlayerFinish()
    {
        playersAcrossFinish++;
        if(playersAcrossFinish >= GetNumPlayers())
        {
            GameWin();
        }
    }

    public void GameWin()
    {
        canvas.SetActive(true);
        hiddenText.SetActive(true);
        soundManager.GameEnd();
    }
}
