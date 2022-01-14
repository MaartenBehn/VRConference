using UnityEngine;

namespace Users.FirstPerson
{
    public class FirstPersonPosSync : MonoBehaviour
    {
        void Update()
        {
            Network.NetworkController.instance.networkSend.UserFirstPersonPos(transform);
        }
    }
}
