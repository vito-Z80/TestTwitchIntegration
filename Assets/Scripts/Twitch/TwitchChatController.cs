using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using UnityEngine;

namespace Twitch
{
    public class TwitchChatController
    {
        readonly Dictionary<string, ChatUserData> m_chatters = new();

        public static Action<ChatUserData, string> OnAvatarStarted;
        public static Action<string, string> OnAvatarPursuit;
        public static Action<ChatUserData> OnSayHello;
        public static event Action<string> OnImageShown;
        public static event Action<ChatUserData, string> OnHighlightedMessage;


        const string HighlightedMsg = "highlighted-message";

        readonly ChatTagParser m_parser = new();

        AppSettingsData m_settings;

        public async Task ProcessMessage(string message)
        {
            var twitchMessage = m_parser.SplitMessageTags(message);
            if (twitchMessage.Length != 2) return;
            var chatTags = twitchMessage[0];
            var chatMessage = twitchMessage[1].Split(":")[1];

            m_settings ??= LocalStorage.GetSettings();

            var chatTagsParts = chatTags.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var userName = m_parser.GetUserName(chatTagsParts);

            await CreateUserData(userName, chatTagsParts);

            if (userName != null)
            {
                HighlightedMessage(userName, chatMessage);

                if (m_settings.useAvatars)
                {
                    await AttackUser(userName, chatMessage);
                    await StartAvatar(userName, chatMessage);
                }

                if (m_settings.useImages)
                {
                    await ShowImage(chatMessage);
                }

                if (m_settings.useGreetings)
                {
                    await SayHello(userName, chatMessage);
                }
            }
        }

        Task CreateUserData(string userName, string[] message)
        {
            var settings = LocalStorage.GetSettings();

            var firstMessage = m_parser.IsFirstMessage(message);
            var nameColor = settings.displayNicknameColor ? m_parser.GetUserColor(message) : Color.white;
            var rewardId = m_parser.GetRewardId(message);
            var msgId = m_parser.GetMsgId(message);

            if (m_chatters.TryGetValue(userName, out var userData))
            {
                userData.CustomRewardId = rewardId;
                userData.IsFirstMessage = firstMessage;
                userData.MsgId = msgId;
            }
            else
            {
                userData = new ChatUserData
                {
                    UserName = userName,
                    IsFirstMessage = firstMessage,
                    IsReturningChatter = m_parser.IsReturnedChatter(message),
                    Color = nameColor,
                    MsgId = msgId
                };
                m_chatters.Add(userName, userData);
            }

            m_parser.SetBadges(userData, message);

            return Task.CompletedTask;
        }

        async Task ShowImage(string chatMessage)
        {
            var imageName = await m_parser.GetTaggingWordWithoutTag(LocalStorage.GetSettings().imageNameTag, chatMessage);
            if (imageName != null)
            {
                OnImageShown?.Invoke(imageName.ToLower());
            }
        }

        async Task SayHello(string userName, string message)
        {
            if (await DidUserSayHello(userName)) return;
            //  TODO любое сообщение не поздоровавшегося чатерса будет выполнять этот говнокод.
            if (m_parser.IsGreetingWordFound(message))
            {
                m_chatters.TryGetValue(userName, out var userData);
                if (userData != null) userData.DidSayHello = true;
                OnSayHello?.Invoke(m_chatters[userName]);
            }
        }

        async Task StartAvatar(string userName, string message)
        {
            m_chatters.TryGetValue(userName, out var userData);
            if (userData == null) return;
            var avatarName = await m_parser.GetTaggingWordWithoutTag(LocalStorage.GetSettings().avatarNameTag, message);
            if (avatarName != null)
            {
                OnAvatarStarted?.Invoke(userData, avatarName.ToLower());
            }
        }

        Task AttackUser(string userName, string message)
        {
            var targetName = m_parser.GetAttackedName(message);
            if (targetName != null)
            {
                OnAvatarPursuit?.Invoke(userName, targetName);
            }

            return Task.CompletedTask;
        }

        Task<bool> DidUserSayHello(string userName)
        {
            m_chatters.TryGetValue(userName, out var userData);
            var result = Task.FromResult(userData != null && userData.DidSayHello);
            return result;
        }

        void HighlightedMessage(string userName, string message)
        {
            if (m_chatters.TryGetValue(userName, out var userData))
            {
                if (userData.MsgId == HighlightedMsg)
                {
                    OnHighlightedMessage?.Invoke(userData, message);
                }
            }
        }
    }
}