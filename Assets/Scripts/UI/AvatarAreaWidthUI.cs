using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AvatarAreaWidthUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            slider.value = m_settings.avatarAreaWidth;
            inputField.text = $"{(int)(m_settings.avatarAreaWidth * m_settings.windowWidth)}";
        }

        public void UserSlideValue(float value)
        {
            inputField.text = $"{(int)(value * m_settings.windowWidth)}";
            m_settings.avatarAreaWidth = value;
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var range = Mathf.Clamp(result, 0, m_settings.windowWidth);
                inputField.text = $"{(int)range}";
                var sliderValue = range / m_settings.windowWidth;
                slider.value = sliderValue;
                m_settings.avatarAreaWidth = sliderValue;
            }
        }
    }
}