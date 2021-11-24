using TMPro;
using UnityEngine;

namespace Utility.Console
{
    public class ConsoleMessage : MonoBehaviour
    {
        public TMP_Text text;
    
        [HideInInspector] public float liveTime;
        [HideInInspector] public float startTime;
        
        private void Update()
        {
            if (startTime + liveTime <= Time.time)
            {
                Destroy(gameObject);
            }
        }
    }
}
