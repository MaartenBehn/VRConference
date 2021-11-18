using SimpleFileBrowser;
using UnityEngine;
using Utility;

namespace FileShare
{
    public class FileSelecter : MonoBehaviour
    {
        public bool multibleFiles;
        public string[] allowedEndings;
        [SerializeField] private PublicEventString uploadEvent;
        
        public void SelectFile()
        {
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
}