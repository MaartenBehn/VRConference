using UnityEngine;

namespace VR.Scripts
{
    public class VRPosSync : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform handLeft;
        [SerializeField] private Transform handRigth;
        
        void FixedUpdate()
        {
            Network.NetworkController.instance.networkSend.UserVRPos(head, handLeft, handRigth);
        }
    }
}
