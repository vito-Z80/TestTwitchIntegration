using System;
using System.Linq;
using Avatars;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarsWindowUI : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown avatarsList;
        [SerializeField] TMP_Dropdown avatarStates;
        [SerializeField] TMP_Dropdown avatarStateVariant;

        [SerializeField] Image avatarImage;


        AvatarData m_avatarData;
        int[] m_animationIndices;

        public int avatarIndex;
        public int avatarStateIndex;
        public int avatarStateVariantIndex;

        int m_currentFrame;
        float m_frameTime;

        void Start()
        {
            RefreshAvatarList();
            RefreshAvatarStates();
        }

        void RefreshAvatarList()
        {
            avatarsList.ClearOptions();
            avatarsList.AddOptions(AvatarsStorage.GetAvatarNames().ToList());
            avatarsList.RefreshShownValue();
        }

        void RefreshAvatarStates()
        {
            avatarStates.ClearOptions();
            var avatarName = AvatarsStorage.GetAvatarNames()[avatarIndex];
            m_avatarData = AvatarsStorage.GetAvatarData(avatarName);
            var stateNames = m_avatarData.Animations.Keys.Select(s => s.ToString()).ToList();
            avatarStates.AddOptions(stateNames);
            avatarStates.RefreshShownValue();
        }

        void RefreshAvatarStateVariant()
        {
            avatarStateVariant.ClearOptions();
            var avatarName = AvatarsStorage.GetAvatarNames()[avatarIndex];
            var avatarState = Enum.Parse<AvatarState>(avatarStates.options[avatarStateIndex].text);
            if (AvatarsStorage.GetAvatarData(avatarName).Animations.TryGetValue(avatarState, out var variants))
            {
                var variantNames = variants.Select(s => s.SubName).ToList();
                avatarStateVariant.AddOptions(variantNames);
                avatarStateVariant.RefreshShownValue();
            }
        }

        public void SelectAvatar(int index)
        {
            avatarIndex = index;
            avatarStateIndex = 0;
            RefreshAvatarStates();
            RefreshAvatarStateVariant();
            SelectAvatarVariant(0);
        }

        public void SelectAvatarState(int index)
        {
            avatarStateIndex = index;
            RefreshAvatarStateVariant();
            SelectAvatarVariant(0);
        }

        public void SelectAvatarVariant(int index)
        {
            avatarStateVariantIndex = index;
            m_currentFrame = 0;
            m_frameTime = 0.0f;
            var avatarState = Enum.Parse<AvatarState>(avatarStates.options[avatarStateIndex].text);
            if (m_avatarData.Animations.TryGetValue(avatarState, out var animationIndices))
            {
                m_animationIndices = animationIndices[avatarStateVariantIndex].AnimationIndices;
            }
        }


        void PlayAnimationVariant()
        {
            if (m_animationIndices == null) return;


            if (m_frameTime > 1.0f / 8.0f)
            {
                m_frameTime = 0.0f;
                m_currentFrame++;
                if (m_currentFrame == m_animationIndices.Length)
                {
                    m_currentFrame = 0;
                    // m_spriteRenderer.sprite = m_avatarsController.GetSprite(m_avatars[m_currentState][m_currentFrame]);
                    // return;
                }

                var spriteIndex = m_animationIndices[m_currentFrame];
                avatarImage.sprite = AvatarsStorage.GetSprites()[spriteIndex];
            }

            m_frameTime += Time.deltaTime * 1.45f;
        }


        void Update()
        {
            PlayAnimationVariant();
        }
    }
}