using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using Network.FileShare;
using UnityEngine;
using UnityEngine.Networking;

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

        private async void Update()
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
                FileShare.instance.SyncFiles();
            }
            else if (entry != null && !entry.local && !FileShare.instance.syncingFile)
            {
                FileShare.instance.SyncFile(entry);
            }
            else if (entry != null &&  entry.local && !playing)
            {
                AudioClip clip = await LoadClip(entry.localPath);
                currentSongFileName = songFileName;

                playing = true;
                foreach (AudioSpeaker audioSpeaker in speakers)
                {
                    audioSpeaker.PlayClip(clip);
                }
            }
        }

        async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
            {
                uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }
    }
}