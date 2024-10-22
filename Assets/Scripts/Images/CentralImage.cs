using System;
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
        [SerializeField] Camera pixelCamera;
        [SerializeField] Sprite testSprite;

        Image m_image;
        bool m_isShowed;

        const float MaxImageSize = 1024.0f;

        void OnEnable()
        {
            Configuration.OnShowTest += TestShow;
            Configuration.OnImageScaleChanged += ScaleImage;
        }

        void OnDisable()
        {
            Configuration.OnShowTest -= TestShow;
            Configuration.OnImageScaleChanged -= ScaleImage;
        }

        void Start()
        {
            transform.position = Vector3.one * 10000;
            m_image = GetComponentInChildren<Image>();
        }

        public async Task Show(string message, ImageData[] imageData)
        {
            if (m_isShowed) return;
            // m_image.SetNativeSize();
            var sprite = await ParseMessage(message, imageData);
            if (sprite is null) return;
            m_isShowed = true;
            ImageSizeCalculate(sprite);
            m_image.sprite = sprite;
            StartCoroutine(ShowImage());
        }
        
        void ImageSizeCalculate(Sprite sprite)
        {
            var width = sprite.bounds.size.x;
            var height = sprite.bounds.size.y;
            var aspectRatio = Mathf.Max(width ,height) / Mathf.Min(width, height);
            if (width > height)
            {
                width = MaxImageSize;
                height = MaxImageSize / aspectRatio;
            }
            else if (width < height)
            {
                height = MaxImageSize;
                width = MaxImageSize / aspectRatio;
            }
            else
            {
                width = MaxImageSize;
                height = MaxImageSize;
            }
            m_image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
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


        void ScaleImage(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        void TestShow(bool isShowed)
        {
            if (isShowed)
            {
                ImageSizeCalculate(testSprite);
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