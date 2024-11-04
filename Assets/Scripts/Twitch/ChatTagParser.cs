using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data;
using JetBrains.Annotations;
using UnityEngine;

//  https://dev.twitch.tv/docs/chat/irc/
//  https://dev.twitch.tv/docs/chat/irc/#notice-tags

namespace Twitch
{
    public class ChatTagParser
    {
        const string NamePattern = "([a-zA-Z0-9_-]+)";
        const string AttackPattern = @"_attack\s+(@[a-zA-Z0-9_-]+)";
        const string Zero = "0";
        const char Separator = '/';


        //  Chat TAGS
        const string PrivateMessage = "PRIVMSG";

        const string DisplayName = "display-name";
        const string FirstMessage = "first-msg"; //  
        const string ReturningChatter = "returning-chatter";
        const string UserColor = "color";
        const string CustomRewardId = "custom-reward-id";
        const string MsgId = "msg-id"; //  msg-id=highlighted-message

        const string Badges = "badges"; //  Значки пользователя: badges=badge/number, badge/number, badge/number...

        //  Ниже значки от Badges: имеют вид: badge/number
        const string Moderator = "moderator";
        const string Subscriber = "subscriber"; //  subscriber/[months] — Подписчик канала, где [months] указывает на продолжительность подписки (например, subscriber/6 для шестимесячной подписки).
        const string Vip = "vip"; //  VIP-зритель, имеющий привилегии на канале.
        const string Partner = "partner"; //  Партнер Twitch.
        const string Premium = "premium"; //  Пользователь Twitch Prime.
        const string Stuff = "stuff"; //  Сотрудник Twitch.
        const string Admin = "admin"; //  Администратор Twitch.
        const string Bits = "bits"; //  bits/[amount] - Значок для пользователей, потративших определенное количество битсов на канале, где [amount] указывает количество.
        const string Founder = "founder"; //  Один из первых подписчиков канала (обычно в пределах первых 10).
        const string HypeTrain = "hype-train"; //   Уровень Hype Train для зрителей, которые участвовали в Hype Train на канале.


        [ItemCanBeNull]
        public Task<string> GetTaggingWordWithoutTag(string tag, string chatMessage)
        {
            var match = Regex.Match(chatMessage, $@"(?:^|\s){Regex.Escape(tag)}([A-Za-z0-9]+)\b");
            return Task.FromResult(match.Success ? match.Groups[1].Value : null);
        }

        public string GetImageCommand()
        {
            return $@"{Regex.Escape(LocalStorage.GetSettings().imageNameTag)}{NamePattern}";
        }

        public bool IsGreetingWordFound(string message)
        {
            var words = LocalStorage.GetGreetingsVariants();
            var pattern = @"\b(" + string.Join("|", words.Select(Regex.Escape)) + @")\b";
            return Regex.IsMatch(message, pattern, RegexOptions.IgnoreCase);
        }

        [CanBeNull]
        public string GetAttackedName(string message)
        {
            var match = Regex.Match(message, AttackPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var targetName = match.Groups[1].Value.Replace("@", "");
                return targetName;
            }

            return null;
        }

        /// <summary>
        /// Разделить строку на теги и сообщение.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string[] SplitMessageTags(string message) => message.Split(PrivateMessage);

        public bool IsFirstMessage(string[] userMessageTags) => GetValueByChatTag(FirstMessage, userMessageTags) != Zero;

        public bool IsReturnedChatter(string[] userMessageTags) => GetValueByChatTag(ReturningChatter, userMessageTags) != Zero;
        public string GetRewardId(string[] userMessageTags) => GetValueByChatTag(CustomRewardId, userMessageTags);
        public string GetMsgId(string[] userMessageTags) => GetValueByChatTag(MsgId, userMessageTags);
        public string GetUserName(string[] userMessageTags) => GetValueByChatTag(DisplayName, userMessageTags);

        public Color GetUserColor(string[] userMessageTags)
        {
            if (ColorUtility.TryParseHtmlString(GetValueByChatTag(UserColor, userMessageTags), out var color))
            {
                return color;
            }

            return Color.white;
        }

        public void SetBadges(ChatUserData data, string[] userMessageTags)
        {
            var badges = GetValueByChatTag(Badges, userMessageTags).Split(","); //  subscriber/1,moderator/1...
            int.TryParse(GetValueByChatTag(Subscriber, badges, Separator), out var subscriberLevel);
            int.TryParse(GetValueByChatTag(Bits, badges, Separator), out var bits);
            int.TryParse(GetValueByChatTag(Founder, badges, Separator), out var founderNumber);
            int.TryParse(GetValueByChatTag(HypeTrain, badges, Separator), out var hypeTrainLevel);
            var vipStatus = GetValueByChatTag(Vip, badges, Separator) != Zero;
            var isModerator = GetValueByChatTag(Moderator, badges, Separator) != Zero;
            var isPartner = GetValueByChatTag(Partner, badges, Separator) != Zero;
            var isPremium = GetValueByChatTag(Premium, badges, Separator) != Zero;
            var isStuff = GetValueByChatTag(Stuff, badges, Separator) != Zero;
            var isAdmin = GetValueByChatTag(Admin, badges, Separator) != Zero;
            var isSubscriber = subscriberLevel != 0;

            data.SubscriberLevel = subscriberLevel;
            data.Bits = bits;
            data.FounderNumber = founderNumber;
            data.HypeTrainLevel = hypeTrainLevel;
            data.IsVip = vipStatus;
            data.IsModerator = isModerator;
            data.IsPartner = isPartner;
            data.IsPremium = isPremium;
            data.IsStuff = isStuff;
            data.IsAdmin = isAdmin;
            data.IsSubscriber = isSubscriber;
        }

        string GetValueByChatTag(string tag, string[] splittingMessage, char separator = '=')
        {
            var partOfMessage = splittingMessage.FirstOrDefault(s => s.StartsWith(tag));
            if (partOfMessage != null)
            {
                var splitMessagePart = partOfMessage.Split(separator);
                if (splitMessagePart.Length == 2)
                {
                    return splitMessagePart[1];
                }
            }

            return Zero;
        }
    }
}