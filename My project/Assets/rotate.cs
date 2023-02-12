using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotateSpeed;
    public bool isParent;
    public UnitController script;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isParent)
        {
            transform.Rotate(new Vector3(0f, rotateSpeed * Time.deltaTime, 0f ));
        }
        else
        {
            float angle = Time.time * rotateSpeed;
            float x = Mathf.Cos(angle) * 5f;
            float z = Mathf.Sin(angle) * 5f;
            transform.localPosition = new Vector3(x, 0, z);
        }
        
        
    }
}
