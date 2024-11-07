using Data;
using Twitch;
using UnityEngine;

namespace UI.Greetings
{
    public class GreetingsLauncher : MonoBehaviour
    {
        [SerializeField] GreetingsImageUI greetingsImageUI;
        [SerializeField] GreeterNameUI greeterNameUI;
        readonly ChatUserData m_defaultChatterData = new() { UserName = "testUser_name", Color = Color.white };
        
        void OnEnable()
        {
            TwitchChatController.OnSayHello += Play;
        }

        void Play(ChatUserData chatterData)
        {
            // TODO нужно контролить более одного поздоровавшегося.
            if (greetingsImageUI.IsPlaying() || greeterNameUI.IsPlaying()) return;
            greetingsImageUI.gameObject.SetActive(true);
            greeterNameUI.gameObject.SetActive(true);
            greetingsImageUI.Play();
            greeterNameUI.Play(chatterData);
        }

        public void TestPlay()
        {
            if (greetingsImageUI.IsPlaying() || greeterNameUI.IsPlaying()) return;
            greetingsImageUI.gameObject.SetActive(true);
            greeterNameUI.gameObject.SetActive(true);
            greetingsImageUI.Play();
            greeterNameUI.Play(m_defaultChatterData);
        }

        void OnDisable()
        {
            TwitchChatController.OnSayHello -= Play;
        }
    }
}