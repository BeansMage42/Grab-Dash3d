using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Moveable : MonoBehaviour
{
    // Start and end points of the line
    private Vector3 pointA;
    private Vector3 pointB;


    //controls the path
    PathManager pathManager;

    //components
    Rigidbody rb;

    //temp test
    Vector3 prevPos;

    bool lockPos = true;
    
    AudioSource audioSource;
    [SerializeField] SoundObjectSO movingSound;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pathManager = GetComponent<PathManager>();
        List<Waypoint> points = pathManager.GetPath();
        pointA = points[0].GetPos();
        pointB = points[1].GetPos();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = movingSound.volume;
        audioSource.pitch = movingSound.pitch;
        audioSource.loop = movingSound.loop;
        audioSource.clip = movingSound.clip;
    }
    

    private void Update()
    {
        if (!lockPos)
        {
            if (rb.position != prevPos)
            {
                rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
            prevPos = rb.position;
        }
        //zero out velocity so it doesnt keep moving
        rb.velocity = Vector3.zero;
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.tag == "Hand")
        {
            lockPos = false;
            
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Hand")
        {
            lockPos = true;
            audioSource.Stop();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.position = rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
        }
    }
    private void FixedUpdate()
    {/*
        if (rb.position != prevPos)
        {
            rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
        }
        prevPos = rb.position;*/

    }
    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

        return origin + heading * dotP;
    }
}
