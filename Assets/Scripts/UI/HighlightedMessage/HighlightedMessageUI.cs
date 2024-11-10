using Data;
using Twitch;
using UnityEngine;

namespace UI.HighlightedMessage
{
    public class HighlightedMessageUI : MonoBehaviour
    {
        [SerializeField] GameObject highlightedMessagePrefab;

        void OnEnable()
        {
            TwitchChatController.OnHighlightedMessage += Show;
        }

        void OnDisable()
        {
            TwitchChatController.OnHighlightedMessage -= Show;
        }

        void Show(ChatUserData userDate, string message)
        {
            var handler = Instantiate(highlightedMessagePrefab, transform).GetComponent<HighlightedMessageHandler>();
            handler.Show(userDate, message);
        }
    }
}