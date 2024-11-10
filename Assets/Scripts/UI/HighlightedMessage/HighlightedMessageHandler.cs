using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.HighlightedMessage
{
    public class HighlightedMessageHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI userName;
        [SerializeField] TextMeshProUGUI userMessage;

        RectTransform m_rectTransform;
        Image m_image;
        Color m_color = new(1f, 1f, 1f, 0.1f);

        AppSettingsData m_settings;

        //  TODO HTML тэги нужно отключить или обрабатывать. Тэг size сломает всё при большом значении.

        public void Show(ChatUserData userData, string message)
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_image = GetComponent<Image>();
            m_image.color = m_color;
            m_rectTransform.anchoredPosition = Vector2.one * 100000;
            userMessage.gameObject.SetActive(false);
            userName.gameObject.SetActive(false);

            userName.text = userData.UserName;
            userName.color = userData.Color;
            userMessage.text = message;

            
            transform.SetAsLastSibling();
            
            StartCoroutine(Show());
        }

        void Init()
        {
            userMessage.gameObject.SetActive(true);
            userName.gameObject.SetActive(true);
            m_settings = LocalStorage.GetSettings();
            userMessage.maxVisibleCharacters = 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_rectTransform);

            m_rectTransform.localScale = Vector3.one * 1.5f;

            var rectSize = m_rectTransform.rect.size;
            var maxX = m_settings.windowWidth / 2.0f - rectSize.x / 2.0f;
            var minX = -maxX;
            var maxY = m_settings.windowHeight / 2.0f - rectSize.y / 2.0f;
            var minY = -maxY;
            var x = Random.Range(minX, maxX);
            var y = Random.Range(minY, maxY);
            m_rectTransform.anchoredPosition = new Vector2(x, y);
            var angle = Random.Range(-8, 8);
            m_rectTransform.Rotate(Vector3.forward, angle);
        }

        IEnumerator Show()
        {
            Init();
            var wait = new WaitForSeconds(0.01f);

            while (m_rectTransform.localScale.x >= 1.0f)
            {
                var dt = Time.deltaTime * 3.0f;
                m_rectTransform.localScale -= Vector3.one * dt;
                m_color.a += dt;
                m_image.color = m_color;
                yield return null;
            }

            m_color.a = 0.75f;
            m_image.color = m_color;
            m_rectTransform.localScale = Vector3.one;

            while (userMessage.text.Length != userMessage.maxVisibleCharacters)
            {
                userMessage.maxVisibleCharacters++;
                yield return wait;
            }

            yield return new WaitForSeconds(2.0f);
            while (m_rectTransform.localScale.x > 0.0f && m_rectTransform.localScale.y > 0.0f)
            {
                var dt = Time.deltaTime;
                m_rectTransform.localScale -= Vector3.one * dt;
                m_color.a = Mathf.Clamp(m_color.a - dt, 0.0f, 1.0f);
                m_image.color = m_color;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}