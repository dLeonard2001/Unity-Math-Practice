using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MatrixTransformation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float resetDuration = 1.0f;
    private Vector3 t_Vector3;
    private Quaternion xRotation;
    private Quaternion yRotation;
    private Quaternion zRotation;
    

    private float moveElapsedTime;
    private float resetElapsedTime;
    private Vector3 startPos;
    private Matrix4x4 newMatrix;
    
    // ===================================== NOTES =====================================
        // General order for matrix transformations are
            // 1. scaling
            // 2. rotations
            // 3. translation
            // there is a specific order because
            // the order in which the transformations are done can affect the result
    // ===================================== NOTES =====================================
    
    // cache some data for later
    void Start()
    {
        _transform = GetComponent<Transform>();
        
        startPos = _transform.position;

        
    }

    public void Execute()
    {
        newMatrix = _transform.localToWorldMatrix;
        
        //newMatrix *= makeRotationZ(45f);
        newMatrix *= makeRotationY(45f);
        //newMatrix *= makeRotationX(45f);
        newMatrix *= Translate(t_Vector3);

        _transform.rotation = Quaternion.LookRotation(newMatrix.GetColumn(2), newMatrix.GetColumn(1));
        _transform.position = newMatrix.MultiplyPoint3x4(Vector3.zero);
    }

    public void Translate()
    {
        newMatrix = _transform.localToWorldMatrix;

        _transform.position = newMatrix.MultiplyPoint3x4(Vector3.zero);
    }

    public void Rotate()
    {
        newMatrix = _transform.localToWorldMatrix;
        
        
    }
    
    #region MakeRotation
    
    // here is how we rotate a matrix on the x-axis
        // | 1    0           0           0 |
        // | 0    cos(theta)    -sin(theta)   0 |
        // | 0    sin(theta)    cos(theta)    0 |
        // | 0    0           0           1 |
    // WHEN ROTATING YOU MUST APPLY A SPECIFIC ORDER
    // apply the x then y then z
    // if you don't apply rotations in this order, you will get different results than you think

    private Matrix4x4 makeRotationX(float degrees)
    {
        float s = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float c = Mathf.Cos(degrees * Mathf.Deg2Rad);
        Matrix4x4 matrix = Matrix4x4.identity;
        
        matrix.m11 = c;
        matrix.m12 = -s;
        matrix.m21 = s;
        matrix.m22 = c;

        return matrix;
    }
    
    // here is how we rotate a matrix on the y-axis
        // | cos(theta)   0   sin(theta)   0 |
        // | 0            1   0              0 |
        // | -sin(theta)  0   cos(theta)   0 |
        // | 0            0   0              1 |
    private Matrix4x4 makeRotationY(float degrees)
    {
        float s = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float c = Mathf.Cos(degrees * Mathf.Deg2Rad);
        Matrix4x4 matrix = Matrix4x4.identity;

        matrix.m00 = c;
        matrix.m02 = s;
        matrix.m20 = -s;
        matrix.m22 = c;

        return matrix;
    }
    
    // here is how we rotate a matrix on the z-axis
        // | cos(theta) -sin(theta)  0   0 |
        // | sin(theta)  cos(theta)  0   0 |
        // | 0           0           1   0 |
        // | 0           0           0   1 |
        private Matrix4x4 makeRotationZ(float degrees)
        {
            float s = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float c = Mathf.Cos(degrees * Mathf.Deg2Rad);
            Matrix4x4 matrix = Matrix4x4.identity;

            matrix.m00 = c;
            matrix.m01 = -s;
            matrix.m10 = s;
            matrix.m12 = c;

            return matrix;
        }

        #endregion
        
    #region HandlePlayerInput

        public void SetX(TMP_InputField info)
        {
        
            t_Vector3.x = float.Parse(info.text);
        }
    
        public void SetY(TMP_InputField info)
        {
        
            t_Vector3.y = float.Parse(info.text);
        }
    
        public void SetZ(TMP_InputField info)
        {
        
            t_Vector3.z = float.Parse(info.text);
        }

        #endregion
        

    // translation for a matrix4x4
        // where {tx, ty, tz} are our elements to change in our matrix4x4
            // | 1  0  0  tx |
            // | 0  1  0  ty |
            // | 0  0  1  tz |
            // | 0  0  0  1  |
            
    // we perform translation this way because m03, m13, m23 are the x y z coordinates in world space
        // and m33 represents a scaling factor
    
    // translation transformation
    private Matrix4x4 Translate(Vector3 direction)
    {
        moveElapsedTime += 0.5f * Time.deltaTime;
        float t = moveElapsedTime / moveDuration;
        Matrix4x4 m = Matrix4x4.identity;

        m.m03 = direction.x;
        m.m13 = direction.y;
        m.m23 = direction.z;

        return m;
    }
    
    // reset our object to its original form
        // the scope to also add in ResetRotation and ResetScale
    private void ResetPosition()
    {
        resetElapsedTime += 0.5f * Time.deltaTime;
        float t = resetElapsedTime / resetDuration;

        newMatrix.m03 = Mathf.Lerp(_transform.position.x, startPos.x, Mathf.SmoothStep(0, 1, t));
        newMatrix.m13 = Mathf.Lerp(_transform.position.y, startPos.y, Mathf.SmoothStep(0, 1, t));
        newMatrix.m23 = Mathf.Lerp(_transform.position.z, startPos.z, Mathf.SmoothStep(0, 1, t));

        _transform.position = newMatrix.MultiplyPoint3x4(Vector3.zero);
    }
    

    
}
