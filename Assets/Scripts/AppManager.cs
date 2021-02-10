using System;
using TMPro;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private History history;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private AudioClip inhale;
    [SerializeField] private AudioClip exhale;

    private AudioSource _Source;
    private BreathingDetector _Detector;
    private TMP_Text _Text;

    private Vector3 _InhaleData;
    private Vector3 _ExhaleData;

    private bool _FirstExhaleSkipped;

    private void Start()
    {
        _FirstExhaleSkipped = false;
        _Source = GetComponent<AudioSource>();
        _Text = GetComponent<TMP_Text>();
        
        _Detector = new BreathingDetector();
        
        Calibartion.CalibrationWasDone += StartDetecting;
        Calibartion.CalibrationStarted += StopDetecting;
    }

    private void StartDetecting()
    {
        _FirstExhaleSkipped = false;
        
        _InhaleData = Vector3.zero;
        _ExhaleData = Vector3.zero;

        input.ValueChanged += ValueChanged;
        
        _Detector.InhaleDetected += DetectedPhaseInhale;
        _Detector.ExhaleDetected += DetectedPhaseExhale;
    }

    private void StopDetecting()
    {
        input.ValueChanged -= ValueChanged;
        
        _Detector.InhaleDetected -= DetectedPhaseInhale;
        _Detector.ExhaleDetected -= DetectedPhaseExhale;
    }

    private void ValueChanged(Vector3 value)
    {
        _Detector.Detect(value);
        _Text.SetText($"{value}");
    }

    private void DetectedPhaseInhale(Vector3 when) => DetectedPhase(when, inhale, Phase.Inhale);
    private void DetectedPhaseExhale(Vector3 when) => DetectedPhase(when, exhale, Phase.Exhale);

    private void DetectedPhase(Vector3 when, AudioClip clip, Phase phase)
    {
        if (_FirstExhaleSkipped == false && phase == Phase.Exhale)
        {
            _FirstExhaleSkipped = true;
            return;
        }
        
        _Source.PlayOneShot(clip);
        phaseText.SetText(phase + " Phase");
        switch (phase)
        {
            case Phase.Inhale:
                _InhaleData = when;
                break;
            case Phase.Exhale:
                _ExhaleData = when;
                break;
        }

        if (!_InhaleData.Equals(Vector3.zero) && !_ExhaleData.Equals(Vector3.zero))
        {
            history.AddCycleToHistory(_InhaleData, _ExhaleData);
            _InhaleData = Vector3.zero;
            _ExhaleData = Vector3.zero;
        }
    }
}
