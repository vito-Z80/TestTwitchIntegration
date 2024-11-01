using UnityEngine;

namespace UI.AvatarAreaWindow
{
    public class AvatarAreaWindowUI : MonoBehaviour
    {
        
        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}