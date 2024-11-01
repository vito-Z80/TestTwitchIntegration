using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarAreaWindow
{
    public class UseAvatarsToggle : MonoBehaviour
    {
        Toggle m_toggle;
        AppSettingsData m_settings;
        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = m_settings.useAvatars;
        }

        public void OnValueChanged(bool value)
        {
            m_settings.useAvatars = value;
        }
    }
}