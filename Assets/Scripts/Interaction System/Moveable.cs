using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pathManager = GetComponent<PathManager>();
        List<Waypoint> points = pathManager.GetPath();
        pointA = points[0].GetPos();
        pointB = points[1].GetPos();

       
    }

    private void Update()
    {
      //  rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
        //print(rb.velocity);
        if (rb.position != prevPos)
        {
            rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
        }
        prevPos = rb.position;
        //zero out velocity so it doesnt keep moving
        rb.velocity = Vector3.zero;
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
