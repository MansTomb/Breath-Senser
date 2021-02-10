using TMPro;
using UnityEngine;

public class HistoryElement : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetData(Vector3 inhale, Vector3 exhale)
    {
        text.SetText($"In: {inhale}, Ex: {exhale}");
    }
}