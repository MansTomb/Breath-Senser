using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class History : MonoBehaviour
{
    [SerializeField] private GameObject historyElementPrefab;
    [SerializeField] private GameObject contentObject;
    
    private List<HistoryElement> _History;

    private void Awake()
    {
        _History = new List<HistoryElement>();
        
        Calibartion.CalibrationWasDone += () =>
        {
            foreach (var element in _History)
            {
                Destroy(element.gameObject);
            }
            _History.Clear();
        };
    }

    public void AddCycleToHistory(Vector3 inhale, Vector3 exhale)
    {
        var element = Instantiate(historyElementPrefab, contentObject.transform, false);
        _History.Add(element.GetComponent<HistoryElement>());
        element.GetComponent<HistoryElement>().SetData(inhale, exhale);
    }
}