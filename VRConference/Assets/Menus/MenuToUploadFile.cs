using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using SimpleFileBrowser;
using Utility;

public class MenuToUploadFile : MonoBehaviour
{

    public GameObject FileBrowserPrefab;

    private GameObject FileBrowserObj;
    public GameObject FbxLoader;
    [SerializeField] private PublicEventString uploadEvent;

    // Start is called before the first frame update
    void Start()
    {
        FileBrowserObj = Instantiate(FileBrowserPrefab,gameObject.transform.parent.transform).transform.GetChild(0).gameObject;
        FileBrowserObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        FileBrowserObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        FileBrowserObj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0,0,0);
        FileBrowserObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        if (Player.instance != null)
        {
            FileBrowserObj.GetComponent<Canvas>().worldCamera = Player.instance.rightHand.transform.GetChild(4).GetComponent<Camera>();
            GetComponent<Canvas>().worldCamera = Player.instance.rightHand.transform.GetChild(4).GetComponent<Camera>();
        }
        FileBrowser.ShowLoadDialog((string[] paths) =>
        {
            
            foreach (string path in paths)
            {
                uploadEvent.Raise(path);
            }
        }, () =>
        {

        }, FileBrowser.PickMode.FilesAndFolders, multibleFiles);
    }

    public bool multibleFiles;


    // Update is called once per frame
    void Update()
    {
        if (FileBrowserObj != null)
        {
            if (FileBrowserObj.active == false)
            {
                AktiveFileMenu();
                FileBrowserObj.SetActive(true);
            }
        }
    }

    void AktiveFileMenu()
    {
        FileBrowserObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        FileBrowserObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        FileBrowserObj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        FileBrowserObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        var fbxScript = FbxLoader.GetComponent<FbxLoader>();
        FileBrowser.ShowLoadDialog((string[] paths) =>
        {

            foreach (string path in paths)
            {
                uploadEvent.Raise(path);
            }
        }, () =>
        {

        }, FileBrowser.PickMode.FilesAndFolders, multibleFiles);
    }
}
