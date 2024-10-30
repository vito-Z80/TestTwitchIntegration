using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UseImagesToggle : MonoBehaviour
    {
        Toggle m_toggle;
        AppSettingsData m_settings;
        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = m_settings.useImages;
        }

        public void OnValueChanged(bool value)
        {
            m_settings.useImages = value;
        }
    }
}