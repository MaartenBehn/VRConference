using System;
using UnityEngine;

namespace UI
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject loadScreen;
        [SerializeField] private GameObject inGameScreen;

        [SerializeField] private PublicEvent loadServer;
        [SerializeField] private PublicEvent loadClient;
        [SerializeField] private PublicEvent loadingDone;
        [SerializeField] private PublicEvent loadingFailed;
        
        private void Awake()
        {
            loadServer.Register(SetLoadScreen);
            loadClient.Register(SetLoadScreen);
            loadingDone.Register(SetInGameScreen);
            loadingFailed.Register(SetStartScreen);
            
            inGameScreen.SetActive(false);
            loadScreen.SetActive(false);
            startScreen.SetActive(true);
        }

        private void SetStartScreen()
        {
            inGameScreen.SetActive(false);
            loadScreen.SetActive(false);
            startScreen.SetActive(true);
        }
        
        private void SetLoadScreen()
        {
            inGameScreen.SetActive(false);
            startScreen.SetActive(false);
            loadScreen.SetActive(true);
        }

        private void SetInGameScreen()
        {
            loadScreen.SetActive(false);
            startScreen.SetActive(false);
            inGameScreen.SetActive(true);
        }
    }
}