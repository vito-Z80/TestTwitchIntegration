using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avatars;
using Data;
using JetBrains.Annotations;
using Twitch;
using UnityEngine;

namespace UI.Interaction
{
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] InteractionImageUI interactionImageUI;
        [SerializeField] AvatarsController avatarsController;
        readonly Dictionary<string, string> m_interactionNames = new();


        void OnEnable()
        {
            TwitchChatController.OnChattersInteraction += Interact;
        }

        void Start()
        {
            foreach (var interactionPath in LocalStorage.GetInteractionPaths())
            {
                var folderName = new DirectoryInfo(interactionPath).Name.ToLower();
                m_interactionNames.Add(folderName, interactionPath);
            }
        }

        public string[] GetInteractionNames() => m_interactionNames.Keys.ToArray();

        void Interact(ChatUserData data1, ChatUserData data2, string chatMessage)
        {
            var interactionName = FindWordInString(chatMessage, m_interactionNames.Keys.ToArray());
            if (interactionName == null) return;
            _ = InteractProcess(data1, data2, interactionName);
        }

        async Task InteractProcess(ChatUserData data1, ChatUserData data2, string interactionName)
        {
            if (!await avatarsController.DirectAvatars(data1.UserName, data2.UserName)) return;
            //  show interaction animation
            interactionImageUI.gameObject.SetActive(true);
            await interactionImageUI.ShowImage(m_interactionNames[interactionName]);
            await avatarsController.WaitInteraction(data1.UserName, data2.UserName);

            interactionImageUI.gameObject.SetActive(false);
            // await interactionImageUI.ShowImage(data1, data2, interactionName);
        }

        [CanBeNull]
        string FindWordInString(string input, string[] words)
        {
            var pattern = @"\b(" + string.Join("|", words.Select(Regex.Escape)) + @")\b";
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Value : null;
        }


        void OnDisable()
        {
            TwitchChatController.OnChattersInteraction -= Interact;
        }
    }
}