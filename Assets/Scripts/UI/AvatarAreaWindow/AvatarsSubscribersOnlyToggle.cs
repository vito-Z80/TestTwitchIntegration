using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarAreaWindow
{
    public class AvatarsSubscribersOnlyToggle : MonoBehaviour
    {

        Toggle m_toggle;
        AppSettingsData m_settings;
        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = m_settings.avatarSubscribersOnly;
        }

        public void OnValueChanged(bool value)
        {
            m_settings.avatarSubscribersOnly = value;
        }
    }
}