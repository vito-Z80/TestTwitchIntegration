using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarsWindow
{
    public class AvatarMovementSpeedSliderUI : MonoBehaviour
    {
        Slider m_slider;

        AvatarAnimationData m_avatarAnimationData;

        

        public void SetAvatarAnimationVariantData(AvatarAnimationData avatarAnimationData)
        {
            m_avatarAnimationData = avatarAnimationData;
            m_slider ??= GetComponent<Slider>();
            m_slider.SetValueWithoutNotify(m_avatarAnimationData.AvatarSpeed);
        }


        public void OnValueChanged(float value)
        {
            m_avatarAnimationData.AvatarSpeed = value;
        }
        
        public void OnValueChanged(string value)
        {
            float.TryParse(value, out var speed);
            m_slider.SetValueWithoutNotify(speed);
        }
    }
}