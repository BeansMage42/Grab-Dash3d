using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Rigidbody rb;
    protected float zDist;
    protected bool leftHandOn = false;
    protected bool rightHandOn = false;
    protected bool isGrabbed = false;

    //ZED ASSETS
    protected ZEDBodyTrackingManager btm;
    protected ZEDManager zManager;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();

        btm = GameManager.Instance.btm;
        zManager = GameManager.Instance.zManager;

        zManager.OnBodyTracking += OnTrackHandDepth;
    }
    private void OnTriggerEnter(Collider other)
    {
        DetectHands(other,true);
    }

    private void OnTriggerExit(Collider other)
    {
        DetectHands (other,false);
    }


    protected virtual void DetectHands(Collider other, bool entered)
    {
       if(other.gameObject.tag == "HandL")
        {
            leftHandOn = entered;
        }

       if(other.gameObject.tag == "HandR")
        {
            rightHandOn = entered;
        }
    }

    private void OnTrackHandDepth(BodyTrackingFrame frame)
    {
        if (leftHandOn && rightHandOn) 
        { 
        
        }

    }
}
