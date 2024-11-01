using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarAreaWindow
{
    public class AvatarsScaleUI : MonoBehaviour
    {
    
        
        [SerializeField] Slider avatarsScaleSlider;

        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            avatarsScaleSlider.value = m_settings.cameraPpu;
        }

        public void UserSlideValue(float value)
        {
            m_settings.cameraPpu = (int)value;
        }
    }
}