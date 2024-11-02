using Data;
using TMPro;
using UnityEngine;

namespace UI.AvatarsWindow
{
    public class AvatarMovementSpeedInputUI : MonoBehaviour
    {
        TMP_InputField m_input;
        AvatarAnimationData m_avatarAnimationData;


        public void SetAvatarAnimationVariantData(AvatarAnimationData avatarAnimationData)
        {
            m_avatarAnimationData = avatarAnimationData;
            m_input ??= GetComponent<TMP_InputField>();
            m_input.SetTextWithoutNotify($"{m_avatarAnimationData.AvatarSpeed}");
        }


        public void OnValueChanged(string value)
        {
            float.TryParse(value, out var speed);
            m_avatarAnimationData.AvatarSpeed = speed;
        }

        public void OnValueChanged(float value)
        {
            m_input.SetTextWithoutNotify($"{value}");
        }
    }
}