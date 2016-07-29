using UnityEngine;
using System.Collections;


public class CallbackChecker : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Awake");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    void OnLevelWasLoaded()
    {
        Debug.Log("OnLevelWasLoaed");
    }

    void Start()
    {
        Debug.Log("Start");
    }

    public bool DisplayOnApplicationPause = false;
    void  OnApplicationPause()
    {
        if (DisplayOnApplicationPause)
            Debug.Log("OnApplicationPause");
    }

    public bool DisplayFixedUpdate = false;
    void FixedUpdate()
    {
        if (DisplayFixedUpdate)
            Debug.Log("FixedUpdate");
    }

    public bool DisplayUpdate = false;
    void Update()
    {
        if (DisplayUpdate)
            Debug.Log("Update");
    }

    public bool DisplayLateUpdate = false;
    void LateUpdate()
    {
        if (DisplayLateUpdate)
            Debug.Log("LateUpdate");
    }

    public bool DisplayOnGUI = false;
    void OnGUI()
    {
        if (DisplayOnGUI)
            Debug.Log("OnGUI");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
