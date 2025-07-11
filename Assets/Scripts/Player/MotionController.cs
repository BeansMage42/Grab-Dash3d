using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    //ZED ASSETS
    private ZEDBodyTrackingManager btm;
    private ZEDManager zManager;

    //POSITIONS
    private Vector3 handLeftpos, handRightpos, prevLeftPos, prevRightPos;

    //VISUALIZATION GAME OBJECTS
    private GameObject leftHandGM, rightHandGM;
    private Rigidbody leftRb, rightRb;

    private float zDist = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        leftHandGM = gameManager.handL;
        rightHandGM = gameManager.handR;
        btm = gameManager.btm;
        zManager = gameManager.zManager;
        zManager.OnBodyTracking += OnTrackHands;
        leftRb = leftHandGM.GetComponent<Rigidbody>();
        rightRb = rightHandGM.GetComponent<Rigidbody>();
        zDist = gameManager.GetCamDistance();

    }

    private void OnTrackHands(BodyTrackingFrame bodyTrackFrame)
    {
        if (bodyTrackFrame.bodyCount > 0)
        {
            //left hand = point 8, right hand = point 15


            //create vector from last from to current frame and scale by screen size
            //this gives the player a full range of motion on the screen space
            handLeftpos = prevLeftPos - bodyTrackFrame.detectedBodies[0].rawBodyData.keypoint[8] * Camera.main.orthographicSize;
            handRightpos = prevRightPos - bodyTrackFrame.detectedBodies[0].rawBodyData.keypoint[15] * Camera.main.orthographicSize;

            //store current frame for the next loop
            prevLeftPos = bodyTrackFrame.detectedBodies[0].rawBodyData.keypoint[8];
            prevRightPos = bodyTrackFrame.detectedBodies[0].rawBodyData.keypoint[15];
            
            //set distance from camera
            handLeftpos.z = zDist;
            handRightpos.z = zDist;
            //fix y reversal issue
            handLeftpos.y *= -1;
            handRightpos.y *= -1;

           // print("right hand at: " + handRightpos + " left hand at: " + handLeftpos);



            leftHandGM.transform.position = handLeftpos;
            rightHandGM.transform.position = handRightpos;
        }
    }

    /*private Vector3 ScaleToScreen(Vector3 vecToScale)
    {
        vecToScale.z = 0;
        vecToScale.y = vecToScale.y * Screen.height;
        vecToScale.x = vecToScale.x * Screen.width;
        return Camera.main.ScreenToWorldPoint( vecToScale);
    }*/

    private void FixedUpdate()
    {
    }
}
