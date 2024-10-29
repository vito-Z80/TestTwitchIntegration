using Avatars;
using Images;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShowHideButtonUI : MonoBehaviour
    {
        [SerializeField] AvatarsController avatarsController;
        [SerializeField] CentralImage centralImage;

        const string ShowLabel = "Show";
        const string HideLabel = "Hide";

        Text m_text;


        void Start()
        {
            m_text = GetComponentInChildren<Text>();
        }

        public void Show()
        {
            var value = m_text.text == ShowLabel;
            m_text.text = !value ? ShowLabel : HideLabel;

            centralImage.TestShow(value);
            avatarsController.ShowTest(value);
        }
    }
}