using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDir;

    [SerializeField] float speed = 4000f;

    // -- Jumping --
    [SerializeField] float jumpForce = 20f;
    bool isGrounded;
    float coyoteTimeCounter;
    [SerializeField] float coyoteTime;
    [SerializeField] float jumpBufferTime;
    float jumpBufferCounter;
    [SerializeField] Transform jumpCheckPos;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] private float deccelerateRate;


    private Vector3 checkPoint;

    private Vector3 zeroSpeed = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        checkPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -20f)
        {
            transform.position = checkPoint;
            rb.velocity = Vector3.zero;
        }

        moveDir = Vector3.zero;

        if (Grounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter -= Time.deltaTime;

        // Jump with jump buffer
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce);

            jumpBufferCounter = 0f;
            //Jump();
        }

        // -- Handle input -- 
        if (Input.GetKey(KeyCode.A))
        {
            moveDir -= Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        moveDir = moveDir.normalized;
        Move();
    }


    private void Move()
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector3(speed * moveDir.x, rb.velocity.y,rb.velocity.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity,ref zeroSpeed, deccelerateRate);
        RaycastHit[] hits = new RaycastHit[2];
        if(Physics.BoxCastNonAlloc(transform.position,gameObject.GetComponent<BoxCollider>().size/2.2f,moveDir,hits,Quaternion.identity,0.05f,groundLayer ) > 0)
        {
            rb.velocity = new Vector3(0,rb.velocity.y,0);
        }

    }

    private void Jump()
    {
        jumpBufferCounter = jumpBufferTime;
        if (coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce);
            jumpBufferCounter = 0f;
        }
    }


    private bool Grounded()
    {
        return Physics.CheckSphere(jumpCheckPos.position, 0.2f, groundLayer);
    }
}
