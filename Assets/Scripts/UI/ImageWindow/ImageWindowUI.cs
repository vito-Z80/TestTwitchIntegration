using UnityEngine;

namespace UI.ImageWindow
{
    public class ImageWindowUI : MonoBehaviour
    {
        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}