using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarAreaWindow
{
    public class AvatarAreaYUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;


        float m_lastY;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_lastY = m_settings.areaPosY;
            slider.SetValueWithoutNotify(m_settings.areaPosY);
            inputField.text = $"{Mathf.RoundToInt(slider.value * m_settings.windowHeight)}";
        }


        void Update()
        {
            if (Mathf.Approximately(m_lastY, m_settings.areaPosY)) return;

            var normalizedAreaHeight = AvatarArea.Rect.height / Core.Instance.WorldSize.y;
            var newValue = Mathf.Clamp(m_settings.areaPosY, 0, 1.0f - normalizedAreaHeight);
            slider.SetValueWithoutNotify(newValue);
            m_settings.areaPosY = newValue;
            inputField.text = $"{Mathf.RoundToInt(newValue * m_settings.windowHeight)}";
            m_lastY = m_settings.areaPosY;
        }

        public void UserSlideValue(float value)
        {
            var normalizedAreaHeight = AvatarArea.Rect.height / Core.Instance.WorldSize.y;
            var newValue = Mathf.Clamp(value, 0, 1.0f - normalizedAreaHeight);
            slider.SetValueWithoutNotify(newValue);
            m_settings.areaPosY = newValue;
            inputField.text = $"{Mathf.RoundToInt(newValue * m_settings.windowHeight)}";
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var avatarsWorldRect = AvatarArea.Rect;
                var ppu = Core.Instance.Ppc.assetsPPU;
                var screenAreaHeight = avatarsWorldRect.size.y * ppu;
                var screenTopY = Mathf.Clamp(result, 0, m_settings.windowHeight - screenAreaHeight * 2.0f);
                inputField.text = $"{Mathf.RoundToInt(screenTopY)}";
                slider.SetValueWithoutNotify(screenTopY / m_settings.windowHeight);
                m_settings.areaPosY = slider.value;
            }
        }
    }
}