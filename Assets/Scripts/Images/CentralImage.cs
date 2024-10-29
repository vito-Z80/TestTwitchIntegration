using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Images
{
    public class CentralImage : MonoBehaviour
    {
        [SerializeField] Sprite testSprite;

        Image m_image;
        bool m_isShowing;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_image = GetComponentInChildren<Image>();
            m_image.gameObject.SetActive(false);
        }

        public void Show(Sprite sprite)
        {
            if (m_isShowing) return;
            m_isShowing = true;
            ImageSizeCalculate();
            m_image.sprite = sprite;
            StartCoroutine(ShowImage());
        }

        void ImageSizeCalculate()
        {
            m_image.SetNativeSize();
            float minWindowSize = Mathf.Min(m_settings.windowWidth, m_settings.windowHeight);
            var maxImageSize = Mathf.Max(m_image.rectTransform.sizeDelta.x, m_image.rectTransform.sizeDelta.y);
            var delta = minWindowSize / maxImageSize;
            var scale = new Vector3(delta, delta, 1.0f);
            m_image.rectTransform.localScale = scale;
        }

        IEnumerator ShowImage()
        {
            m_image.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            m_image.gameObject.SetActive(false);
            m_image.sprite = null;
            m_isShowing = false;
            yield return null;
        }


        void Update()
        {
            var halfScale = m_settings.imageScale * m_settings.windowHeight * 0.5f;
            var posX = -m_settings.windowWidth * 0.5f +  halfScale + m_settings.imageX * m_settings.windowWidth;
            var posY = -m_settings.windowHeight * 0.5f +  halfScale + m_settings.imageY * m_settings.windowHeight;
            transform.localPosition = new Vector3(posX, posY, 0.0f);
        }

        public void ScaleImage(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void TestShow(bool isShowed)
        {
            if (isShowed)
            {
                m_image.gameObject.SetActive(true);
                ImageSizeCalculate();
                m_image.sprite = testSprite;
                var posX = m_settings.imageX * m_settings.windowWidth + m_settings.windowWidth / 2.0f;
                var posY = m_settings.imageY * m_settings.windowHeight + m_settings.windowHeight / 2.0f;
                transform.localPosition = new Vector3(posX, transform.localPosition.y, 0.0f);
            }
            else
            {
                m_image.sprite = null;
                m_image.gameObject.SetActive(false);
            }
        }
    }
}