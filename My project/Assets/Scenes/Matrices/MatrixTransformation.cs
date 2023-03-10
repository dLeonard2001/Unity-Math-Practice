using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MatrixTransformation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float actionDuration = 1.0f;
    private Vector3 translationVector;
    private Vector3 scaleVector;
    private Vector3 originalScale;

    private Matrix4x4 newMatrix;

    private bool cr_active;

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

        scaleVector = Vector3.one;
        originalScale = _transform.localScale;
    }

    
    // a poor way to determine if we have won/beat the current level
    private void Update()
    {
        if ((targetTransform.position - _transform.position).magnitude < 0.5f && !cr_active)
        {
            StartCoroutine(Win());
        }
    }

    private IEnumerator Win()
    {

        cr_active = true;
        _animator.CrossFade("winAnimation", 0f, 0);

        yield return new WaitForSeconds(3f);

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        cr_active = false;
    }
    
    // reset our player's transformations to the beginning of the level
    public void ResetPlayer()
    {
        StopAllCoroutines();
        
        newMatrix = Matrix4x4.identity;

        _transform.localScale = originalScale;
        _transform.localRotation = Quaternion.LookRotation(newMatrix.GetColumn(2), newMatrix.GetColumn(1));
        _transform.position = newMatrix.GetPosition();
    }
    
    // functions to smoothly interpolate the player's input
    private IEnumerator SmoothScale(Vector3 scaleVector)
    {
        float resetDuration = 0f;
        newMatrix = _transform.localToWorldMatrix;

        Matrix4x4 m = newMatrix * Scale(scaleVector);
        Vector3 newScaleVector = m.lossyScale;

        float t = resetDuration / actionDuration;
        while (_transform.localScale != m.lossyScale)
        {
            _transform.localScale = new Vector3(Mathf.Lerp(_transform.localScale.x, newScaleVector.x, t),
                _transform.localScale.y, _transform.localScale.z);
            
            _transform.localScale = new Vector3(_transform.localScale.x,
                Mathf.Lerp(_transform.localScale.y, newScaleVector.y, t), _transform.localScale.z);
            
            _transform.localScale = new Vector3(_transform.localScale.x,
                _transform.localScale.y, Mathf.Lerp(_transform.localScale.z, newScaleVector.z, t));

            resetDuration += 0.1f * Time.deltaTime;
            t = resetDuration / actionDuration;

            yield return null;
        }
    }

    private IEnumerator SmoothTranslate(Vector3 translateVector)
    {
        float resetDuration = 0f;
        newMatrix = _transform.localToWorldMatrix;

        Matrix4x4 m = newMatrix * Translate(translateVector);
        Vector3 newPositionVec = m.GetPosition();

        float t = resetDuration / actionDuration;
        while (_transform.position != translateVector)
        {
            _transform.position = new Vector3(Mathf.Lerp(_transform.position.x, newPositionVec.x, t),
                _transform.position.y, _transform.position.z);
            
            _transform.position = new Vector3(_transform.position.x,
                Mathf.Lerp(_transform.position.y, newPositionVec.y, t), _transform.position.z);
            
            _transform.position = new Vector3(_transform.position.x,
                _transform.position.y, Mathf.Lerp(_transform.position.z, newPositionVec.z, t));
            
            
            resetDuration += 0.1f * Time.deltaTime;
            t = resetDuration / actionDuration;

            yield return null;
        }
        
    }

    #region OrderOfOperations
    
    // apply our scale input
    public void ApplyScale()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothScale(scaleVector));
    }

    // apply x rotation input
    public void ApplyRotationX(TMP_InputField input)
    {
        if(input.text.Length == 0) return;
        
        newMatrix = _transform.localToWorldMatrix;
        newMatrix *= makeRotationX(float.Parse(input.text));

        _transform.rotation = Quaternion.LookRotation(newMatrix.GetColumn(2), newMatrix.GetColumn(1));
    }
    
    // apply y rotation input
    public void ApplyRotationY(TMP_InputField input)
    {
        if(input.text.Length == 0) return;
        
        newMatrix = _transform.localToWorldMatrix;
        newMatrix *= makeRotationY(float.Parse(input.text));

        _transform.rotation = Quaternion.LookRotation(newMatrix.GetColumn(2), newMatrix.GetColumn(1));
    }
    
    // apply z rotation input
    public void ApplyRotationZ(TMP_InputField input)
    {
        if(input.text.Length == 0) return;
        
        newMatrix = _transform.localToWorldMatrix;
        newMatrix *= makeRotationZ(float.Parse(input.text));

        _transform.rotation = Quaternion.LookRotation(newMatrix.GetColumn(2), newMatrix.GetColumn(1));
    }
    
    // apply translation input
    public void ApplyTranslation()
    {
        StartCoroutine(SmoothTranslate(translationVector));
    }
    
    #endregion
    
    #region makeScaleTransformation

    // scale transformation
        // | sx  0   0   0 |
        // | 0   sy  0   0 |
        // | 0   0   sz  0 |
        // | 0   0   0   1 |
    private Matrix4x4 Scale(Vector3 scaleVec)
    {
        Matrix4x4 m = Matrix4x4.identity;

        m.m00 = scaleVec.x;
        m.m11 = scaleVec.y;
        m.m22 = scaleVec.z;

        return m;
    }
    
    public void SetXScale(TMP_InputField info)
    {
        if (info.text.Length == 0)
            scaleVector.x = 1;
        else
            scaleVector.x = float.Parse(info.text);
    }

    public void SetYScale(TMP_InputField info)
    {

        if (info.text.Length == 0)
            scaleVector.y = 1;
        else
            scaleVector.y = float.Parse(info.text);
    }

    public void SetZScale(TMP_InputField info)
    {
        if (info.text.Length == 0)
            scaleVector.z = 1;
        else
            scaleVector.z = float.Parse(info.text);
    }
    
    #endregion
    
    #region MakeRotationTransformation

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
        float radians = degrees * Mathf.Deg2Rad;
        float s = Mathf.Sin(radians);
        float c = Mathf.Cos(radians);
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
        float radians = degrees * Mathf.Deg2Rad;
        float s = Mathf.Sin(radians);
        float c = Mathf.Cos(radians);
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
    
    #region makeTranslationTransformation

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
        Matrix4x4 m = Matrix4x4.identity;

        m.m03 = direction.x;
        m.m13 = direction.y;
        m.m23 = direction.z;

        // Debug.Log(newMatrix.MultiplyPoint3x4(Vector3.zero));
        // _transform.position = newMatrix.MultiplyPoint3x4(Vector3.zero);
        return m;
    }
    
    public void SetXPosition(TMP_InputField info)
    {
        if (info.text.Length == 0)
            translationVector.x = 0;
        else
            translationVector.x = float.Parse(info.text);
    }

    public void SetYPosition(TMP_InputField info)
    {

        if (info.text.Length == 0)
            translationVector.y = 0;
        else
            translationVector.y = float.Parse(info.text);
    }

    public void SetZPosition(TMP_InputField info)
    {

        if (info.text.Length == 0)
            translationVector.z = 0;
        else
            translationVector.z = float.Parse(info.text);
    }
    
    #endregion

}
