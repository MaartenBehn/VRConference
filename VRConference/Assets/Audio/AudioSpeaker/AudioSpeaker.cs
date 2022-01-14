using System;
using System.Threading.Tasks;
using Network;
using Network.FileShare;
using UnityEngine;
using UnityEngine.Networking;
using Voice;

namespace Engine.AudioSpeaker
{
    public class AudioSpeaker : MonoBehaviour
    {
        public int speakerId;
        public string songFileName;
        public string currentSongFileName;
        [SerializeField] private AudioSource audioSource;

        private void Start()
        {
            speakerId = SpeakerController.instance.speakers.Count;
            SpeakerController.instance.speakers.Add(this);
            audioSource.loop = true;

            VoiceConnection.SetAudioSourceSettings(audioSource);
        }

        public void SetSong(string fileName)
        {
            songFileName = fileName;
            
            NetworkController.instance.networkSend.SpeakerPlaySong(speakerId, fileName);
        }

        private async void Update()
        {
            if (songFileName != currentSongFileName && audioSource.isPlaying)
            {
                audioSource.Pause();
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
            else if (entry != null &&  entry.local && !audioSource.isPlaying)
            {
                AudioClip clip = await LoadClip(entry.localPath);
                audioSource.clip = clip;
                audioSource.Play();
                currentSongFileName = songFileName;
            }
            
        }
        
        async Task<AudioClip> LoadClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://"+path, AudioType.MPEG))
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