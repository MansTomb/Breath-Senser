using System;
using UnityEngine;

public class BreathingDetector
{
    public float TimeYThreshold;
    public float TimeXZThreshold;

    public float BufferXZ;
    
    public bool BlockY;
    
    private float _TimeYincrement;
    private float _TimeYdecrement;
    
    private float _TimeXincrement;
    private float _TimeXdecrement;
    
    private float _TimeZincrement;
    private float _TimeZdecrement;

    public event Action<Vector3> InhaleDetected;
    public event Action<Vector3> ExhaleDetected;

    private event Action OnBlockY;

    public BreathingDetector()
    {
        TimeYThreshold = 1f;
        TimeXZThreshold = 2f;
        BufferXZ = 1f;
        BlockY = false;

        OnBlockY += () => BlockY = true;
    }
    public void Detect(Vector3 acceleratorData, Vector3 delta)
    {
        var changeValue = GetChangeInValue(acceleratorData.y, delta.y);
        DetectX(acceleratorData, changeValue);
        DetectZ(acceleratorData, changeValue);
        DetectY(acceleratorData, changeValue);
    }

    private void DetectY(Vector3 acceleratorData, float changeValue)
    {
        if (BlockY)
            return;
        
        if (changeValue < 0)
        {
            _TimeYdecrement += Time.deltaTime;
            _TimeYincrement = 0;
            if (_TimeYdecrement >= TimeYThreshold)
                ExhaleDetected?.Invoke(acceleratorData);
        }
        else if (changeValue > 0)
        {
            _TimeYincrement += Time.deltaTime;
            _TimeYdecrement = 0;
            if (_TimeYincrement >= TimeYThreshold)
                InhaleDetected?.Invoke(acceleratorData);
        }
    }
    
    private void DetectX(Vector3 acceleratorData, float changeValue)
    {
        if (InBufferRangeX(acceleratorData.x))
        {
            _TimeXdecrement = 0;
            _TimeXincrement = 0;
            return;
        }
        
        if (changeValue < 0)
        {
            _TimeXdecrement += Time.deltaTime;
            _TimeXincrement = 0;
            if (_TimeXdecrement >= TimeXZThreshold)
                OnBlockY?.Invoke();
        }
        else if (changeValue > 0)
        {
            _TimeXincrement += Time.deltaTime;
            _TimeXdecrement = 0;
            if (_TimeXincrement >= TimeXZThreshold)
                OnBlockY?.Invoke();
        }
    }
    
    private void DetectZ(Vector3 acceleratorData, float changeValue)
    {
        if (InBufferRangeZ(acceleratorData.z))
        {
            _TimeZdecrement = 0;
            _TimeZincrement = 0;
            return;
        }

        if (changeValue < 0)
        {
            _TimeZdecrement += Time.deltaTime;
            _TimeZincrement = 0;
            if (_TimeZdecrement >= TimeXZThreshold)
                OnBlockY?.Invoke();
        }
        else if (changeValue > 0)
        {
            _TimeZincrement += Time.deltaTime;
            _TimeZdecrement = 0;
            if (_TimeZincrement >= TimeXZThreshold)
                OnBlockY?.Invoke();
        }
    }

    private bool InBufferRangeX(float value)
    {
        var min = Mathf.Min(Calibartion.ExhaleData.x, Calibartion.InhaleData.x);
        var max = Mathf.Max(Calibartion.ExhaleData.x, Calibartion.InhaleData.x);
        return value > min - BufferXZ && value < max + BufferXZ;
    }
    private bool InBufferRangeZ(float value)
    {
        var min = Mathf.Min(Calibartion.ExhaleData.z, Calibartion.InhaleData.z);
        var max = Mathf.Max(Calibartion.ExhaleData.z, Calibartion.InhaleData.z);
        return value > min - BufferXZ && value < max + BufferXZ;
    }
    private static float GetChangeInValue(float x, float xOld) => Mathf.Abs(x) - Mathf.Abs(xOld);
}

