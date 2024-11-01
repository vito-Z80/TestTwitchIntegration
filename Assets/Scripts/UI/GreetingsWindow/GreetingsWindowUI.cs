using Data;
using UnityEngine;

namespace UI.GreetingsWindow
{
    public class GreetingsWindowUI : MonoBehaviour
    {
        AppSettingsData m_settings;

        void Start()
        {
            m_settings = LocalStorage.GetSettings();
        }


        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}