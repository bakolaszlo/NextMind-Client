using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private NeuroManagerListener _neuroTagListener;
    private void Awake()
    {
        _neuroTagListener = GameObject.Find("NeuroManager").GetComponent<NeuroManagerListener>();
    }
    public void Triggered()
    {
        string state = "Allowed";
        if (!_neuroTagListener.IsAllowed)
        {
            state = "Denied";
        }
        _text.text = $"Current Access is: {state}";
    }
}
