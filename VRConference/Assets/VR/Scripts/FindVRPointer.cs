using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FindVRPointer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GetComponent<Canvas>();
        if (Player.instance != null)
        {
            var pointer = Player.instance.rightHand.transform.GetChild(4).GetComponent<Camera>();
            if (pointer != null)
            {
                canvas.worldCamera = pointer;
            }
        }
    }
}
