using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AvatarsGeneralSpeedUI : MonoBehaviour
    {
    
        [SerializeField] Slider generalSpeedSlider;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            generalSpeedSlider.value = m_settings.avatarsSpeed;
        }

        public void UserChangeSpeed(float value)
        {
            m_settings.avatarsSpeed = value;
        }
        
    }
}