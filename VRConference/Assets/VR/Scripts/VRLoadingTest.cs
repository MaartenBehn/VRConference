using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.XR;
using UnityEngine.XR.Management;
using Valve.VR;

public class VRLoadingTest : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject VR_Player;
    public GameObject VR_Teleporting;
    public GameObject VR_TeleportPoint;

    public GameObject VR_Menu;

    void Start()
    {
        StartCoroutine(StartXR());
    }

    private void OnDestroy()
    {
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }

    IEnumerator StartXR()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();


        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.activeLoader.Start();
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            
            XRGeneralSettings.Instance.Manager.InitializeLoader();
            
            //yield return new WaitForSecondsRealtime(5);


            var playertest = Instantiate(VR_Player, new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(VR_Teleporting, new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(VR_TeleportPoint, new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(VR_TeleportPoint, new Vector3(0, 0, 0), Quaternion.identity);
            var test = Instantiate(VR_Menu, new Vector3(0.02485144f, 1.229f, 5.52f), Quaternion.identity);
            test.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            test.transform.GetChild(0).GetComponent<Canvas>().worldCamera = playertest.transform.GetChild(0).transform.GetChild(2).GetChild(4).GetComponent<Camera>();
        }
    }

    public SteamVR_Action_Boolean headsetOnHead = SteamVR_Input.GetBooleanAction("HeadsetOnHead");


    // Update is called once per frame
    void Update()
    {
        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            return;
        
        if (headsetOnHead != null)
        {
            if (headsetOnHead.GetStateDown(SteamVR_Input_Sources.Head))
            {
                Debug.Log("Headset is on Head!");
            }
            else if (headsetOnHead.GetStateUp(SteamVR_Input_Sources.Head))
            {
               
            }
        }
    }
}
