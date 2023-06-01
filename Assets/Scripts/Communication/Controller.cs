using Assets.Scripts;
using Assets.Scripts.Communication;
using Assets.Scripts.Communication.Models;
using Assets.Scripts.Crypto;
using Newtonsoft.Json;
using NextMind;
using NextMind.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Controller : MonoBehaviour
{
    [SerializeField]
    NeuroManager neuroManager;
    Device _device;
    float[] _contacts = new float[8];
    List<SensorData> sensorDataQueue = new();
    float updateInterval = 99999f;
    [SerializeField] float elapsedTime;
    [SerializeField] string elapsedTimeText;

    public void Login()
    {
        _device = neuroManager.ConnectedDevices[0];
        StartCoroutine(Login(_device.Name, _device.ID.ToString()));
    }

    public void SendTrigger()
    {
        // Create a JSON object containing the login credentials
        var payload = new { location = new int[] { 0, 0, 0 } };
        string jsonData = JsonConvert.SerializeObject(payload);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        // Create the UnityWebRequest object
        UnityWebRequest www = new UnityWebRequest(Constants.API_URL + Constants.TRIGGER_ENDPOINT, "POST");
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Set the request headers
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", $"bearer {PlayerPrefs.GetString("jwtToken")}");
        // Send the request
        www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            // Debug.LogError("TURN OFF"); // TODO
        }
        else
        {
            Debug.Log("Trigger data sent.");
        }
        // Explicitly dispose of the UploadHandlerRaw to avoid memory leaks
        www.uploadHandler.Dispose();
    }
    private IEnumerator Login(string username, string password)
    {
        // Create a JSON object containing the login credentials
        UpdateContacts();
        var loginData = new LoginModel()
        {
            Username = username,
            Password = password,
            SensorData = _contacts
        };
        string jsonData = JsonConvert.SerializeObject(loginData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        // Create the UnityWebRequest object
        UnityWebRequest www = new UnityWebRequest(Constants.API_URL + Constants.LOGIN_ENDPOINT, "POST");
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Set the request headers
        www.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Get the response data
            string responseData = www.downloadHandler.text;
            Debug.Log(responseData);
            // parse the response data and get the JWT token from the response
            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData);
            string jwtToken = responseJson["token"];
            float updateIntervalRes = float.Parse(responseJson["UpdateInterval"]);
            // Save the JWT token for future requests
            PlayerPrefs.SetString("jwtToken", jwtToken);
            updateInterval = updateIntervalRes;
            StartCoroutine(UpdateData());
            StartCoroutine(SendData());
        }

        www.Dispose();
    }

    private void UpdateContacts()
    {
        for (uint i = 0; i < _contacts.Length; i++)
        {
            _contacts[i] = _device.GetContact(i);
        }
    }

    private IEnumerator UpdateData()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateContacts();
            sensorDataQueue.Add(new SensorData
            {
                RecordedTime = DateTime.Now,
                SensorValues = _contacts
            });
        }
    }

    private IEnumerator SendData()
    {
        while (true)
        {
            yield return StartCoroutine(KeyManager.GetServerPublicKey());
            yield return new WaitForSeconds(3);


            // Create a JSON object containing the login credentials
            var payload = sensorDataQueue;
            string jsonData = JsonConvert.SerializeObject(payload);
            sensorDataQueue.Clear();
            sensorDataQueue = new();
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            byte[] cipheredMessage = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(Convert.ToBase64String(OneTimePad.Cipher(jsonToSend))));
            // Create the UnityWebRequest object

            UnityWebRequest www = new UnityWebRequest(Constants.API_URL + Constants.PING_ENDPOINT, "POST");
            www.uploadHandler = new UploadHandlerRaw(cipheredMessage);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the request headers
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"bearer {PlayerPrefs.GetString("jwtToken")}");
            // Send the request
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.downloadHandler.text.ToString());
                Debug.Log(www.error);
                // Debug.LogError("TURN OFF"); // TODO
                // yield break;
            }
            else
            {
                Debug.Log("Everything is Okay.");
            }
            www.Dispose();
        }
    }

    public void Update()
    {
        if (BooleanTimer.shouldRecord)
        {
            elapsedTime = Time.time - BooleanTimer.startTime;
            elapsedTimeText = $"Time: {elapsedTime:F2}s";
        }
    }
}
