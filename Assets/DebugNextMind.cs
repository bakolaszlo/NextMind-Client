using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugNextMind : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textmesh;
    private void Awake()
    {
        textmesh.text = $"{Debug.isDebugBuild} : {Application.isPlaying}";
        if (!Debug.isDebugBuild && Application.isPlaying)
        {
            enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
