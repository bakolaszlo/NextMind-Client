using NextMind.NeuroTags;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NeuroTag))]
public class NeuroTagListener : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI overlayValue;

    protected Transform visualObject;
    public void Awake()
    {
        if(overlayValue == null )//&& Constants.IsDebugging)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            overlayValue = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        this.OnSetup();
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateNeuroTag();
    }

    public virtual void OnSetup()
    {
        if(overlayValue == null)
        {
            Debug.LogError("Error finding overlay value");
        }

        visualObject = transform.Find("Visual");
    }

    public virtual void UpdateNeuroTag()
    {

    }

    public virtual void OnConfidenceChanged(float value)
    {
        // if (Constants.IsDebugging)
        {
            overlayValue.text = $"{value}";
        }
    }

    public virtual void OnTriggered()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnTriggered executed";
    }

    public virtual void OnMaintained()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnMaintained executed";
    }

    public virtual void OnReleased()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnReleased executed";
    }
}
