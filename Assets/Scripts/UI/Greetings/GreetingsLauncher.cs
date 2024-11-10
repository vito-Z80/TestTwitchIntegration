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
            greetingsImageUI.gameObject.SetActive(true);
            greeterNameUI.gameObject.SetActive(true);
            _ = greetingsImageUI.Play();
            greeterNameUI.Play(chatterData);
            
            StartCoroutine(LocalStorage.LoadAndPlayAudio(m_settings.greetingsAudioPath, m_audioSource));
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