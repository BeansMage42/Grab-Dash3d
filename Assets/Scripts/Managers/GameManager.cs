using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [SerializeField] private float camDistance;
    //ZED ASSETS
     public ZEDBodyTrackingManager btm;
     public ZEDManager zManager;

    public GameObject handL, handR;
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
}
