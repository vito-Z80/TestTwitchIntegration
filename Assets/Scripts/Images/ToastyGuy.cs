using System.Collections;
using Data;
using TMPro;
using Twitch;
using UnityEngine;

namespace Images
{
    public class ToastyGuy : MonoBehaviour
    {
        AudioSource m_audio;
        [SerializeField] TextMeshProUGUI userNameLabel;

        RectTransform m_imageRectTransform;

        bool m_isPlaying;
        Vector2 m_targetPosition;

        void OnEnable()
        {
            TwitchChatController.OnSayHello += SayHello;
        }


        void Start()
        {
            m_imageRectTransform = GetComponent<RectTransform>();
            m_audio = GetComponent<AudioSource>();
        }

        public void SayHello(ChatUserData userData)
        {
            if (m_isPlaying) return;
            m_isPlaying = true;
            if (LocalStorage.GetSettings().displayNicknameColor)
            {
                userNameLabel.color = userData.Color;
            }

            userNameLabel.text = userData.UserName;

            StartCoroutine(PlayGreetingsCoroutine());
            m_audio.Play();
        }

        public void PlaySound()
        {
            
        }

        IEnumerator PlayGreetingsCoroutine()
        {
            const float deviation = 0.1f;
            const float speed = 8.0f;
            while (Vector2.Distance(m_imageRectTransform.anchoredPosition, m_targetPosition) > deviation)
            {
                m_imageRectTransform.anchoredPosition = Vector2.Lerp(m_imageRectTransform.anchoredPosition, m_targetPosition, Time.deltaTime * speed);
                yield return null;
            }

            while (Vector2.Distance(m_imageRectTransform.anchoredPosition, Vector2.zero) > deviation)
            {
                m_imageRectTransform.anchoredPosition = Vector2.Lerp(m_imageRectTransform.anchoredPosition, Vector2.zero, Time.deltaTime * speed);
                yield return null;
            }

            m_imageRectTransform.anchoredPosition = Vector2.zero;
            yield return null;
            m_isPlaying = false;
        }

        public void SetTargetPosition(Vector2 targetPosition)
        {
            m_targetPosition = targetPosition;
        }

        void OnDisable()
        {
            TwitchChatController.OnSayHello -= SayHello;
        }
    }
}