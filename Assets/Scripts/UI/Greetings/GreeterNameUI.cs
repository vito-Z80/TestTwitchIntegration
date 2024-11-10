using System.Threading.Tasks;
using Data;
using TMPro;
using UnityEngine;

namespace UI.Greetings
{
    public class GreeterNameUI : MonoBehaviour
    {
        RectTransform m_rectTransform;
        TextMeshProUGUI m_text;
        Vector2 m_textTargetPosition;

        AppSettingsData m_settings;

        bool m_isPlaying;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_rectTransform = GetComponent<RectTransform>();
            m_text = GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        public bool IsPlaying() => m_isPlaying;
        
        public void Play(ChatUserData userData)
        {
            if (m_isPlaying) return;
            m_isPlaying = true;
            transform.SetAsLastSibling();
            if (LocalStorage.GetSettings().displayNicknameColor)
            {
                m_text.color = userData.Color;
            }
            m_text.text = userData.UserName;
            var textPosition = new Vector2(0.0f, -m_settings.windowHeight / 2.0f - m_rectTransform.sizeDelta.y / 2.0f);
            m_rectTransform.anchoredPosition = textPosition;
            m_textTargetPosition = Vector2.zero;
            _ = PlayGreetingsText();
        }

        async Task PlayGreetingsText()
        {
            const float deviation = 0.1f;
            const float speed = 5.5f;
            var finishPosition = -m_rectTransform.anchoredPosition;
            while (Vector2.Distance(m_rectTransform.anchoredPosition, m_textTargetPosition) > deviation)
            {
                m_rectTransform.anchoredPosition = Vector2.Lerp(m_rectTransform.anchoredPosition, m_textTargetPosition, Time.deltaTime * speed);
                await Task.Yield();
            }

            m_textTargetPosition = finishPosition;
            while (Vector2.Distance(m_rectTransform.anchoredPosition, m_textTargetPosition) > deviation)
            {
                m_rectTransform.anchoredPosition = Vector2.Lerp(m_rectTransform.anchoredPosition, m_textTargetPosition, Time.deltaTime * speed);
                await Task.Yield();
            }

            m_rectTransform.anchoredPosition = -finishPosition;
            m_isPlaying = false;
            gameObject.SetActive(false);
        }
    }
}