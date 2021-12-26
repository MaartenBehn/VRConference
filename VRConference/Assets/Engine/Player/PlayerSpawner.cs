using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;
using Utility;

namespace Engine.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            playerStart.Register(() =>
            {
                playerFaetureState.value = (int) FeatureState.starting;

                if (!isVr.value)
                {
                    player = Instantiate(playerFirstPerson, world.transform);
                    playerFaetureState.value = (int) FeatureState.online;
                    firstPersonFaetureState.value = (int) FeatureState.online;
                }
                else
                {
                    StartCoroutine(StartXR());
                }
            });

            playerStop.Register(() =>
            {
                playerFaetureState.value = (int) FeatureState.stopping;
                Destroy(player);
                playerFaetureState.value = (int) FeatureState.offline;
            });
        }
        
        IEnumerator StartXR()
        {
            Debug.Log("Initializing XR...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
                playerFaetureState.value = (int) FeatureState.failed;
                vrFaetureState.value = (int) FeatureState.failed;
            }
            else
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.activeLoader.Start();
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            
                XRGeneralSettings.Instance.Manager.InitializeLoader();
            
                //yield return new WaitForSecondsRealtime(5);


                player = Instantiate(VR_Player, new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(VR_Teleporting, new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(VR_TeleportPoint, new Vector3(0, 0, 0), Quaternion.identity);
                Instantiate(VR_TeleportPoint, new Vector3(0, 0, 0), Quaternion.identity);
                var test = Instantiate(VR_Menu, new Vector3(0.02485144f, 1.229f, 5.52f), Quaternion.identity);
                test.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                test.transform.GetChild(0).GetComponent<Canvas>().worldCamera = player.transform.GetChild(0).transform.GetChild(2).GetChild(4).GetComponent<Camera>();
                playerFaetureState.value = (int) FeatureState.online;
                vrFaetureState.value = (int) FeatureState.online;
            }
        }

        [SerializeField] private GameObject playerFirstPerson;
        [SerializeField] private PublicBool isVr;
        
        [SerializeField] private PublicEvent playerStart;
        [SerializeField] private PublicEvent playerStop;
        [SerializeField] private PublicInt playerFaetureState;
        [SerializeField] private PublicInt vrFaetureState;
        [SerializeField] private PublicInt firstPersonFaetureState;
        
        
        
        [SerializeField] private GameObject world;
        
        public GameObject VR_Player;
        public GameObject VR_Teleporting;
        public GameObject VR_TeleportPoint;

        public GameObject VR_Menu;

        private GameObject player;
        
        private void OnDestroy()
        {
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }
}