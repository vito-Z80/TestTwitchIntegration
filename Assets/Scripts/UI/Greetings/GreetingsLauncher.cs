using System;
using Data;
using Twitch;
using UnityEngine;

namespace UI.Greetings
{
    public class GreetingsLauncher : MonoBehaviour
    {
        [SerializeField] GreetingsImageUI greetingsImageUI;
        [SerializeField] GreeterNameUI greeterNameUI;
        AudioSource m_audioSource;
        readonly ChatUserData m_defaultChatterData = new() { UserName = "testUser_name", Color = Color.white };

        AppSettingsData m_settings;


        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            TwitchChatController.OnSayHello += Play;
        }

        void Play(ChatUserData chatterData)
        {
            // TODO нужно контролить более одного поздоровавшегося.
            if (greetingsImageUI.IsPlaying() || greeterNameUI.IsPlaying() || m_audioSource.isPlaying) return;
            if (m_settings.greetingsImagePath.Length > 0)
            {
                greetingsImageUI.gameObject.SetActive(true);    
                _ = greetingsImageUI.Play();
            }
            
            greeterNameUI.gameObject.SetActive(true);
            greeterNameUI.Play(chatterData);
            
            if (m_settings.greetingsAudioPath.Length > 0)
            {
                StartCoroutine(LocalStorage.LoadAndPlayAudio(m_settings.greetingsAudioPath, m_audioSource));    
            }
            
        }

        public void TestPlay()
        {
            Play(m_defaultChatterData);
        }

        void OnDisable()
        {
            TwitchChatController.OnSayHello -= Play;
        }
    }
}