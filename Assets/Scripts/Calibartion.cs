using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calibartion : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private AppManager appManager;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text buttonText;

    private Button _Button;
    
    public static Vector3 InhaleData { get; private set; } = Vector3.zero;
    public static Vector3 ExhaleData { get; private set; } = Vector3.zero;
    public static event Action CalibrationWasDone;
    public static event Action CalibrationStarted;

    private void Start()
    {
        _Button = GetComponent<Button>();

        CalibrationStarted += () => _Button.interactable = false;
        CalibrationWasDone += () =>
        {
            buttonText.SetText("Redo Calibration");
            _Button.interactable = true;
        };
    }

    public void Calibrate()
    {
        StartCoroutine(CalibrateCoroutine());
    }
    
    private IEnumerator CalibrateCoroutine()
    {
        CalibrationStarted?.Invoke();
        text.SetText("Make Inhale");
        yield return new WaitForSeconds(4f);
        InhaleData = input.Value;
        text.SetText("Make Exhale");
        yield return new WaitForSeconds(4f);
        ExhaleData = input.Value;
        text.SetText("CalibrationEnded");
        CalibrationWasDone?.Invoke();
    }

}
