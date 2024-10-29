using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ImageChatTagUI : MonoBehaviour
    {
    
        [SerializeField] TMP_InputField inputField;

        AppSettingsData m_settings;


        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            inputField.text = m_settings.imageNameTag.Trim();
        }

        public void SetChatTag(string chatTag)
        {
            m_settings.imageNameTag = chatTag.Trim();
        }
    }
}