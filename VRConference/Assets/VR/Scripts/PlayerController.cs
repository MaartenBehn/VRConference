using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEditor.XR;
using UnityEngine.XR.Management;

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public SteamVR_Action_Boolean a_button;
    public float speed = 10;

    [SerializeField]
    private PublicEvent stopVR;

    private void Start()
    {
        a_button.AddOnStateDownListener(Test, SteamVR_Input_Sources.Any);

        stopVR.Register(() => Destroy(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Player.instance.leftHand.transform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        transform.position += speed * Time.deltaTime * new Vector3(direction.x, direction.y, direction.z);
    }

    private void Test(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        var go = Player.instance.rightHand.currentAttachedObject;
        var go2 = Player.instance.leftHand.currentAttachedObject;
        Destroy(go);
        Destroy(go2);
    }





}
