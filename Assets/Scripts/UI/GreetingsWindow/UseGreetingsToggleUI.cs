using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GreetingsWindow
{
    public class UseGreetingsToggleUI : MonoBehaviour
    {
    
        Toggle m_toggle;
        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = m_settings.useGreetings;
        }

        public void OnValueChanged(bool value)
        {
            m_settings.useGreetings = value;
        }
    }
}