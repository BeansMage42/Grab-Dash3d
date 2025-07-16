using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
public class PlayerController : MonoBehaviour
{

    //Player Components
    Rigidbody rb;
    
    //Direction of movement
    Vector3 moveDir;

    //Movement
    [SerializeField] float speed = 40f;
    [SerializeField] private float deccelerateRate;
    private Vector3 zeroSpeed = Vector3.zero;

    // -- Jumping --
    [SerializeField] float jumpForce = 20f;
    //bool isGrounded;
    float coyoteTimeCounter;
    [SerializeField] float coyoteTime;
    [SerializeField] float jumpBufferTime;
    float jumpBufferCounter;
    [SerializeField] Transform jumpCheckPos;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector3 jumpCheckYOffset = new Vector3(0,0.1f,0);
    private float airSpeedMod = 1;

    //Player Death and spawning
    private Vector3 checkPoint;
    public delegate void OnPlayerDeath();
    public event OnPlayerDeath onPlayerDeath;

    public delegate void OnPlayerWin();
    public event OnPlayerWin onPlayerWin;

    //Level winning
    private bool hasFinished = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        checkPoint = transform.position;
        onPlayerDeath += Respawn;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(transform.position.y < -20f)
        {
            KillPlayer();
        }

        moveDir = Vector3.zero;

        if (Grounded())
        {
           // Debug.Log("isgrounded");
            coyoteTimeCounter = coyoteTime;
        }
        else
        {/*
            rb.velocity = rb.velocity + (Physics.gravity * Time.deltaTime * 2f);
            airSpeedMod = 0.6f;*/
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter -= Time.deltaTime;

        // Jump with jump buffer
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            Debug.Log("jump buffer");
             rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
             rb.AddForce(Vector3.up * jumpForce);

             jumpBufferCounter = 0f;
        }

    }

    private void FixedUpdate()
    {
    }

    public void Move(Vector3 inputDirection)
    {
        moveDir = inputDirection;
        //Debug.Log("move " + moveDir);
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new((speed * airSpeedMod) * moveDir.x, rb.velocity.y, rb.velocity.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroSpeed, deccelerateRate);
        RaycastHit[] hits = new RaycastHit[2];
        if (Physics.BoxCastNonAlloc(transform.position, gameObject.GetComponent<BoxCollider>().size / 2.2f, moveDir, hits, Quaternion.identity, 0.05f, groundLayer) > 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

    }
    public void Jump()
    {
        Debug.Log("Attempt jump");
        jumpBufferCounter = jumpBufferTime;
        if (coyoteTimeCounter > 0f)
        {
            Debug.Log("jump");
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce);
            jumpBufferCounter = 0f;
        }
    }


    public bool Grounded()
    {
        return Physics.CheckBox(gameObject.transform.position-jumpCheckYOffset, gameObject.transform.lossyScale/2.2f,gameObject.transform.rotation,groundLayer);
    }

    public void KillPlayer()
    {
       onPlayerDeath?.Invoke();
    }


    private void Respawn()
    {
        transform.position = checkPoint;
        rb.velocity = Vector3.zero;
    }
    private void NewCheckPoint(Vector3 checkPointPos)
    {
        checkPoint = checkPointPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Obstacle") KillPlayer();
        else if(other.gameObject.tag == "CheckPoint") NewCheckPoint(other.transform.position);
        else if(other.gameObject.tag == "Exit" && !hasFinished)
        {
            hasFinished = true;
            NewCheckPoint(other.transform.position);
            GameManager.Instance.PlayerFinish();
        }
    }
}
