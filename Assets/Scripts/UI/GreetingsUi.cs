using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GreetingsUi : MonoBehaviour
    {
        
        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            
            
            
        }
    }
}