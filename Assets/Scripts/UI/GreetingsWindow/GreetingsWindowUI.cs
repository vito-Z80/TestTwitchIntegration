using System;
using Data;
using Images;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GreetingsWindow
{
    public class GreetingsWindowUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField greetingsInputField;
        [SerializeField] Slider greetingsSlider;
        [SerializeField] Image greetingsImage;
        [SerializeField] Toggle topLeftToggle;
        [SerializeField] Toggle topRightToggle;
        [SerializeField] Toggle bottomLeftToggle;
        [SerializeField] Toggle bottomRightToggle;
        [SerializeField] ToggleGroup toggleGroup;


        RectTransform m_greetingsImageTransform;
        ToastyGuy m_toastyGuy;

        TMP_Compatibility.AnchorPositions m_activeAnchor;
        AppSettingsData m_settings;


        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_greetingsImageTransform = greetingsImage.GetComponent<RectTransform>();
            m_toastyGuy = greetingsImage.GetComponent<ToastyGuy>();
            SetActiveToggle(m_settings.greetingsImageAnchor ?? TMP_Compatibility.AnchorPositions.BottomRight);
            greetingsSlider.SetValueWithoutNotify(m_settings.greetingsImageSize);
            SetGreetingsImageSize(m_settings.greetingsImageSize);
        }

        public void OnAnchorChanged()
        {
            var activeToggle = toggleGroup.GetFirstActiveToggle();

            if (activeToggle == topLeftToggle)
            {
                m_activeAnchor = TMP_Compatibility.AnchorPositions.TopLeft;
                m_greetingsImageTransform.pivot = new Vector2(1.0f, 0.0f);
                m_greetingsImageTransform.anchorMin = new Vector2(0.0f, 1.0f);
                m_greetingsImageTransform.anchorMax = new Vector2(0.0f, 1.0f);
                SetTopLeftTargetPosition();
            }
            else if (activeToggle == topRightToggle)
            {
                m_activeAnchor = TMP_Compatibility.AnchorPositions.TopRight;
                m_greetingsImageTransform.pivot = new Vector2(0.0f, 0.0f);
                m_greetingsImageTransform.anchorMin = new Vector2(1.0f, 1.0f);
                m_greetingsImageTransform.anchorMax = new Vector2(1.0f, 1.0f);
                SetTopRightTargetPosition();
            }
            else if (activeToggle == bottomLeftToggle)
            {
                m_activeAnchor = TMP_Compatibility.AnchorPositions.BottomLeft;
                m_greetingsImageTransform.pivot = new Vector2(1.0f, 1.0f);
                m_greetingsImageTransform.anchorMin = new Vector2(0.0f, 0.0f);
                m_greetingsImageTransform.anchorMax = new Vector2(0.0f, 0.0f);
                SetBottomLeftTargetPosition();
            }
            else if (activeToggle == bottomRightToggle)
            {
                m_activeAnchor = TMP_Compatibility.AnchorPositions.BottomRight;
                m_greetingsImageTransform.pivot = new Vector2(0.0f, 1.0f);
                m_greetingsImageTransform.anchorMin = new Vector2(1.0f, 0.0f);
                m_greetingsImageTransform.anchorMax = new Vector2(1.0f, 0.0f);
                SetBottomRightTargetPosition();
            }

            m_greetingsImageTransform.anchoredPosition = Vector2.zero;
            m_settings.greetingsImageAnchor = m_activeAnchor;
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

        public void SetGreetingsImageSize(float value)
        {
            var size = value * m_settings.windowHeight;
            var deltaSize = new Vector2(size, size);
            m_greetingsImageTransform.sizeDelta = deltaSize;
            greetingsInputField.SetTextWithoutNotify($"{(int)size}");
            m_settings.greetingsImageSize = value;
            OnAnchorChanged();
        }

        public void SetGreetingsImageSize(string value)
        {
            int.TryParse(value, out var sizeInt);

            var size = (int)Mathf.Clamp(sizeInt, m_settings.windowHeight * greetingsSlider.minValue, m_settings.windowHeight);
            var deltaSize = new Vector2(size, size);
            m_greetingsImageTransform.sizeDelta = deltaSize;
            m_settings.greetingsImageSize = (float)sizeInt / m_settings.windowHeight;
            greetingsSlider.SetValueWithoutNotify(m_settings.greetingsImageSize);
            greetingsInputField.SetTextWithoutNotify($"{size}");
            OnAnchorChanged();
        }

        void SetBottomRightTargetPosition()
        {
            m_toastyGuy.SetTargetPosition(new Vector3(-m_settings.windowHeight * m_settings.greetingsImageSize, m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetBottomLeftTargetPosition()
        {
            m_toastyGuy.SetTargetPosition(new Vector3(m_settings.windowHeight * m_settings.greetingsImageSize, m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetTopRightTargetPosition()
        {
            m_toastyGuy.SetTargetPosition(new Vector3(-m_settings.windowHeight * m_settings.greetingsImageSize, -m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        void SetTopLeftTargetPosition()
        {
            m_toastyGuy.SetTargetPosition(new Vector3(m_settings.windowHeight * m_settings.greetingsImageSize, -m_settings.greetingsImageSize * m_settings.windowHeight, 0.0f));
        }

        public void Play()
        {
            m_toastyGuy.SayHello(new ChatUserData { UserName = "test name" });
        }


        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}