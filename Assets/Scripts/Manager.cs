using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    private void Awake()
    {
        Application.targetFrameRate = 60;
        // Sync the frame rate to the screen's refresh rate
        QualitySettings.vSyncCount = 1;
        DontDestroyOnLoad(this);
        foreach (var item in gameObjects)
        {
            DontDestroyOnLoad(item);
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
