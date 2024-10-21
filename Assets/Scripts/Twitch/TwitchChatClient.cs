using System;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace Twitch
{
    public class TwitchChatClient
    {
        string m_userName; // Имя пользователя.
        string m_channelName; // Имя канала для подключения.
        WebSocket m_websocket;
        TwitchChatController m_twitchChatController;
        TwitchOAuth m_twitchOAuth;
        
        const string PrivateMessage = "PRIVMSG";
        
        public static Action<bool> OnConnectPanelVisible;

        public async Task Connect(TwitchOAuth oAuth, string userName, string channelName)
        {
            m_twitchChatController = new TwitchChatController();
            m_twitchOAuth = oAuth;
            m_userName = userName;
            m_channelName = channelName;
            await ConnectToChat();
        }

        async Task ConnectToChat()
        {
            // Создаем WebSocket-соединение с Twitch IRC сервером.
            m_websocket = new WebSocket("wss://irc-ws.chat.twitch.tv:443");
            m_websocket.OnOpen += OpenChat;
            m_websocket.OnMessage += GetChatMessage;
            m_websocket.OnError += ChatError;
            m_websocket.OnClose += ChatClosed;
            await m_websocket.Connect();
        }

        void ChatClosed(WebSocketCloseCode closeCode)
        {
            
            Log.LogMessage("Соединение закрыто: " + closeCode);
        }

        void ChatError(string errorMsg)
        {
            Log.LogMessage("Ошибка WebSocket: " + errorMsg);
        }

        void OpenChat()
        {
            Authenticate();
            Log.LogMessage("Подключился к чату.");
            OnConnectPanelVisible?.Invoke(false);
        }

        async void GetChatMessage(byte[] bytes)
        {
            var message = Encoding.UTF8.GetString(bytes);
            if (message.Contains(PrivateMessage))
            {
                Log.LogMessage($"Сообщение из чата: {message}");
                await m_twitchChatController.ProcessMessage(message.ToLower());
            }
        }


        private void Authenticate()
        {
            // Отправляем команды для аутентификации.
            SendCommand($"PASS oauth:{m_twitchOAuth.TokenData.access_token}");
            SendCommand($"NICK {m_userName}");
            SendCommand($"JOIN #{m_channelName}");
        }

        private void SendCommand(string command)
        {
            // Log.LogMessage(command);
            if (m_websocket.State == WebSocketState.Open)
            {
                m_websocket.SendText(command);
            }
        }

        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            m_websocket?.DispatchMessageQueue();
#endif
        }

        public async void OnApplicationQuit()
        {
            if (m_websocket != null)
            {
                await m_websocket.Close();
                m_websocket.OnClose -= ChatClosed;
                m_websocket.OnError -= ChatError;
                m_websocket.OnMessage -= GetChatMessage;
                m_websocket.OnOpen -= OpenChat;
            }
        }
    }
}