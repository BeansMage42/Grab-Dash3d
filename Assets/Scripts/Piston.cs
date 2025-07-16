using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    [Tooltip("Highest Y coordinate in global space")]
    [SerializeField] float maxHeight = 1f;
    [Tooltip("lowest Y coordinate in global space")]
    [SerializeField] float minHeight = 1f;
    [Tooltip("Speed of up/down motion")]
    [SerializeField] float cycleSpeed = 1f;
    [Tooltip("Delay before switching directions")]
    [SerializeField] float waitTime;
    [Tooltip("Offset the sine wave by X seconds")]
    [SerializeField] float cycleOffset = 0f;

    private float amplitude;
    private float centre;

    private float timer;
    private float cycle;

   // [SerializeField] Vector3 checkOffset = new Vector3(0,0.1f,0);
    [SerializeField] float checkOffset = 0.1f;
    [SerializeField] LayerMask handLayer;
    [SerializeField] LayerMask playerLayer;
    bool isWaiting;
    private bool goingUp;
    
    // Start is called before the first frame update
    void Start()
    {
        centre = (maxHeight + minHeight)*0.5f;
        amplitude = maxHeight - centre;

        if(Mathf.Sin(cycleSpeed * (cycle + 0.1f + cycleOffset)) > 0)
        {
            goingUp = true;
        }
        else
        {
            goingUp = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!HandsBelow() && !isWaiting)
        {
            cycle += Time.deltaTime;
            float yPos = (amplitude * Mathf.Sin(cycleSpeed * (cycle + cycleOffset))) + centre;
            //Debug.Log(yPos);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            if(!goingUp) CheckKillPlayers();

        }
        if (goingUp && transform.position.y >= maxHeight - 0.01f && !isWaiting)
        {
            isWaiting = true;
            goingUp = false;
            timer = waitTime;
            
        }
        else if (!goingUp && transform.position.y <= minHeight + 0.01f && !isWaiting)
        {
            isWaiting = true;
            goingUp = true;
            timer = waitTime;
        }
        if (isWaiting)
        { 
            timer -= Time.deltaTime;
            if (timer <= 0f)isWaiting = false;
        }
    }


    private bool HandsBelow()
    {
        bool stop = false;
        Vector3 dir = Vector3.up * (goingUp? 1f : -1f);
        RaycastHit[] hits = new RaycastHit[2];
        if(Physics.BoxCastNonAlloc(transform.position, gameObject.transform.lossyScale * 0.5f,dir,hits, Quaternion.identity,checkOffset, handLayer) > 0)
        {
            for (int i = 0; i < 1; i++) 
            {
                if (hits[i].transform == null) continue;
              Vector3  fromTo =  hits[i].collider.transform.position - transform.position;
                float dot = Vector3.Dot(Vector3.up, fromTo);
                if (goingUp)
                {
                    if (dot > 0) stop = true;
                }
                else if (dot < 0) stop = true;
                    

            }
        }
       return stop;
    }

    private void CheckKillPlayers()
    {
        Vector3 dir = -Vector3.up;
        RaycastHit[] hits = new RaycastHit[10];
        if (Physics.BoxCastNonAlloc(transform.position, gameObject.transform.lossyScale * 0.4f, dir, hits, Quaternion.identity, checkOffset, playerLayer) > 0)
        {
            Debug.Log("players hit" + hits[0].collider.name);
            foreach(RaycastHit hit in hits) 
            {
                if (hit.collider == null) continue;
                GameObject obj = hit.collider.gameObject;
                if(obj.TryGetComponent(out PlayerController hitPlayer))
                {
                    Debug.Log("player found");
                    if (hitPlayer.Grounded())
                    {
                        Debug.Log("player killed");
                        hitPlayer.KillPlayer();
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
    
}
