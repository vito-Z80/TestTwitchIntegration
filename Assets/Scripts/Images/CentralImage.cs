using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Images
{
    public class CentralImage : MonoBehaviour
    {
        [SerializeField] Sprite testSprite;

        Image m_image;
        bool m_isShowed;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            transform.position = Vector3.one * 10000;
            m_image = GetComponentInChildren<Image>();
        }

        public async Task Show(string message, ImageData[] imageData)
        {
            if (m_isShowed) return;
            var sprite = await ParseMessage(message, imageData);
            if (sprite is null) return;
            m_isShowed = true;
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

        Task<Sprite> ParseMessage(string message, ImageData[] imageData)
        {
            foreach (var data in imageData)
            {
                var regex = new Regex(data.Pattern, RegexOptions.IgnoreCase);
                var coincidence = regex.IsMatch(message);
                if (coincidence)
                {
                    return Task.FromResult(data.Sprite);
                }
            }

            return Task.FromResult<Sprite>(null);
        }


        IEnumerator ShowImage()
        {
            transform.localPosition = Vector3.zero;
            yield return new WaitForSeconds(3.0f);
            transform.localPosition = Vector3.one * 10000;
            m_image.sprite = null;
            m_isShowed = false;
            yield return null;
        }


        public void ScaleImage(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void TestShow(bool isShowed)
        {
            if (isShowed)
            {
                ImageSizeCalculate();
                m_image.sprite = testSprite;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                m_image.sprite = null;
                transform.localPosition = Vector3.one * 10000;
            }
        }
    }
}