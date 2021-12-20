using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PauseMenue : MonoBehaviour
{
    
    [SerializeField] private PublicBool pause;
    [SerializeField] private float lockDelay = 1;
    private float lastLockTime;

    [SerializeField] private GameObject pauseMeune;

    private void Start()
    {
        Set(true);
    }

    void Update()
    {
        if (lastLockTime + lockDelay < Time.time && Input.GetKey(KeyCode.Escape))
        {
            lastLockTime = Time.time;
            Set(!pause.value);
        }
    }

    public void Set(bool value)
    {
        pause.value = value;
        Cursor.lockState = !pause.value ? CursorLockMode.Locked : CursorLockMode.None;
        pauseMeune.SetActive(pause.value);
    }
}
