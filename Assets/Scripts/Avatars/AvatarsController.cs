using System;
using System.Collections.Generic;
using Data;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

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
        public Rect avatarsArea;
        public Rect fullArea;
        Sprite[] m_sprites;


        void Start()
        {
            m_pixelPerfectCamera = Launcher.Instance.Ppc;
            m_camera = Launcher.Instance.Camera;
            m_avatarsStorage = new AvatarsStorage();
            m_avatarsStorage.Init();
            m_sprites = m_avatarsStorage.GetSprites();
            TwitchChatController.OnAvatarStarted += StartAvatar;
            TwitchChatController.OnAvatarPursuit += PursuitAvatar;
        }

        void Update()
        {
            var worldSize = Launcher.Instance.WorldSize;
            fullArea.position = (Vector2)m_pixelPerfectCamera.transform.position - worldSize * 0.5f;
            fullArea.size = worldSize;
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
        

        void StartAvatar(string userName, string avatarName)
        {
            // TODO пул аватаров...

            if (m_avatars.TryGetValue(userName, out var existingAvatar))
            {
                if (existingAvatar.avatarName == avatarName) return;
                var avatarIndices = m_avatarsStorage.GetAvatar(avatarName);
                if (avatarIndices != null)
                {
                    m_avatars[userName].Init(m_pixelPerfectCamera, avatarName, this, avatarIndices);
                    m_names[userName].SetTargetAvatar(m_avatars[userName], canvas, m_camera, userName);
                }
            }
            else
            {
                if (CreateAvatar(userName, avatarName, Vector3.zero))
                {
                    CreateUserName(userName);
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

        void CreateUserName(string userName)
        {
            var hisAvatar = m_avatars[userName];
            var avatarPosition = m_camera.WorldToScreenPoint(hisAvatar.transform.position);
            var un = Instantiate(userNamePrefab, avatarPosition, Quaternion.identity, canvas.transform).GetComponent<UserName>();
            un.SetTargetAvatar(hisAvatar, canvas, m_camera, userName);
            m_names[userName] = un;
        }

        void SetAvatarsRect(Rect rect)
        {
            avatarsArea.position = rect.position;
            avatarsArea.size = rect.size;
        }

        void OnEnable()
        {
            Configuration.OnShowTest += ShowTestAvatarsTest;
            Configuration.OnRandomSpeed += SetRandomSpeedEachAvatar;
            AvatarArea.OnRectSizeChanged += SetAvatarsRect;
        }


        void OnDisable()
        {
            Configuration.OnShowTest -= ShowTestAvatarsTest;
            AvatarArea.OnRectSizeChanged -= SetAvatarsRect;
        }

        void SetRandomSpeedEachAvatar(bool isRandom)
        {
            foreach (var avatar in m_avatars.Values)
            {
                var randomSpeed = isRandom ? Random.Range(0.5f, 3.0f) : 1.0f;
                avatar.randomSpeed = randomSpeed;
            }
        }

        void ShowTestAvatarsTest(bool isTest)
        {
            if (isTest)
            {
                for (var i = 0; i < 20; i++)
                {
                    StartAvatar($"testName_{i}", "toad");
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
        }
    }
}