using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    //ZED ASSETS
    private ZEDBodyTrackingManager btm;
    private ZEDManager zManager;

    Vector3 rightHandPos, leftHandPos;
    Rigidbody rbL, rbR;

    //VISUALIZATION GAME OBJECTS
    private GameObject leftHandGM, rightHandGM;
    private Transform leftTransform, rightTransform;

    private float zDist = 5f;

    //MOTION ADJUSTEMENTS
    [Header(" physical range of motion")]
    [Tooltip("comfortable range of motion on the x-axis in meters")]
    [SerializeField] float physicalXRange = 0.6f; 
    [Tooltip("comfortable range of motion on the y-axis in meters")]
    [SerializeField] float physicalYRange = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        leftHandGM = gameManager.handL;
        rightHandGM = gameManager.handR;
        btm = gameManager.btm;
        zManager = gameManager.zManager;
        zManager.OnBodyTracking += OnTrackHands;
        leftTransform = leftHandGM.transform;
        rightTransform = rightHandGM.transform;
        zDist = gameManager.GetCamDistance();
        rbR = rightHandGM.GetComponent<Rigidbody>();
        rbL = leftHandGM.GetComponent <Rigidbody>();

    }
    /// <summary>
    /// tracks hand movement and scales it to match screen space rather than unity world or
    /// real world space to allow more comfortable movements and gesture control
    /// </summary>
    /// <param name="bodyTrackFrame"></param>

    private void OnTrackHands(BodyTrackingFrame bodyTrackFrame)
    {
        if (bodyTrackFrame.bodyCount > 0)
        {
            var body = bodyTrackFrame.detectedBodies[0];
            var keypoints = body.rawBodyData.keypoint;

            // Get Unity world positions
            Vector3 leftWorld = zManager.transform.TransformPoint(keypoints[8]);
            Vector3 rightWorld = zManager.transform.TransformPoint(keypoints[15]);
            Vector3 torsoWorld = zManager.transform.TransformPoint(keypoints[1]);

            // Get relative hand positions (offset from torso)
            Vector3 leftOffset = leftWorld - torsoWorld;
            Vector3 rightOffset = rightWorld - torsoWorld;


           

            float halfYRange = physicalYRange * 0.5f;
            float halfXRange = physicalXRange * 0.5f;

            //normalize it realative to screen space (treating it as points between 0 and 1)
            //this allows it to scale up with screen size accurately
            Vector2 leftNorm = new Vector2(
                Mathf.Clamp01((leftOffset.x + halfXRange) / physicalXRange),
                Mathf.Clamp01((leftOffset.y + halfYRange) / physicalYRange)
            );

            Vector2 rightNorm = new Vector2(
                Mathf.Clamp01((rightOffset.x + halfXRange) / physicalXRange),
                Mathf.Clamp01((rightOffset.y + halfYRange) / physicalYRange)
            );

            // Flips x to prevent mirroring
            leftNorm.x = 1f - leftNorm.x;
            rightNorm.x = 1f - rightNorm.x;


            // Convert to screen position by scaling with screen size, making all motion relative to the screen rather than world coordinates.
            //this allows for more comfortable and predictable range of motion
            Vector3 leftHand = new Vector3(leftNorm.x * Screen.width, leftNorm.y * Screen.height, zDist);
            Vector3 rightHand = new Vector3(rightNorm.x * Screen.width, rightNorm.y * Screen.height, zDist);

            // return to world space
            leftHandPos = Camera.main.ScreenToWorldPoint(leftHand);
            rightHandPos = Camera.main.ScreenToWorldPoint(rightHand);
        }
    }

    private void Update()
    {
        //update positions usinng rigid body to preserve colissions
        rbL.position = leftHandPos;
        rbR.position = rightHandPos;
        
    }

}
