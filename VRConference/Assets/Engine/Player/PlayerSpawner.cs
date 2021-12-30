using System.Collections;
using System.Security.Cryptography;
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
                if (isVr.value)
                {
                    Destroy(teleport);
                }
                
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


                player = Instantiate(VR_Player, new Vector3(0, 0, 0), Quaternion.identity);
                teleport = Instantiate(VR_Teleporting, new Vector3(0, 0, 0), Quaternion.identity);
        
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

        private GameObject player;
        private GameObject teleport;
        private GameObject vrMeune;
        
        private void OnDestroy()
        {
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }
}