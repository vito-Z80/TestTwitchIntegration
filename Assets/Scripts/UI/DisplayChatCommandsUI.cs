using System.Linq;
using System.Text;
using Avatars;
using Data;
using Images;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DisplayChatCommandsUI : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] ImageController imageController;
        [SerializeField] AvatarsController avatarsController;
        [SerializeField] TMP_InputField inputField;

        AppSettingsData m_settings;

        readonly StringBuilder m_text = new();


        void OnEnable()
        {
            m_settings = LocalStorage.GetSettings();
            if (imageController == null ||
                avatarsController == null) return;
            GenerateText();
            inputField.text = m_text.ToString();
        }

        void GenerateText()
        {
            var imageTag = m_settings.imageNameTag;
            var avatarTag = m_settings.avatarNameTag;
            m_text.Clear();
            Section("Images", imageController.GetImageNames().ToArray(), imageTag);
            Section("Avatars", avatarsController.GetAvatarNames().ToArray(), avatarTag);
        }

        void Section(string sectionName, string[] names, string sectionTag)
        {
            m_text.Append(GetSectionHeader(sectionName));
            foreach (var text in names)
            {
                var line = $"<b>{sectionTag}</b>{text}\n";
                m_text.Append(line);
            }
        }

        string GetSectionHeader(string sectionName)
        {
            return $"<align=right><color={Utils.GetHexColor(Color.blue)}><b>{sectionName}</b></color></align>\n";
        }

        public void SetVisible()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}