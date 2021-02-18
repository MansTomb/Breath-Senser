using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const float AccelerometerUpdateInterval = 1.0f / 60.0f;
    private const float Precision = 100f;
    
    private float _LowPassKernelWidthInSeconds = .5f;

    private float _LowPassFilterFactor;
    private Vector3 _LowPassValueOld = Vector3.zero;
    private Vector3 _LowPassValue = Vector3.zero;

    public event Action<Vector3, Vector3> ValueChanged;

    public void SetLowPassKernelWidth(float value)
    {
        _LowPassKernelWidthInSeconds = value;
        _LowPassFilterFactor = AccelerometerUpdateInterval / _LowPassKernelWidthInSeconds;
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        
        _LowPassFilterFactor = AccelerometerUpdateInterval / _LowPassKernelWidthInSeconds;
        _LowPassValue = GetAccelerometerValue();
    }

    void Update()
    {
        _LowPassValue = LowPassFilterAccelerometer(_LowPassValue);
    }

    private Vector3 LowPassFilterAccelerometer(Vector3 prevValue)
    {
        Vector3 newValue = Vector3.Lerp(prevValue, GetAccelerometerValue(), _LowPassFilterFactor);
        ValueChanged?.Invoke(newValue * Precision, _LowPassValueOld);
        _LowPassValueOld = (newValue * Precision);
        return newValue;
    }

    private Vector3 GetAccelerometerValue()
    {
        Vector3 acc = Vector3.zero;
        float period = 0.0f;

        foreach(AccelerationEvent evnt in Input.accelerationEvents)
        {
            acc += evnt.acceleration * evnt.deltaTime;
            period += evnt.deltaTime;
        }
        if (period > 0)
        {
            acc *= 1.0f / period;
        }
        return acc;
    }

    private Vector3 CompensateGravity(Vector3 value)
    {
        Quaternion q = Input.gyro.attitude;
        Vector3 gravity = new Vector3(0f, 0f, -.981f * Precision);

        Vector3 rotated = q * value;

        return rotated - gravity;
    }
}
