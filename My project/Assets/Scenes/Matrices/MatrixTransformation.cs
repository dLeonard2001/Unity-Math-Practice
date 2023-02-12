using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTransformation : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Vector3 t_Vector3;
    private bool isResetting;
    private Vector3 startPos;
    private Matrix4x4 newMatrix;
    
    // translation for a matrix4x4
    // where {tx, ty, tz} are our elements to change in our matrix4x4
        // | 1  0  0  tx |
        // | 0  1  0  ty |
        // | 0  0  1  tz |
        // | 0  0  0  1  |
    
    
    // cache some data for later
    void Start()
    {
        _transform = GetComponent<Transform>();
        startPos = _transform.position;
        
        newMatrix = _transform.localToWorldMatrix;
    }

    // translate our object, if we aren't resetting the object
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isResetting)
            StartCoroutine(ResetPosition());
        
        if (!isResetting)
        {
            Translate(t_Vector3.x, t_Vector3.y, t_Vector3.z);

            _transform.position += newMatrix.MultiplyPoint3x4(Vector3.zero);
        }

        // Debug.Log($"Delta Time: {Time.deltaTime}");
        // Debug.Log($"FixedDelta Time: {Time.fixedDeltaTime}");
        
    }

    // need to finish
    IEnumerator ResetPosition()
    {
        isResetting = true;
        Vector3 currentPos = _transform.position;
        Vector3 moveDirection = startPos - currentPos;
        
        Debug.Log(moveDirection);
        
        Debug.Break();
        
        while (_transform.position != startPos)
        {
            
            
            yield return null;
        }

        isResetting = false;
    }
    
    private void Translate(float x, float y, float z)
    {
        newMatrix.m03 = x * Time.deltaTime;
        newMatrix.m13 = y * Time.deltaTime;
        newMatrix.m23 = z * Time.deltaTime;
    }
}
