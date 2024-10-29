using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageY : MonoBehaviour
    {
        [SerializeField] Slider imageY;
        [SerializeField] TMP_InputField inputField;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            imageY.value = m_settings.imageY;
        }

        public void UserSlideValue(float value)
        {
            var correctValue = Mathf.Clamp(value, 0f, 1.0f - m_settings.imageScale);
            m_settings.imageY = correctValue;
            imageY.SetValueWithoutNotify(correctValue);
            inputField.text = $"{Mathf.RoundToInt(correctValue * m_settings.windowHeight)}";
        }
        
        public void UserSetValue(string value)
        {
            if (float.TryParse(value, out var result))
            {
                var sliderValue = Mathf.Clamp(result / m_settings.windowHeight, 0f, 1f - m_settings.imageScale);
                var fieldValue = Mathf.RoundToInt(sliderValue * m_settings.windowHeight);
                imageY.SetValueWithoutNotify(sliderValue);
                m_settings.imageY = sliderValue;
                inputField.text = fieldValue.ToString();
            }
        }
    }
}