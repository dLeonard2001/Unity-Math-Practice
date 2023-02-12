using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float rotateSpeed;
    private float degrees;
    private float radians;

    // for our case
    // x = sin
    // z = cos

    // Start is called before the first frame update
    void Start()
    {
        // same as saying degrees * pi/180
        degrees = 0;
        radians = degrees * Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void Update()
    {
        degrees += rotateSpeed * Time.deltaTime;
        if (degrees > 360)
            degrees = 0;
        
        radians = degrees * Mathf.Deg2Rad;
    }

    private void OnDrawGizmos()
    {
        
        // our dynamic angle
        Gizmos.color = Color.red;
        Gizmos.DrawLine(center.position, new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * 5f);
        
        // center line
        // Gizmos.DrawLine(center.position, new Vector3(Mathf.Cos(180), 0f, 0f) * 9f);
        
        // our static angle
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center.position, new Vector3(Mathf.Cos(0), 0f, 0f) * 5f);

        // dot product in unity  can return -1, 0, 1
            // if -1
                // the vectors are facing away from each other
                // -1 means they are completely looking in the opposite direction
            // if 0
                // this means the vectors are perpendicular to one another
            // if 1 
                // this means the vectors are completely facing the same direction
        float dotProduct = Vector3.Dot(new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f),new Vector3(Mathf.Cos(0), 0f, 0f));
            
        // our dot product
        Debug.Log($"Dot Product: {dotProduct}");
        
        // to get the angle of our dot product
            // multiple the arc-cosine of our dot product BY 180/pi
        Debug.Log($"Angle of our dot product: {Mathf.Acos(dotProduct) * Mathf.Rad2Deg}");
    }
}
