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
        
        Image m_image;
        bool m_isShowed;

        void Start()
        {
            transform.position = Vector3.one * 10000;
            m_image = GetComponentInChildren<Image>();
        }

        public async Task Show(string message, ImageData[] imageData)
        {
            if (m_isShowed) return;
            var sprite = await ParseMessage(message, imageData);
            if (sprite is null) return;
            m_isShowed = true;
            m_image.sprite = sprite;
            // m_image.SetNativeSize();
            SetImageSize();
            StartCoroutine(ShowImage());
        }

        void SetImageSize()
        {
            RectTransform rt = m_image.GetComponent<RectTransform>();

            // Параметры пиксельной камеры
            float cameraPPU = 16f; // Pixels Per Unit для пиксельной камеры
            float worldSize = 128f / cameraPPU; // Размер текстуры в мировых единицах

            // Масштаб экрана
            float screenScaleX = Screen.width / (2f * pixelCamera.orthographicSize * pixelCamera.aspect);
            float screenScaleY = Screen.height / (2f * pixelCamera.orthographicSize);

            // Размер изображения в пикселях на канвасе
            float sizeX = worldSize * screenScaleX;
            float sizeY = worldSize * screenScaleY;

            // Устанавливаем размер изображения в пикселях
            rt.sizeDelta = new Vector2(sizeX, sizeY);
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
            m_isShowed = false;
            yield return null;
        }
    }
}