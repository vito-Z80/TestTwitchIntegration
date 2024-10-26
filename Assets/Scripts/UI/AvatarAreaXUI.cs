using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AvatarAreaXUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;


        Rect m_lastRect;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            // slider.value = m_settings.areaPosX;
            // inputField.text = $"{(int)(m_settings.avatarAreaWidth * m_settings.windowWidth)}";
        }


        void Update()
        {
            var rect = AvatarArea.Rect;
            if (rect == m_lastRect) return;

            var ppu = Core.Instance.Ppc.assetsPPU;

            var x = rect.xMin * ppu * 2.0f + m_settings.windowWidth * 0.5f;
            inputField.text = $"{Mathf.RoundToInt(x)}";

            m_lastRect = rect;
        }

        public void UserSlideValue(float value)
        {
            // inputField.text = $"{(int)(value * m_settings.windowWidth)}";
            // m_settings.areaPosX = value;
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var rect = AvatarArea.Rect;
                var ppu = Core.Instance.Ppc.assetsPPU;
                var areaPixHalfWidth = rect.size.x * ppu;
                var leftX = Mathf.Clamp(result, 0, m_settings.windowWidth - areaPixHalfWidth * 2.0f);
                // Debug.Log($"{leftX} | {rect.xMin * ppu * 2.0f + m_settings.windowWidth * 0.5f}");
                inputField.text = $"{Mathf.RoundToInt(leftX)}";
                m_settings.areaPosX = Mathf.RoundToInt(leftX / ppu * 0.5f - areaPixHalfWidth);
                // var sliderValue = x / m_settings.windowWidth;
                slider.value = leftX / m_settings.windowWidth;
                var aaa = Core.Instance.WorldSize.x - Core.Instance.WorldSize.x / 2.0f + rect.size.x;
                Debug.Log($"{leftX} | {aaa}");
                // m_settings.avatarAreaWidth = sliderValue;
            }
        }
    }
}