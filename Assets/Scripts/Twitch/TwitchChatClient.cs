using System;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;

namespace Twitch
{
    public class TwitchChatClient
    {
        string m_userName; // Имя пользователя.
        string m_channelName; // Имя канала для подключения.
        WebSocket m_websocket;
        TwitchChatController m_twitchChatController;
        TwitchOAuth m_twitchOAuth;

        // const string PrivateMessage = "PRIVMSG";

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

        //  вебсокет считает соединение не активным через некоторое время, нужно отправлять ему сообщение, что бы избежать отключения. 
        async Task SendPing()
        {
            while (m_websocket != null && m_websocket.State == WebSocketState.Open)
            {
                SendCommand("PING :tmi.twitch.tv");
                Log.LogMessage("Отправлен PING");
                await Task.Delay(550000);
            }
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
            _ = SendPing();
        }

        async void GetChatMessage(byte[] bytes)
        {
            try
            {
                var message = Encoding.UTF8.GetString(bytes);
                Log.LogMessage($"Сообщение из чата: {message}");
                await m_twitchChatController.ProcessMessage(message);
            }
            catch (Exception e)
            {
                Log.LogMessage($"Не удалось получить последнее сообщение из чата. {e.Message}");
            }
        }

        private void Authenticate()
        {
            // Включаем режим тегов для получения метаданных
            SendCommand("CAP REQ :twitch.tv/tags");
            // Отправляем команды для аутентификации.
            SendCommand($"PASS oauth:{m_twitchOAuth.AuthorizationTokenData.access_token}");
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