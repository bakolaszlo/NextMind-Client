using NextMind.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public void OnButtonPress()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(0);
    }
}
