using System;
using System.Threading.Tasks;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Greetings
{
    public class GreetingsImageUI : MonoBehaviour
    {
        RectTransform m_greetingsImageTransform;

        Vector2 m_imageTargetPosition;
        AppSettingsData m_settings;

        bool m_isPlaying;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_greetingsImageTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public bool IsPlaying() => m_isPlaying;

        public async Task Play()
        {
            if (m_isPlaying) return;
            m_isPlaying = true;
            await PlayGreetings();
        }

        void OnImageChanged()
        {
            //  TODO нинада каждый раз подгружать одну и ту же картинку !
            var image = m_greetingsImageTransform.GetComponent<Image>();
            image.sprite = LocalStorage.LoadSprite(m_settings.greetingsImagePath);
            SetImageAnchor();
            SetImageSize();
        }

        void SetImageAnchor()
        {
            switch (m_settings.greetingsImageAnchor)
            {
                case TMP_Compatibility.AnchorPositions.TopLeft:
                    m_greetingsImageTransform.pivot = new Vector2(1.0f, 0.0f);
                    m_greetingsImageTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    m_greetingsImageTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    SetTopLeftTargetPosition();
                    break;
                case TMP_Compatibility.AnchorPositions.TopRight:
                    m_greetingsImageTransform.pivot = new Vector2(0.0f, 0.0f);
                    m_greetingsImageTransform.anchorMin = new Vector2(1.0f, 1.0f);
                    m_greetingsImageTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    SetTopRightTargetPosition();
                    break;
                case TMP_Compatibility.AnchorPositions.BottomLeft:
                    m_greetingsImageTransform.pivot = new Vector2(1.0f, 1.0f);
                    m_greetingsImageTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    m_greetingsImageTransform.anchorMax = new Vector2(0.0f, 0.0f);
                    SetBottomLeftTargetPosition();
                    break;
                case TMP_Compatibility.AnchorPositions.BottomRight:
                    m_greetingsImageTransform.pivot = new Vector2(0.0f, 1.0f);
                    m_greetingsImageTransform.anchorMin = new Vector2(1.0f, 0.0f);
                    m_greetingsImageTransform.anchorMax = new Vector2(1.0f, 0.0f);
                    SetBottomRightTargetPosition();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_settings.greetingsImageAnchor), m_settings.greetingsImageAnchor, null);
            }

            m_greetingsImageTransform.anchoredPosition = Vector2.zero;
        }

        void SetImageSize()
        {
            var size = m_settings.greetingsImageSize * m_settings.windowHeight;
            m_greetingsImageTransform.sizeDelta = new Vector2(size, size);
        }

        void SetBottomRightTargetPosition()
        {
            SetTargetPosition(new Vector3(-m_settings.windowHeight * m_settings.greetingsImageSize, m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetBottomLeftTargetPosition()
        {
            SetTargetPosition(new Vector3(m_settings.windowHeight * m_settings.greetingsImageSize, m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetTopRightTargetPosition()
        {
            SetTargetPosition(new Vector3(-m_settings.windowHeight * m_settings.greetingsImageSize, -m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetTopLeftTargetPosition()
        {
            SetTargetPosition(new Vector3(m_settings.windowHeight * m_settings.greetingsImageSize, -m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        async Task PlayGreetings()
        {
            OnImageChanged();
            const float deviation = 0.1f;
            const float speed = 8.0f;
            while (Vector2.Distance(m_greetingsImageTransform.anchoredPosition, m_imageTargetPosition) > deviation)
            {
                m_greetingsImageTransform.anchoredPosition = Vector2.Lerp(m_greetingsImageTransform.anchoredPosition, m_imageTargetPosition, Time.deltaTime * speed);
                await Task.Yield();
            }

            while (Vector2.Distance(m_greetingsImageTransform.anchoredPosition, Vector2.zero) > deviation)
            {
                m_greetingsImageTransform.anchoredPosition = Vector2.Lerp(m_greetingsImageTransform.anchoredPosition, Vector2.zero, Time.deltaTime * speed);
                await Task.Yield();
            }

            m_greetingsImageTransform.anchoredPosition = Vector2.zero;
            m_isPlaying = false;
            gameObject.SetActive(false);
        }

        void SetTargetPosition(Vector2 targetPosition)
        {
            m_imageTargetPosition = targetPosition;
        }
    }
}