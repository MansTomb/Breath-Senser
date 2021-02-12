using System;
using TMPro;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private AudioClip inhale;
    [SerializeField] private AudioClip exhale;

    private AudioSource _Source;
    private BreathingDetector _Detector;
    private TMP_Text _Text;
    
    private void Start()
    {
        _Source = GetComponent<AudioSource>();
        _Text = GetComponent<TMP_Text>();
        
        _Detector = new BreathingDetector();
        
        Calibartion.CalibrationWasDone += StartDetecting;
        Calibartion.CalibrationStarted += StopDetecting;
    }

    public void SetDetectorYThreshold(float value) => _Detector.TimeYThreshold = value;
    public void SetDetectorXZThreshold(float value) => _Detector.TimeXZThreshold = value;
    public void SetDetectorXZBuffer(float value) => _Detector.BufferXZ = value;
    public void UnblockYAxis() => _Detector.BlockY = false;

    private void StartDetecting()
    {
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

    private void ValueChanged(Vector3 value, Vector3 delta)
    {
        _Detector.Detect(value, delta);
        _Text.SetText($"{value}");
    }

    private void DetectedPhaseInhale(Vector3 when) => DetectedPhase(when, inhale, Phase.Inhale);
    private void DetectedPhaseExhale(Vector3 when) => DetectedPhase(when, exhale, Phase.Exhale);

    private void DetectedPhase(Vector3 when, AudioClip clip, Phase phase)
    {
        phaseText.SetText(phase + " Phase");
        _Source.clip = clip;
        if (_Source.clip != clip)
        {
            _Source.Stop();
            return;
        }
        if (_Source.isPlaying == false)
            _Source.Play();
    }
}
