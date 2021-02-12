using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Parameter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void OnValueChanged(float value) => text.SetText(value.ToString());
}
