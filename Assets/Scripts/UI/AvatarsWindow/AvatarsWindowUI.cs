using System;
using System.Linq;
using Avatars;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarsWindowUI : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown avatarsList;
        [SerializeField] TMP_Dropdown avatarStates;
        [SerializeField] TMP_Dropdown avatarStateVariant;

        [SerializeField] Image avatarImage;
        [SerializeField] Image avatarImageFrame;

        [SerializeField] AvatarAnimationSpeedSliderUI animationSpeedSlider;
        [SerializeField] AvatarAnimationSpeedInputUI animationSpeedInput;
        [SerializeField] AvatarMovementSpeedSliderUI avatarMovementSpeedSlider;
        [SerializeField] AvatarMovementSpeedInputUI avatarMovementSpeedInput;


        AvatarData m_avatarData;
        AvatarAnimationData m_avatarAnimationData;
        int[] m_animationIndices;

        public int avatarIndex;
        public int avatarStateIndex;
        public int avatarStateVariantIndex;

        int m_currentFrame;
        float m_accumulatedTime;

        void Start()
        {
            RefreshAvatarList();
            SelectAvatar(0);
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
            m_avatarData = GetAvatarData();
            var stateNames = m_avatarData.Animations.Keys.Select(s => s.ToString()).ToList();
            avatarStates.AddOptions(stateNames);
            avatarStates.RefreshShownValue();
        }

        void RefreshAvatarStateVariant()
        {
            avatarStateVariant.ClearOptions();
            var avatarState = Enum.Parse<AvatarState>(avatarStates.options[avatarStateIndex].text);
            if (m_avatarData.Animations.TryGetValue(avatarState, out var variants))
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
            var avatarState = Enum.Parse<AvatarState>(avatarStates.options[avatarStateIndex].text);
            if (m_avatarData.Animations.TryGetValue(avatarState, out var animationIndices))
            {
                m_avatarAnimationData = animationIndices[avatarStateVariantIndex];
                m_animationIndices = m_avatarAnimationData.AnimationIndices;
            }

            animationSpeedSlider.SetAvatarAnimationVariantData(m_avatarAnimationData);
            animationSpeedInput.SetAvatarAnimationVariantData(m_avatarAnimationData);
            avatarMovementSpeedSlider.SetAvatarAnimationVariantData(m_avatarAnimationData);
            avatarMovementSpeedInput.SetAvatarAnimationVariantData(m_avatarAnimationData);
        }


        void PlayAnimationVariant()
        {
            var animationFrameTime = 1.0f / m_avatarAnimationData.AnimationSpeed;
            m_accumulatedTime += Time.deltaTime;

            if (m_accumulatedTime < animationFrameTime)
            {
                return;
            }

            m_accumulatedTime -= animationFrameTime;
            m_currentFrame++;
            if (m_currentFrame == m_animationIndices.Length)
            {
                m_currentFrame = 0;
            }

            var spriteIndex = m_animationIndices[m_currentFrame];
            SetAnimationFrame(AvatarsStorage.GetSprites()[spriteIndex]);
        }

        void SetAnimationFrame(Sprite sprite)
        {
            var ppu = 32.0f;
            var frameOffset = 12.0f;
            var width = sprite.bounds.size.x * ppu;
            var height = sprite.bounds.size.y * ppu;
            avatarImage.sprite = sprite;
            var imageSize = new Vector2(width, height);
            var imageFrameSize = new Vector2(width + frameOffset, height + frameOffset);
            avatarImage.rectTransform.sizeDelta = imageSize;
            avatarImageFrame.rectTransform.sizeDelta = imageFrameSize;
        }

        AvatarData GetAvatarData()
        {
            var avatarName = AvatarsStorage.GetAvatarNames()[avatarIndex];
            return AvatarsStorage.GetAvatarData(avatarName);
        }

        void Update()
        {
            if (m_animationIndices == null) return;
            PlayAnimationVariant();
        }
    }
}