
using UnityEngine;
public class PlayerController : MonoBehaviour
{

    //Player Components
    [Header("Components")]
    Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    private AudioSource source;
    
    //Direction of movement
    Vector3 moveDir;
    [Header("Movement Speed")]
    //Movement
    [SerializeField] float speed = 40f;
    [SerializeField] private float deccelerateRate;
    private Vector3 zeroSpeed = Vector3.zero;

    // -- Jumping --
    [SerializeField] float jumpForce = 20f;

    [Header("Jump check controls")]
    private bool isJumping;
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

    [Header("Audio clips")]
    [SerializeField] SoundObjectSO jump;
    [SerializeField] private SoundObjectSO[] footsteps;
    [SerializeField] private SoundObjectSO deathSound;
    [SerializeField] private SoundObjectSO checkPointSound;

    [Header("Foot step randomizer")]
    [SerializeField] private float minFootSpeed;
    [SerializeField] private float maxFootSpeed;
    [SerializeField,Range(0.1f,2)] private float minFootPitch = 1;
    [SerializeField, Range(0.1f, 2)] private float maxFootPitch = 1;
    [SerializeField, Range(0.1f, 2)] private float minFootVol = 1;
    [SerializeField, Range(0.1f, 2)] private float maxFootVol  = 1;
    private float footTimer;
    private float footDelayTime;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
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
        
        if(transform.position.y < -10f)
        {
            KillPlayer();
        }

        //moveDir = Vector3.zero;

        if (Grounded())
        {
            coyoteTimeCounter = coyoteTime;
            
            //only plays footstep audio while on the ground
            if (Mathf.Abs(rb.velocity.x) > 0)
            {
                footTimer += Time.deltaTime;
                if(footTimer > footDelayTime)
                {
                    footTimer = 0;
                    footDelayTime = Random.Range(minFootSpeed, maxFootSpeed);
                    SoundObjectSO currentStep = footsteps[Random.Range(0, footsteps.Length-1)];
                    float pitchMod = Random.Range(minFootPitch, maxFootPitch);
                    float volMod = Random.Range(minFootVol, maxFootVol);
                    source.pitch = currentStep.pitch * pitchMod;
                    source.volume = currentStep.volume * volMod;
                    source.clip = currentStep.clip;
                    source.Play();
                    
                }

            }
        }
        coyoteTimeCounter -= Time.deltaTime;
        
        jumpBufferCounter -= Time.deltaTime;

        // Jump with jump buffer
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            Debug.Log("jump buffer");
            jumpBufferCounter = 0f;
            
             rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
             rb.AddForce(Vector3.up * jumpForce);
            source.PlayOneShot(jump.clip);
             
        }
        animator.SetBool("isMoving", Mathf.Abs(rb.velocity.x) > 0);
    }

    private void FixedUpdate()
    {
    }

    public void Move(Vector3 inputDirection)
    {
        moveDir = inputDirection;
        if (moveDir.magnitude > 0) 
        {
            
            spriteRenderer.flipX = (moveDir.x > 0)? true : false;

        }
        //Debug.Log("move " + moveDir);
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new((speed * airSpeedMod) * moveDir.x, rb.velocity.y, rb.velocity.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroSpeed, deccelerateRate);
        RaycastHit[] hits = new RaycastHit[2];
        if (Physics.BoxCastNonAlloc(transform.position, gameObject.transform.lossyScale / 2.2f, moveDir, hits, Quaternion.identity, 0.09f, groundLayer) > 0)
        {
           // Debug.Log("wall detected");
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

    }
    public void Jump()
    {
        Debug.Log("Attempt jump");
        if (jumpBufferCounter <= 0) 
        {
            jumpBufferCounter = jumpBufferTime;
        }
        if (coyoteTimeCounter > 0f)
        {/*
            //Debug.Log("jump");
            source.PlayOneShot(jump.clip);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce);
            jumpBufferCounter = 0f;*/
        }
    }


    public bool Grounded()
    {
        return Physics.CheckBox(gameObject.transform.position-jumpCheckYOffset, gameObject.transform.lossyScale/2.2f,gameObject.transform.rotation,groundLayer);
    }

    public void KillPlayer()
    {
       onPlayerDeath?.Invoke();
        source.PlayOneShot(deathSound.clip);
    }


    private void Respawn()
    {
        transform.position = checkPoint;
        rb.velocity = Vector3.zero;
    }
    private void NewCheckPoint(Vector3 checkPointPos)
    {
        if (checkPoint != checkPointPos)
        {
            checkPoint = checkPointPos;
            source.PlayOneShot(checkPointSound.clip);
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gameObject.transform.position - jumpCheckYOffset, gameObject.transform.lossyScale / 2.2f);
    }
}
