using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageX : MonoBehaviour
    {
        [SerializeField] Slider imageX;
        [SerializeField] TMP_InputField inputField;


        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            imageX.value = m_settings.imageX;
        }

        public void UserSlideValue(float value)
        {
            var imageScale = m_settings.imageScale * m_settings.windowHeight / m_settings.windowWidth;
            var correctValue = Mathf.Clamp(value, 0f, 1.0f - imageScale);
            m_settings.imageX = correctValue;
            imageX.SetValueWithoutNotify(correctValue);
            inputField.text = $"{Mathf.RoundToInt(correctValue * m_settings.windowWidth)}";
        }

        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var imageScale = m_settings.imageScale * m_settings.windowHeight / m_settings.windowWidth;
                var sliderValue = Mathf.Clamp(result / m_settings.windowWidth, 0f, 1f - imageScale);
                var fieldValue = Mathf.RoundToInt(sliderValue * m_settings.windowWidth);
                imageX.SetValueWithoutNotify(sliderValue);
                m_settings.imageX = sliderValue;
                inputField.text = fieldValue.ToString();
            }
        }
    }
}