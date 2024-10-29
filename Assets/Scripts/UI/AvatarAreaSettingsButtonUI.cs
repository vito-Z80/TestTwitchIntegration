using UnityEngine;

namespace UI
{
    public class AvatarAreaSettingsButtonUI : MonoBehaviour
    {
        [SerializeField] GameObject settingsWindow;

        public void OnClick()
        {
            settingsWindow.SetActive(!settingsWindow.activeSelf);
            if (settingsWindow.activeSelf) settingsWindow.transform.SetAsLastSibling();
        }
    }
}