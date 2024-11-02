using System;
using Avatars;
using Images;
using UI.AvatarsWindow;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TopMenu
{
    public class ShowHideButtonUI : MonoBehaviour
    {
        [SerializeField] AvatarsController avatarsController;
        [SerializeField] CentralImage centralImage;

        const string ShowLabel = "Show";
        const string HideLabel = "Hide";

        string m_displayAvatarName;
        
        Text m_text;


        void OnEnable()
        {
            AvatarsWindowUI.OnAvatarChanged += SetTestAvatarName;
        }

        void SetTestAvatarName(string avatarName)
        {
            m_displayAvatarName = avatarName;
        }

        void Start()
        {
            m_text = GetComponentInChildren<Text>();
            m_displayAvatarName = AvatarsStorage.GetAvatarNames()[0];
        }

        public void Show()
        {
            var value = m_text.text == ShowLabel;
            m_text.text = !value ? ShowLabel : HideLabel;

            centralImage.TestShow(value);
            avatarsController.ShowTest(value,m_displayAvatarName);
        }

        void OnDisable()
        {
            AvatarsWindowUI.OnAvatarChanged -= SetTestAvatarName;
        }
    }
}