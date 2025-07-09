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


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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
            rb.AddForce(Vector3.up * speed * jumpForce);

            jumpBufferCounter = 0f;
            //Jump();
        }

        // -- Handle input -- 
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir -= Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        moveDir = moveDir.normalized;
        Move();
    }

    private void FixedUpdate()
    {

    }

    private void Move()
    {
        if (moveDir == Vector3.zero)
            return;

        rb.AddForce(moveDir * speed * Time.deltaTime, ForceMode.Impulse);
    }

    private void Jump()
    {
        jumpBufferCounter = jumpBufferTime;
        if (coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * speed * jumpForce);
            jumpBufferCounter = 0f;
        }
    }


    private bool Grounded()
    {
        return Physics.CheckSphere(jumpCheckPos.position, 0.2f, groundLayer);
    }
}
