using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarAreaWindow
{
    public class AvatarAreaXUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;


        float m_lastX;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_lastX = m_settings.areaPosX;
            slider.SetValueWithoutNotify(m_settings.areaPosX);
            inputField.text = $"{Mathf.RoundToInt(slider.value * m_settings.windowWidth)}";
        }


        void Update()
        {
            if (Mathf.Approximately(m_lastX, m_settings.areaPosX)) return;

            var normalizedAreaWidth = AvatarArea.Rect.width / Core.Instance.WorldSize.x;
            var newValue = Mathf.Clamp(m_settings.areaPosX, 0, 1.0f - normalizedAreaWidth);
            slider.SetValueWithoutNotify(newValue);
            m_settings.areaPosX = newValue;
            inputField.text = $"{Mathf.RoundToInt(newValue * m_settings.windowWidth)}";

            m_lastX = m_settings.areaPosX;
        }

        public void UserSlideValue(float value)
        {
            var normalizedAreaWidth = AvatarArea.Rect.width / Core.Instance.WorldSize.x;
            var newValue = Mathf.Clamp(value, 0, 1.0f - normalizedAreaWidth);
            slider.SetValueWithoutNotify(newValue);
            m_settings.areaPosX = newValue;
            inputField.text = $"{Mathf.RoundToInt(newValue * m_settings.windowWidth)}";
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var avatarsWorldRect = AvatarArea.Rect;
                var ppu = Core.Instance.Ppc.assetsPPU;
                var screenAreaWidth = avatarsWorldRect.size.x * ppu;
                var screenLeftX = Mathf.Clamp(result, 0, m_settings.windowWidth - screenAreaWidth * 2.0f);
                inputField.text = $"{Mathf.RoundToInt(screenLeftX)}";
                slider.SetValueWithoutNotify(screenLeftX / m_settings.windowWidth);
                m_settings.areaPosX = slider.value;
            }
        }
    }
}