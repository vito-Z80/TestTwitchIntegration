using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarAnimationSpeedSliderUI : MonoBehaviour
    {
        Slider m_slider;

        AvatarAnimationData m_avatarAnimationData;


        public void SetAvatarAnimationVariantData(AvatarAnimationData avatarAnimationData)
        {
            m_avatarAnimationData = avatarAnimationData;
            m_slider ??= GetComponent<Slider>();
            m_slider.SetValueWithoutNotify(m_avatarAnimationData.AnimationSpeed);
        }


        public void OnValueChanged(float value)
        {
            m_avatarAnimationData.AnimationSpeed = value;
        }
        
        public void OnValueChanged(string value)
        {
            float.TryParse(value, out var speed);
            m_slider.SetValueWithoutNotify(speed);
        }
    }
}