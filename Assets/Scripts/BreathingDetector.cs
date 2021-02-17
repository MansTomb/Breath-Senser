using System;
using UnityEngine;

public class BreathingDetector
{
    public float TimeYThreshold;
    public float TimeXZThreshold;

    public float BufferXZ;
    public float BufferY;
    
    private bool BlockY;
    
    private float _TimeYincrement;
    private float _TimeYdecrement;
    
    private float _TimeX;
    private float _XCurrentValueChange;
    
    private float _TimeZ;
    private float _ZCurrentValueChange;
    private float _YCurrentValueChange;

    public event Action<Vector3> InhaleDetected;
    public event Action<Vector3> ExhaleDetected;
    public event Action BreatingStoped;

    public event Action OnBlockY;

    public BreathingDetector()
    {
        TimeYThreshold = 0.1f;
        TimeXZThreshold = 0.3f;
        BufferXZ = 0.1f;
        BufferY = 0.02f;
        BlockY = false;

        OnBlockY += StopGivingFeedback;
    }

    public void StopGivingFeedback() => BlockY = true;
    public void StartGivingFeedback() => BlockY = false;
    
    public void Detect(Vector3 acceleratorData, Vector3 delta)
    {
        DetectX(GetChangeInValue(acceleratorData.x, delta.x));
        DetectZ(GetChangeInValue(acceleratorData.z, delta.z));
        DetectY(acceleratorData, GetChangeInValue(acceleratorData.y, delta.y));
    }

    private void DetectY(Vector3 acceleratorData, float changeValue)
    {
        if (BlockY)
            return;
        
        _YCurrentValueChange = changeValue;
        if (InBufferRangeY())
        {
            _TimeYincrement = 0;
            _TimeYdecrement = 0;
            Debug.Log($"{_YCurrentValueChange} {BufferY}");
            BreatingStoped?.Invoke();
            return;
        }
        
        if (changeValue < 0)
        {
            _TimeYdecrement += Time.deltaTime;
            _TimeYincrement = 0;
            if (_TimeYdecrement >= TimeYThreshold)
                ExhaleDetected?.Invoke(acceleratorData);
            else
                BreatingStoped?.Invoke();
        }
        else if (changeValue > 0)
        {
            _TimeYincrement += Time.deltaTime;
            _TimeYdecrement = 0;
            if (_TimeYincrement >= TimeYThreshold)
                InhaleDetected?.Invoke(acceleratorData);
            else
                BreatingStoped?.Invoke();
        }
    }
    
    private void DetectX(float changeValue)
    {
        _XCurrentValueChange = changeValue;
        if (InBufferRangeX())
        {
            _TimeX = 0;
            return;
        }

        _TimeX += Time.deltaTime;
        if (_TimeX >= TimeXZThreshold)
            OnBlockY?.Invoke();
    }
    
    private void DetectZ(float changeValue)
    {
        _ZCurrentValueChange = changeValue;
        if (InBufferRangeZ())
        {
            _TimeZ = 0;
            return;
        }

        _TimeZ += Time.deltaTime;
        if (_TimeZ >= TimeXZThreshold)
            OnBlockY?.Invoke();
    }
    
    private bool InBufferRangeX() => Mathf.Abs(_XCurrentValueChange) < BufferXZ;
    private bool InBufferRangeY() => Mathf.Abs(_YCurrentValueChange) < BufferY;
    private bool InBufferRangeZ() => Mathf.Abs(_ZCurrentValueChange) < BufferXZ;
    
    private static float GetChangeInValue(float x, float xOld) => Mathf.Abs(x) - Mathf.Abs(xOld);
}

