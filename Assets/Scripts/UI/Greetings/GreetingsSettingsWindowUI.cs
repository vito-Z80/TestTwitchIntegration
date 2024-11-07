using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Greetings
{
    public class GreetingsSettingsWindowUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField greetingsInputField;
        [SerializeField] Slider greetingsSlider;

        [SerializeField] Toggle topLeftToggle;
        [SerializeField] Toggle topRightToggle;
        [SerializeField] Toggle bottomLeftToggle;
        [SerializeField] Toggle bottomRightToggle;
        [SerializeField] ToggleGroup toggleGroup;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            SetActiveToggle(m_settings.greetingsImageAnchor ?? TMP_Compatibility.AnchorPositions.BottomRight);
            greetingsSlider.SetValueWithoutNotify(m_settings.greetingsImageSize);
            greetingsInputField.SetTextWithoutNotify($"{(int)(m_settings.greetingsImageSize * m_settings.windowHeight)}");
        }


        public void OnAnchorChanged()
        {
            var activeToggle = toggleGroup.GetFirstActiveToggle();

            if (activeToggle == topLeftToggle)
            {
                m_settings.greetingsImageAnchor = TMP_Compatibility.AnchorPositions.TopLeft;
            }
            else if (activeToggle == topRightToggle)
            {
                m_settings.greetingsImageAnchor = TMP_Compatibility.AnchorPositions.TopRight;
            }
            else if (activeToggle == bottomLeftToggle)
            {
                m_settings.greetingsImageAnchor = TMP_Compatibility.AnchorPositions.BottomLeft;
            }
            else if (activeToggle == bottomRightToggle)
            {
                m_settings.greetingsImageAnchor = TMP_Compatibility.AnchorPositions.BottomRight;
            }
        }

        public void OnImageSizeChanged(float value)
        {
            m_settings.greetingsImageSize = value;
            greetingsInputField.SetTextWithoutNotify($"{(int)(m_settings.greetingsImageSize * m_settings.windowHeight)}");
        }

        public void OnImageSizeChanged(string value)
        {
            if (float.TryParse(value, out var size))
            {
                size = Mathf.Clamp(size, greetingsSlider.minValue * m_settings.windowHeight, m_settings.windowHeight);
                greetingsInputField.SetTextWithoutNotify($"{(int)size}");
                m_settings.greetingsImageSize = size / m_settings.windowHeight;
                greetingsSlider.SetValueWithoutNotify(m_settings.greetingsImageSize);
            }
            else
            {
                greetingsInputField.SetTextWithoutNotify($"{(int)(m_settings.greetingsImageSize * m_settings.windowHeight)}");
            }
        }

        void SetActiveToggle(TMP_Compatibility.AnchorPositions anchor)
        {
            switch (anchor)
            {
                case TMP_Compatibility.AnchorPositions.TopLeft:
                    topLeftToggle.isOn = true;
                    break;
                case TMP_Compatibility.AnchorPositions.TopRight:
                    topRightToggle.isOn = true;
                    break;
                case TMP_Compatibility.AnchorPositions.BottomLeft:
                    bottomLeftToggle.isOn = true;
                    break;
                case TMP_Compatibility.AnchorPositions.BottomRight:
                    bottomRightToggle.isOn = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
            }
        }

        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}