using UnityEngine;

namespace Engine
{
    public class FileBrowser : MonoBehaviour
    {
        void Start()
        {
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(null, null, SimpleFileBrowser.FileBrowser.PickMode.FilesAndFolders);
        }
    }
}
