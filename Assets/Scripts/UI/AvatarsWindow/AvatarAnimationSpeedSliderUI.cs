using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarAnimationSpeedSliderUI : MonoBehaviour
    {
        Slider m_slider;

        AvatarAnimationData m_avatarAnimationData;


        void Start()
        {
            m_slider = GetComponent<Slider>();
        }

        public void SetAvatarAnimationVariantData(AvatarAnimationData avatarAnimationData)
        {
            m_avatarAnimationData = avatarAnimationData;
            m_slider.value = m_avatarAnimationData.AnimationSpeed;
        }


        public void OnValueChanged(float value)
        {
            m_avatarAnimationData.AnimationSpeed = value;
        }
        
        public void OnValueChanged(string value)
        {
            float.TryParse(value, out var speed);
            m_slider.value = speed;
        }
    }
}