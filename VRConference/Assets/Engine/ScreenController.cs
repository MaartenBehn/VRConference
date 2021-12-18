using UnityEngine;
using Utility;

namespace Engine
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject loadScreen;
        [SerializeField] private GameObject inGameScreen;
        
        [SerializeField] private PublicEventBool load;
        [SerializeField] private PublicEvent loadingDone;
        [SerializeField] private PublicEvent loadingFailed;
        [SerializeField] private PublicEvent unload;

        private void Awake()
        {
            load.Register((bool b) =>
            {
                SetLoadScreen();
            });
            loadingDone.Register(SetInGameScreen);
            loadingFailed.Register(SetStartScreen);
            unload.Register(SetStartScreen);
            
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