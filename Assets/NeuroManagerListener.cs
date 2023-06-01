using NextMind;
using NextMind.Devices;
using NextMind.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NeuroManagerListener : MonoBehaviour
{
    public enum CheckType // On App start caches the contacts 
    {
        CheckIfConnectionIsLower = 0, // and flags if contact is lower than the one after calibration, if so flag it.

        CheckIfConnectionIsLowerTailored = 3, // and flags if contact is lower than the one after calibration with certain threshold, if so flag it.

        CheckSumOfConnecedPoints = 1, // and sums up it's contact score, if the sum passes a certain threshold, if so flag it.

        CheckForEveryConnectedPoints = 2, // and checks for each contact if passes a certain threshold, if so flags it.
    }

    Device _device;
    [SerializeField] float[] _contacts = new float[8];
    [SerializeField] float[] _currentContactStrength = new float[8];
    
    [SerializeField] float _allowedDifference = 20f;
    [SerializeField] float _allowedSumDifference = 20f;
    [SerializeField] float _sumCache = 0f;

    [SerializeField] CheckType _checkType;
    [SerializeField] float _allowedLowerTailor = 20f;

    [SerializeField] bool _masterBreak = false;
    [SerializeField] private bool _isAllowed = true;
    public bool IsAllowed 
    {
        get { return _isAllowed; }
    }
    public void OnConnected()
    {
        _device = GetComponent<NeuroManager>().Devices[0];
        _device.onConnectionStatusChanged.AddListener(PrintContactChanged);
    }
    public void OnCalibrated()
    {
        // CacheConnections();
        // StartCoroutine(CheckContacts());
        // StartCoroutine(UpdateCurrentContacts());
    }
    IEnumerator UpdateCurrentContacts()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            for(uint i = 0; i < _contacts.Length; i++)
            {
                _currentContactStrength[i] = _device.GetContact(i);
            }
        }
    }
    IEnumerator CheckContacts()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            bool shouldBreak = true;
            switch (_checkType)
            {
                case CheckType.CheckIfConnectionIsLower:
                    shouldBreak = CheckContactChangesLower();
                    break;


                case CheckType.CheckSumOfConnecedPoints:
                    shouldBreak = CheckContactChangesSum();
                    break;


                case CheckType.CheckForEveryConnectedPoints:
                    shouldBreak = CheckContactChanges();
                    break;


                case CheckType.CheckIfConnectionIsLowerTailored:
                    shouldBreak = CheckContactChangesLowerTailored();
                    break;


                default:
                    break;
            }

            _isAllowed = !shouldBreak;
            if (_masterBreak)
            {
                if (shouldBreak)
                {
                    break;
                }
                Debug.LogError($"Last check failed. Should break.");
               
            }
        }
    }

    private bool CheckContactChangesLowerTailored()
    {
        for (uint i = 0; i < _contacts.Length; i++)
        {
            if (_contacts[i] - _allowedLowerTailor > _device.GetContact(i))
            {
                Debug.LogError($"On Contact-{i} I was expecting at least: {_contacts[i] - _allowedLowerTailor} but got {_device.GetContact(i)}");
                return true;
            }
        }

        return false;
    }

    private bool CheckContactChangesLower()
    {
        for(uint i = 0; i < _contacts.Length; i++)
        {
            if(_contacts[i] > _device.GetContact(i))
            {
                Debug.LogError($"On Contact-{i} I was expecting at least: {_contacts[i]} but got {_device.GetContact(i)}");
                return true;
            }
        }

        return false;
    }

    private bool CheckContactChangesSum()
    {
        float sumDevice = 0f;

        for (uint i = 0; i < _contacts.Length; i++)
        {
            sumDevice += _device.GetContact(i);
        }

        if(Mathf.Abs(sumDevice - _sumCache) >= _allowedSumDifference)
        {
           Debug.LogError("Contacts changed disconecting.");
           Debug.Log($"On Contact Sum I was expecting: {_sumCache} but was {sumDevice}.");
           return true;
        }
        return false;
    }

    private bool CheckContactChanges()
    {
        for (uint i = 0; i < _contacts.Length; i++)
        {
            if(Mathf.Abs(_contacts[i] - _device.GetContact(i)) > _allowedDifference)
            {
                Debug.LogError("Contacts changed disconecting.");
                Debug.Log($"On Contact {i} I was expecting: {_contacts[i]} but was {_device.GetContact(i)}.");
                return true;
            }
            // Debug.Log($"Contact-{i}:{_contacts[i]}:{_device.GetContact(i)}");
        }
        return false;
    }

    private void CacheConnections()
    {
        // for (uint i = 0; i < _contacts.Length; i++)
        // {
        //     _contacts[i] = _device.GetContact(i);
        //     _sumCache += _contacts[i];
        //     Debug.Log($"Setting Contact-{i} = {_device.GetContact(i)}");
        // }
    }

    public void PrintContactChanged(ConnectionStatus newStatus)
    {
        Debug.Log($"Contact changed, new Status: {newStatus}");
    }

}
