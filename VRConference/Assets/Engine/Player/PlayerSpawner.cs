using System;
using UnityEngine;
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
                if (!isVr)
                {
                    player = Instantiate(playerFirstPerson, transform);
                }
                else
                {
                    player = Instantiate(playerVr, transform);
                }
                playerFaetureState.value = (int) FeatureState.online;
            });

            playerStop.Register(() =>
            {
                playerFaetureState.value = (int) FeatureState.stopping;
                Destroy(player);
                playerFaetureState.value = (int) FeatureState.offline;
            });
        }

        [SerializeField] private GameObject playerFirstPerson;
        [SerializeField] private GameObject playerVr;
        [SerializeField] private PublicBool isVr;
        [SerializeField] private PublicEvent playerStart;
        [SerializeField] private PublicEvent playerStop;
        [SerializeField] private PublicInt playerFaetureState;

        private GameObject player;
    }
}