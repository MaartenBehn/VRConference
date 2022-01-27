using System;
using System.Linq;
using Network.FileShare;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Menus
{
    public class FileList : MonoBehaviour
    {
        private float lastUpdate;
        [SerializeField] private GameObject entryPreFab;
        
        void Update()
        {
            if (lastUpdate + 3 < Time.time || transform.childCount != FileShare.instance.fileEntries.Count)
            {
                lastUpdate = Time.time;

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                
                foreach (FileEntry fileEntry in FileShare.instance.fileEntries)
                {
                    FileListEntry entry = Instantiate(entryPreFab, transform).GetComponent<FileListEntry>();
                    entry.name.text = fileEntry.fileName;
                    
                    if (fileEntry.local)
                    {
                        entry.path.text = fileEntry.localPath;
                    }
                    else
                    {
                        entry.path.text = "Nicht lokal vorhanden.";
                    }
                }
            }
        }
    }
}
