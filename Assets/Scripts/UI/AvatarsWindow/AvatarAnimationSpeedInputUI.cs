using Data;
using TMPro;
using UnityEngine;

namespace UI.AvatarsWindow
{
    public class AvatarAnimationSpeedInputUI : MonoBehaviour
    {
        TMP_InputField m_input;
        AvatarAnimationData m_avatarAnimationData;


        void Start()
        {
            m_input = GetComponent<TMP_InputField>();
        }

        public void SetAvatarAnimationVariantData(AvatarAnimationData avatarAnimationData)
        {
            m_avatarAnimationData = avatarAnimationData;
            m_input.text = $"{m_avatarAnimationData.AnimationSpeed}";
        }


        public void OnValueChanged(string value)
        {
            float.TryParse(value, out var speed);
            m_avatarAnimationData.AnimationSpeed = speed;
        }
        public void OnValueChanged(float value)
        {
            m_input.text = $"{value}";
        }
    }
}