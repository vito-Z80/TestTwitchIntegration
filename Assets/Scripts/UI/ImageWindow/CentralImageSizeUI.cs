﻿using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ImageWindow
{
    public class CentralImageSizeUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Slider slider;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            slider.value = m_settings.imageScale;
            var maxSize = Mathf.Min(m_settings.windowWidth, m_settings.windowHeight);
            inputField.text = $"{(int)(m_settings.imageScale * maxSize)}";
        }

        public void UserSlideValue(float value)
        {
            var maxSize = Mathf.Min(m_settings.windowWidth, m_settings.windowHeight);
            inputField.text = $"{(int)(value * maxSize)}";
            m_settings.imageScale = value;
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var maxSize = Mathf.Min(m_settings.windowWidth, m_settings.windowHeight);
                var range = Mathf.Clamp(result, 0, maxSize);
                inputField.text = $"{(int)range}";
                var sliderValue = range / maxSize;
                slider.value = sliderValue;
                m_settings.imageScale = sliderValue;
            }
        }
    }
}