using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AvatarsRandomSpeedUI : MonoBehaviour
    {
        [SerializeField] Toggle randomSpeedToggle;

        
        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            randomSpeedToggle.isOn = m_settings.randomSpeedEnabled;
        }

        public void UserChangeRandomSpeed(bool isOn)
        {
            m_settings.randomSpeedEnabled = isOn;
        }
    }
}