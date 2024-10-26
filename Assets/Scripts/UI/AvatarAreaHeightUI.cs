using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AvatarAreaHeightUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            slider.value = m_settings.avatarAreaHeight;
            inputField.text = $"{(int)(m_settings.avatarAreaHeight * m_settings.windowHeight)}";
        }

        public void UserSlideValue(float value)
        {
            inputField.text = $"{(int)(value * m_settings.windowHeight)}";
            m_settings.avatarAreaHeight = value;
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var range = Mathf.Clamp(result, 0, m_settings.windowHeight);
                inputField.text = $"{(int)range}";
                var sliderValue = range / m_settings.windowHeight;
                slider.SetValueWithoutNotify(sliderValue);
                m_settings.avatarAreaHeight = sliderValue;
            }
        }
    }
}