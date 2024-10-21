using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Twitch
{
    public class TwitchChatController
    {
        const string AvatarNamePattern = @"_i_([a-zA-Z0-9_-]+)";
        const string UserChatNamePattern = @"(@[a-zA-Z0-9_-]+)\.tmi";
        const string AttackPattern = @"_attack\s+(@[a-zA-Z0-9_-]+)";

        const string HelloPattern =
            @"(?i)\b(hello|hi|hey|good\s(morning|afternoon|evening)|ку|драсте|драсти|дратути|дратуте|дароу|привет(ы)|халоу|драсть|драсьте|здарова|сдарова|привет|здравствуй(те)?|добрый\s(день|утро|вечер))\b";

        readonly List<string> m_pullNames = new();


        public static Action<string, string> OnAvatarStarted;
        public static Action<string, string> OnAvatarPursuit;
        public static Action<string> OnMessageSend;
        public static Action<string> OnSayHello;


        public async Task ProcessMessage(string message)
        {
            var userName = await PullByPattern(message, UserChatNamePattern);
            if (userName != null)
            {
                SendMessage(message);
                await AttackUser(userName, message);
                await StartAvatar(userName, message);
                await SayHello(userName, message);
            }
        }

        void SendMessage(string message)
        {
            OnMessageSend?.Invoke(message);
        }

        async Task SayHello(string userName, string message)
        {
            if (await UserWhoSaidHello(userName)) return;
            // Debug.Log($"<color=yellow>{userName}</color>: {string.Join(",", m_pullNames)}");
            if (await PullByPattern(message, HelloPattern) is not null)
            {
                m_pullNames.Add(userName);
                OnSayHello?.Invoke(userName);
            }
        }

        async Task StartAvatar(string userName, string message)
        {
            var avatarName = await PullByPattern(message, AvatarNamePattern);
            if (avatarName != null)
            {
                OnAvatarStarted?.Invoke(userName, avatarName);
            }
        }

        Task AttackUser(string userName, string message)
        {
            var match = Regex.Match(message, AttackPattern);
            if (match.Success)
            {
                var targetName = match.Groups[1].Value;
                OnAvatarPursuit?.Invoke(userName, targetName);
            }
            return Task.CompletedTask;
        }


        [ItemCanBeNull]
        Task<string> PullByPattern(string message, string pattern)
        {
            var match = Regex.Match(message, pattern, RegexOptions.IgnoreCase);
            return Task.FromResult(match.Success ? match.Groups[1].Value.ToLower() : null);
        }

        Task<bool> UserWhoSaidHello(string userName)
        {
            var result = Task.FromResult(m_pullNames.Contains(userName));
            return result;
        }
    }
}