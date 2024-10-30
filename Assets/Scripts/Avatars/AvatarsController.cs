using System.Collections.Generic;
using Data;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.U2D;

namespace Avatars
{
    public class AvatarsController : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject avatarPrefab;
        [SerializeField] GameObject userNamePrefab;

        readonly Dictionary<string, Avatar> m_avatars = new(); //  string:userName, Avatar:avatar
        readonly Dictionary<string, UserName> m_names = new(); //  string:userName, Avatar:avatar


        PixelPerfectCamera m_pixelPerfectCamera;
        Camera m_camera;
        AvatarsStorage m_avatarsStorage;
        Sprite[] m_sprites;


        void Start()
        {
            m_pixelPerfectCamera = Core.Instance.Ppc;
            m_camera = Core.Instance.Camera;
            m_avatarsStorage = new AvatarsStorage();
            m_avatarsStorage.Init();
            m_sprites = m_avatarsStorage.GetSprites();
        }


        public string[] GetAvatarNames() => m_avatarsStorage.GetAvatarNames();

        void OnEnable()
        {
            TwitchChatController.OnAvatarStarted += StartAvatar;
            TwitchChatController.OnAvatarPursuit += PursuitAvatar;
        }

        public Sprite GetSprite(int id) => m_sprites[id];

        void PursuitAvatar(string attackerNickname, string targetNickname)
        {
            if (m_avatars.TryGetValue(attackerNickname, out var attackerAvatar))
            {
                if (attackerAvatar.HasState(AvatarState.AttackLeft) || attackerAvatar.HasState(AvatarState.AttackRight))
                {
                    if (m_avatars.TryGetValue(targetNickname, out var targetAvatar))
                    {
                        if (targetNickname != attackerNickname)
                        {
                            attackerAvatar.StartPursuit(targetAvatar);
                        }
                    }
                }
            }
        }


        void StartAvatar(ChatUserData userData, string avatarName)
        {
            // TODO пул аватаров...

            var userName = userData.UserName;
            if (m_avatars.TryGetValue(userName, out var existingAvatar))
            {
                if (existingAvatar.avatarName == avatarName) return;
                var avatarIndices = m_avatarsStorage.GetAvatar(avatarName);
                if (avatarIndices != null)
                {
                    m_avatars[userName].Init(m_pixelPerfectCamera, avatarName, this, avatarIndices);
                    m_names[userName].SetTargetAvatar(m_avatars[userName], canvas, m_camera, userData);
                }
            }
            else
            {
                if (CreateAvatar(userName, avatarName, Vector3.zero))
                {
                    CreateUserName(userData);
                }
            }
        }

        bool CreateAvatar(string userName, string avatarName, Vector3 position)
        {
            var avatarIndices = m_avatarsStorage.GetAvatar(avatarName);
            if (avatarIndices != null)
            {
                var newAvatar = Instantiate(avatarPrefab, position, Quaternion.identity, transform).GetComponent<Avatar>();
                newAvatar.Init(m_pixelPerfectCamera, avatarName, this, avatarIndices);
                m_avatars[userName] = newAvatar;
                return true;
            }

            return false;
        }

        void CreateUserName(ChatUserData userData)
        {
            var hisAvatar = m_avatars[userData.UserName];
            var avatarPosition = m_camera.WorldToScreenPoint(hisAvatar.transform.position);
            var un = Instantiate(userNamePrefab, avatarPosition, Quaternion.identity, canvas.transform).GetComponent<UserName>();
            un.SetTargetAvatar(hisAvatar, canvas, m_camera, userData);
            m_names[userData.UserName] = un;
        }

        public void ShowTest(bool isTest)
        {
            if (isTest)
            {
                for (var i = 0; i < 20; i++)
                {
                    
                    var testUserData = new ChatUserData
                    {
                        UserName = $"test_name_{i}",
                        Color = Color.white,
                        IsModerator = false,
                        IsSubscriber = false,
                        IsFirstMessage = false,
                        IsReturningChatter = true,
                    };
                    
                    StartAvatar(testUserData, "toad");
                }
            }
            else
            {
                //  TODO нинада так... пул сделай !!!

                foreach (var avatar in m_avatars.Values)
                {
                    Destroy(avatar.gameObject);
                }

                foreach (var value in m_names.Values)
                {
                    Destroy(value.gameObject);
                }

                m_avatars.Clear();
                m_names.Clear();
            }
        }

        void OnDestroy()
        {
            TwitchChatController.OnAvatarStarted -= StartAvatar;
            TwitchChatController.OnAvatarPursuit -= PursuitAvatar;
        }
    }
}