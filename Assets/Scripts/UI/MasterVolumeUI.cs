using Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    public class MasterVolumeUI : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;
        Slider m_slider;
        AppSettingsData m_settings;
        
        const string MasterVolumeKey = "MasterVolume";

        void Start()
        {
            m_slider = GetComponent<Slider>();
            m_settings = LocalStorage.GetSettings();
            m_slider.value = m_settings.masterVolume;
        }
        
        public void SetMasterVolume(float volume)
        {
            m_settings.masterVolume = volume;
            audioMixer.SetFloat(MasterVolumeKey, volume);
        }
    }
}