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
        [Header("Dropdown")] [SerializeField] TMP_Dropdown avatarsList;
        [SerializeField] TMP_Dropdown avatarStates;
        [SerializeField] TMP_Dropdown avatarStateVariant;

        [Header("Images")] [SerializeField] Image avatarImage;
        [SerializeField] Image avatarImageFrame;

        [Header("Sliders")] [SerializeField] AvatarAnimationSpeedSliderUI animationSpeedSlider;
        [SerializeField] AvatarAnimationSpeedInputUI animationSpeedInput;
        [SerializeField] AvatarMovementSpeedSliderUI avatarMovementSpeedSlider;
        [SerializeField] AvatarMovementSpeedInputUI avatarMovementSpeedInput;

        [Header("Toggles")] [SerializeField] AvatarAccessTogglesUI accessToggles;

        const float FrameSizeOffset = 12.0f;

        float m_avatarVisibilitySize = 32.0f;

        public static event Action<string> OnAvatarChanged;

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
            accessToggles.SetToggleValues(m_avatarData);
            OnAvatarChanged?.Invoke(m_avatarData.AvatarName);
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
            avatarImage.sprite = AvatarsStorage.GetSprites()[spriteIndex];
            SizeCorrection(avatarImage.sprite);
        }

        void SizeCorrection(Sprite sprite)
        {
            var width = sprite.bounds.size.x * m_avatarVisibilitySize;
            var height = sprite.bounds.size.y * m_avatarVisibilitySize;
            var imageSize = new Vector2(width, height);
            var imageFrameSize = new Vector2(width + FrameSizeOffset, height + FrameSizeOffset);
            avatarImage.rectTransform.sizeDelta = imageSize;
            avatarImageFrame.rectTransform.sizeDelta = imageFrameSize;
        }

        AvatarData GetAvatarData()
        {
            var avatarName = AvatarsStorage.GetAvatarNames()[avatarIndex];
            return AvatarsStorage.GetAvatarData(avatarName);
        }

        public void OnAccessTogglesValueChanged()
        {
            m_avatarData.Access = Utils.GetAccessValue(accessToggles.GetUserData());
        }

        void Update()
        {
            if (m_animationIndices == null) return;
            PlayAnimationVariant();
        }

        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}