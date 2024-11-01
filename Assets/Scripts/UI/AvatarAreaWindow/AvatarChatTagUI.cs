using Data;
using TMPro;
using UnityEngine;

namespace UI.AvatarAreaWindow
{
    public class AvatarChatTagUI : MonoBehaviour
    {
    
        [SerializeField] TMP_InputField inputField;

        AppSettingsData m_settings;


        void Start()
        {
            m_settings = LocalStorage.GetSettings();
            inputField.text = m_settings.avatarNameTag.Trim();
        }

        public void SetChatTag(string chatTag)
        {
            m_settings.avatarNameTag = chatTag.Trim();
        }
    }
}