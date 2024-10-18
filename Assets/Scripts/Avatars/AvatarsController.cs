using System;
using System.Collections.Generic;
using System.Linq;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

namespace Avatars
{
    public class AvatarsController : MonoBehaviour
    {
        [SerializeField] PixelPerfectCamera pixelPerfectCamera;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject avatarPrefab;
        [SerializeField] GameObject userNamePrefab;

        readonly Dictionary<string, Avatar> m_avatars = new(); //  string:userName, Avatar:avatar
        readonly Dictionary<string, UserName> m_names = new(); //  string:userName, Avatar:avatar


        Camera m_camera;
        AvatarsStorage m_avatarsStorage;
        Vector2Int m_screenSize;
        [HideInInspector] public Rect avatarsArea;

        Sprite[] m_sprites;


        void Start()
        {
            m_camera = pixelPerfectCamera.GetComponent<Camera>();
            m_avatarsStorage = new AvatarsStorage();
            m_avatarsStorage.Init();
            m_sprites = m_avatarsStorage.GetSprites();
            avatarsArea = Rect.zero;
            var size = GetOffScreenSize() / pixelPerfectCamera.assetsPPU;
            var position = new Vector2(pixelPerfectCamera.transform.position.x - size.x / 2, pixelPerfectCamera.transform.position.y - size.y / 2);
            avatarsArea.position = position;
            avatarsArea.size = size;
            TwitchChatController.OnAvatarStarted += StartAvatar;
            TwitchChatController.OnAvatarPursuit += PursuitAvatar;

            // for (var i = 0; i < 1; i++)
            // {
            //     StartAvatar($"petya{i}", "toad");
            //     StartAvatar($"manya{i}", "scull");
            // }
        }

        public Sprite GetSprite(int id) => m_sprites[id];

        void PursuitAvatar(string attackerNickname, string targetNickname)
        {
            // Debug.Log($"Pursuit {attackerNickname} to {targetNickname}");
            if (m_avatars.TryGetValue(attackerNickname, out var attackerAvatar))
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


        void Update()
        {
            ResizeArea();
        }

        void ResizeArea()
        {
            if (WasWindowResized())
            {
                var size = GetOffScreenSize() / pixelPerfectCamera.assetsPPU;
                var position = new Vector2(pixelPerfectCamera.transform.position.x - size.x / 2, pixelPerfectCamera.transform.position.y - size.y / 2);
                avatarsArea.position = position;
                avatarsArea.size = size;
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
                    m_avatars[userName].Init(pixelPerfectCamera, avatarName, this, avatarIndices);
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
                newAvatar.Init(pixelPerfectCamera, avatarName, this, avatarIndices);
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

        Vector2 GetOffScreenSize()
        {
            int refResolutionX = pixelPerfectCamera.refResolutionX;
            int refResolutionY = pixelPerfectCamera.refResolutionY;

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // zoom level (PPU scale)
            int verticalZoom = screenHeight / refResolutionY;
            int horizontalZoom = screenWidth / refResolutionX;
            var zoom = Math.Max(1, Math.Min(verticalZoom, horizontalZoom));

            var offscreenRTWidth = screenWidth / zoom / 2 * 2;
            var offscreenRTHeight = screenHeight / zoom / 2 * 2;

            return new Vector2(offscreenRTWidth, offscreenRTHeight);
        }

        bool WasWindowResized()
        {
            if (Screen.width == m_screenSize.x && Screen.height == m_screenSize.y) return false;
            m_screenSize.x = Screen.width;
            m_screenSize.y = Screen.height;
            return true;
        }

        void OnDestroy()
        {
            TwitchChatController.OnAvatarStarted -= StartAvatar;
        }
    }
}