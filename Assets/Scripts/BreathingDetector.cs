using System;
using UnityEngine;

public class BreathingDetector
{
    public event Action<Vector3> InhaleDetected;
    public event Action<Vector3> ExhaleDetected;

    private bool _InhaleWasDetected;
    private bool _ExhaleWasDetected;

    private float _Precision = 2f;

    public BreathingDetector()
    {
        _ExhaleWasDetected = false;
        _InhaleWasDetected = false;

        Calibartion.CalibrationWasDone += () => _Precision = 2f;
    }
    public void Detect(Vector3 acceleratorData)
    {
        if (InRangeInhale(acceleratorData) && InRangeExhale(acceleratorData))
        {
            _Precision -= 0.1f;
            return;
        }
        if (InRangeInhale(acceleratorData) && _InhaleWasDetected == false)
        {
            InhaleDetected?.Invoke(acceleratorData);
            _InhaleWasDetected = true;
            _ExhaleWasDetected = false;
        }
        else if (InRangeExhale(acceleratorData) && _ExhaleWasDetected == false)
        {
            ExhaleDetected?.Invoke(acceleratorData);
            _ExhaleWasDetected = true;
            _InhaleWasDetected = false;
        }
    }

    public bool InRangeInhale(Vector3 value)
    {
        return Mathf.Abs(Calibartion.InhaleData.x - value.x) < _Precision &&
               Mathf.Abs(Calibartion.InhaleData.y - value.y) < _Precision &&
               Mathf.Abs(Calibartion.InhaleData.z - value.z) < _Precision;
    }
    
    public bool InRangeExhale(Vector3 value)
    {
        return Mathf.Abs(Calibartion.ExhaleData.x - value.x) < _Precision &&
               Mathf.Abs(Calibartion.ExhaleData.y - value.y) < _Precision &&
               Mathf.Abs(Calibartion.ExhaleData.z - value.z) < _Precision;
    }
}

