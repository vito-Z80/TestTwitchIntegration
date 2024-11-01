using Data;
using TMPro;
using Twitch;
using UnityEngine;

namespace Images
{
    public class ToastyGuy : MonoBehaviour
    {
        Animator m_toastyGuyAnimator;
        int m_toastyGuyAnimationId;
        Animator m_userNameAnimator;
        int m_userNameAnimationId;
        AudioSource m_audio;
        [SerializeField] TextMeshProUGUI userNameLabel;

        void OnEnable()
        {
            TwitchChatController.OnSayHello += SayToasty;
        }


        void Start()
        {
            m_toastyGuyAnimator = GetComponent<Animator>();
            m_userNameAnimator = userNameLabel.GetComponent<Animator>();
            m_userNameAnimationId = Animator.StringToHash("ShowName");
            m_toastyGuyAnimationId = Animator.StringToHash("Toasty");
            m_audio = GetComponent<AudioSource>();
        }

        void SayToasty(ChatUserData userData)
        {
            if (LocalStorage.GetSettings().displayNicknameColor)
            {
                userNameLabel.color = userData.Color;
            }

            userNameLabel.text = userData.UserName;
            m_toastyGuyAnimator.SetTrigger(m_toastyGuyAnimationId);
            m_userNameAnimator.SetTrigger(m_userNameAnimationId);
        }

        public void PlaySound()
        {
            m_audio.Play();
        }

        void OnDisable()
        {
            TwitchChatController.OnSayHello -= SayToasty;
        }
    }
}