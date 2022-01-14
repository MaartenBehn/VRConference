using Engine;
using TMPro;
using UnityEngine;
using Utility;

public class FeatureSymbol : MonoBehaviour
{
    [SerializeField] private GameObject offline;
    [SerializeField] private GameObject online;
    [SerializeField] private GameObject failed;
    [SerializeField] private GameObject loading;
    [SerializeField] private TMP_Text text;
    private PublicInt state;
    private int lastState;

    private void Awake()
    {
        online.SetActive(false);
        offline.SetActive(true);
        failed.SetActive(false);
        loading.SetActive(false);
    }

    public void SetText(string t)
    {
        text.text = t;
    }

    public void SetFeatureState(PublicInt s)
    {
        state = s;
    }

    private void Update()
    {
        if (state == null || lastState == state.value)
            return;

        switch (state.value)
        {
            case (int) FeatureState.offline:
                online.SetActive(false);
                offline.SetActive(true);
                failed.SetActive(false);
                loading.SetActive(false);
                break;
            
            case (int) FeatureState.online:
                online.SetActive(true);
                offline.SetActive(false);
                failed.SetActive(false);
                loading.SetActive(false);
                break;
            
            case (int) FeatureState.starting:
                online.SetActive(false);
                offline.SetActive(false);
                failed.SetActive(false);
                loading.SetActive(true);
                break;
            
            case (int) FeatureState.stopping:
                online.SetActive(false);
                offline.SetActive(false);
                failed.SetActive(false);
                loading.SetActive(true);
                break;
            
            case (int) FeatureState.failed:
                online.SetActive(false);
                offline.SetActive(false);
                failed.SetActive(true);
                loading.SetActive(false);
                break;
        }

        lastState = state.value;
    }
}
