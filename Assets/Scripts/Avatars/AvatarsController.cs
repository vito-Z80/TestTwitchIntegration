﻿using System;
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
        Vector2Int m_screenSize;
        public Rect avatarsArea;

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

        Vector2 GetOffScreenSize()
        {
            int refResolutionX = m_pixelPerfectCamera.refResolutionX;
            int refResolutionY = m_pixelPerfectCamera.refResolutionY;

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
            m_screenSize.x = Screen.width / 2 * 2;
            m_screenSize.y = Screen.height / 2 * 2;
            Screen.SetResolution(m_screenSize.x, m_screenSize.y, FullScreenMode.Windowed);
            return true;
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