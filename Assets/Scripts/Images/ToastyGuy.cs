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
        [SerializeField] TextMeshProUGUI m_userName;

        void OnEnable()
        {
            TwitchChatController.OnSayHello += SayToasty;
        }

        
        void Start()
        {
            m_toastyGuyAnimator = GetComponent<Animator>();
            m_userNameAnimator = m_userName.GetComponent<Animator>();
            m_userNameAnimationId = Animator.StringToHash("ShowName");
            m_toastyGuyAnimationId = Animator.StringToHash("Toasty");
            m_audio = GetComponent<AudioSource>();
        }
        
        void SayToasty(string userName)
        {
            m_userName.text = userName.Replace("@", "");
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