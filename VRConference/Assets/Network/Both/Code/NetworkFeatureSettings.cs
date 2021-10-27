using UnityEngine;

namespace Network.Both
{
    [CreateAssetMenu(fileName = "NetworkFeatureSettings", menuName = "Network/NetworkFeatureSettings")]
    public class NetworkFeatureSettings : ScriptableObject
    {
        public bool UPDSupport;
    }
}