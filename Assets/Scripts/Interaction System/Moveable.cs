using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{

    private Vector3 pointA;
    private Vector3 pointB;

    PathManager pathManager;

    //private Vector3 lastPos;
    
    Rigidbody rb;
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
        rb.position = FindNearestPointOnLine(pointA, pointB, rb.position);
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
