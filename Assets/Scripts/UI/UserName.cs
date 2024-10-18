using TMPro;
using UnityEngine;
using Avatar = Avatars.Avatar;

namespace UI
{
    public class UserName : MonoBehaviour
    {
        Avatar m_avatar;
        TextMeshProUGUI m_userName;
        Canvas m_canvas;
        Camera m_camera;


        void Start()
        {
            transform.SetAsFirstSibling();
        }

        public void SetTargetAvatar(Avatar targetAvatar, Canvas canvas, Camera camera, string userName)
        {
            m_camera = camera;
            m_canvas = canvas;
            m_avatar = targetAvatar;
            m_userName ??= GetComponent<TextMeshProUGUI>();
            m_userName.text = userName.Replace("@", "");
        }


        void Update()
        {
            SetNamePosition();
        }

        void SetNamePosition()
        {
            //  TODO work only if avatar sprite has pivot.y == 0.0f
            var avatarSprite = m_avatar.GetSpriteRenderer().sprite;
            if (avatarSprite is null)
            {
                transform.position = Vector3.up * 10000.0f;
                return;
            }
            var avatarWidth = avatarSprite.bounds.size.y;
            var avatarPosition = m_avatar.transform.position;
            avatarPosition.y += avatarWidth;
            avatarPosition = m_camera.WorldToScreenPoint(avatarPosition);
            transform.position = avatarPosition;
        }
    }
}