using System.Collections;
using System.Collections.Generic;
using Network;
using Network.FileShare;
using UnityEngine;
using Utility;


namespace Audio.AudioSpeaker
{
    public class SpeakerController : MonoBehaviour
    {
        public static SpeakerController instance;
        public List<AudioSpeaker> speakers;
        public string songFileName;
        public string currentSongFileName;
        public bool playing;

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

            speakers = new List<AudioSpeaker>();
        }
        
        public void SetSong(string fileName)
        {
            songFileName = fileName;
            
            NetworkController.instance.networkSend.SpeakerPlaySong(fileName);
        }

        private void Update()
        {
            if (songFileName != currentSongFileName && playing)
            {
                playing = false;
                foreach (AudioSpeaker audioSpeaker in speakers)
                {
                    audioSpeaker.audioSource.Pause();
                }
            }
            
            if (songFileName == "") { return; }

            FileEntry entry = null;


            foreach (FileEntry fileEntry in FileShare.instance.fileEntries)
            {
                if (fileEntry.fileName == songFileName)
                {
                    entry = fileEntry;
                    break;
                }
            }

            if (entry == null && !FileShare.instance.syncingFile)
            {
                Debug.Log("SPEAKER: Song unknown");
                FileShare.instance.SyncFiles();
            }
            else if (entry != null && !entry.local && !FileShare.instance.syncingFile)
            {
                Debug.Log("SPEAKER: Downloading Song");
                FileShare.instance.SyncFile(entry);
            }
            else if (entry != null && entry.local && !playing)
            {
                playing = true;
                currentSongFileName = songFileName;
                
                Debug.Log("SPEAKER: Loading Clip");
                StartCoroutine(LoadSongCoroutine(entry.localPath));
            }
        }
        
        IEnumerator LoadSongCoroutine(string path)
        {
            string url = string.Format("file://{0}", path); 
            WWW www = new WWW(url);
            yield return www;

            var clip = www.GetAudioClip(false, false);
            foreach (AudioSpeaker audioSpeaker in speakers)
            {
                audioSpeaker.PlayClip(clip);
            }
            Debug.Log("SPEAKER: Loading Done");
        }
    }
}