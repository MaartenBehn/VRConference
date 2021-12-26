using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CheckVR : MonoBehaviour
{

    public PublicBool isVR;

    private void Awake()
    {
        if (!isVR.value)
        {
            Destroy(this.gameObject);
        }
    }
}
