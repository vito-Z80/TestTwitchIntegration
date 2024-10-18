using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UserName : MonoBehaviour
    {
        Transform m_targetAvatar;
        TextMeshProUGUI m_userName;
        Canvas m_canvas;
        Camera m_camera;


        void Start()
        {
            transform.SetAsFirstSibling();
        }

        public void SetTargetAvatar(Transform targetAvatar, Canvas canvas, Camera camera, string userName)
        {
            m_camera = camera;
            m_canvas = canvas;
            m_targetAvatar = targetAvatar;
            m_userName ??= GetComponent<TextMeshProUGUI>();
            m_userName.text = userName.Replace("@", "");
        }


        void Update()
        {
            if (m_targetAvatar != null)
            {
                var avatarPosition = m_camera.WorldToScreenPoint(m_targetAvatar.position);
                transform.position = avatarPosition;
            }
        }
    }
}