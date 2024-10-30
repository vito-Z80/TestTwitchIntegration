using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Twitch
{
    public class TwitchChatController
    {
        const string NamePattern = "([a-zA-Z0-9_-]+)";
        const string UserChatNamePattern = @"(@[a-zA-Z0-9_-]+)\.tmi";
        const string AttackPattern = @"_attack\s+(@[a-zA-Z0-9_-]+)";

        const string PrivateMessage = "PRIVMSG";
        const string DisplayName = "display-name";
        const string FirstMessage = "first-msg";
        const string Moderator = "mod";
        const string ReturningChatter = "returning-chatter";
        const string Subscriber = "subscriber";
        const string UserType = "user-type";
        const string UserColor = "color";

        const string Zero = "0";

        const string HelloPattern =
            @"(?i)\b(hello|hi|hey|good\s(morning|afternoon|evening)|ку|драсте|драсти|дратути|дратуте|дароу|привет(ы)|халоу|драсть|драсьте|здарова|сдарова|привет|здравствуй(те)?|добрый\s(день|утро|вечер))\b";

        // readonly List<string> m_pullNames = new();
        readonly Dictionary<string, ChatUserData> m_chatters = new();


        public static Action<ChatUserData, string> OnAvatarStarted;
        public static Action<string, string> OnAvatarPursuit;
        public static Action<string> OnSayHello;
        public static event Action<string> OnImageShown;


        AppSettingsData m_settings;
        
        public async Task ProcessMessage(string message)
        {
            // var userName = await PullByPattern(message, UserChatNamePattern);


            m_settings ??= LocalStorage.GetSettings();
            
            if (!message.Contains(PrivateMessage)) return;

            var messageParts = message.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var userName = GetValueByChatTag(DisplayName, messageParts);


            await CreateUserData(userName, messageParts);

            if (userName != null)
            {
                if (m_settings.useAvatars)
                {
                    await AttackUser(userName, message);
                    await StartAvatar(userName, message);   
                }

                if (m_settings.useImages)
                {
                    await ShowImage(message);    
                }

                if (m_settings.canBeWelcomed)
                {
                    await SayHello(userName, message);   
                }
            }
        }

        Task CreateUserData(string userName, string[] message)
        {
            var settings = LocalStorage.GetSettings();

            var moderator = GetValueByChatTag(Moderator, message) != Zero;
            var firstMessage = GetValueByChatTag(FirstMessage, message) != Zero;
            var subscriber = GetValueByChatTag(Subscriber, message) != Zero;
            var returningChatter = GetValueByChatTag(ReturningChatter, message) != Zero;

            if (!ColorUtility.TryParseHtmlString(GetValueByChatTag(UserColor, message), out var color) || !settings.displayNicknameColor)
            {
                color = Color.white;
            }

            if (m_chatters.TryGetValue(userName, out var userData))
            {
                userData.Color = color;
                userData.IsModerator = moderator;
                userData.IsSubscriber = subscriber;
                userData.IsReturningChatter = returningChatter;
                userData.IsFirstMessage = firstMessage;
            }
            else
            {
                var newUserData = new ChatUserData
                {
                    UserName = userName,
                    IsModerator = moderator,
                    IsFirstMessage = firstMessage,
                    IsSubscriber = subscriber,
                    IsReturningChatter = returningChatter,
                    Color = color
                };
                m_chatters.Add(userName, newUserData);
            }

            return Task.CompletedTask;
        }

        async Task ShowImage(string message)
        {
            var tag = $@"{Regex.Escape(LocalStorage.GetSettings().imageNameTag)}{NamePattern}";
            var startOfChatMessage = message.Split(":").Last();
            var imageName = await PullByPattern(startOfChatMessage, tag);
            if (imageName != null)
            {
                OnImageShown?.Invoke(imageName);
            }
        }

        async Task SayHello(string userName, string message)
        {
            if (await DidUserSayHello(userName)) return;
            if (await PullByPattern(message, HelloPattern) is not null)
            {
                m_chatters.TryGetValue(userName, out var userData);
                if (userData != null) userData.DidSayHello = true;
                OnSayHello?.Invoke(userName);
            }
        }

        async Task StartAvatar(string userName, string message)
        {
            m_chatters.TryGetValue(userName, out var userData);
            if (userData == null) return;
            if (LocalStorage.GetSettings().avatarSubscribersOnly && !userData.IsSubscriber) return;
            var tag = $@"{Regex.Escape(LocalStorage.GetSettings().avatarNameTag)}{NamePattern}";
            var startOfChatMessage = message.Split(":").Last();
            var avatarName = await PullByPattern(startOfChatMessage, tag);
            if (avatarName != null)
            {
                OnAvatarStarted?.Invoke(userData, avatarName);
            }
        }

        Task AttackUser(string userName, string message)
        {
            var match = Regex.Match(message, AttackPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var targetName = match.Groups[1].Value;
                OnAvatarPursuit?.Invoke(userName, targetName);
            }

            return Task.CompletedTask;
        }


        string GetValueByChatTag(string tag, string[] splittingMessage)
        {
            var partOfMessage = splittingMessage.FirstOrDefault(s => s.StartsWith(tag));
            if (partOfMessage != null)
            {
                var splitMessagePart = partOfMessage.Split('=');
                if (splitMessagePart.Length == 2)
                {
                    return splitMessagePart[1];
                }
            }

            return Zero;
        }


        [ItemCanBeNull]
        Task<string> PullByPattern(string message, string pattern)
        {
            var match = Regex.Match(message, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Task.FromResult(match.Success ? match.Groups[1].Value : null);
        }

        Task<bool> DidUserSayHello(string userName)
        {
            m_chatters.TryGetValue(userName, out var userData);
            var result = Task.FromResult(userData != null && userData.DidSayHello);
            return result;
        }
    }
}