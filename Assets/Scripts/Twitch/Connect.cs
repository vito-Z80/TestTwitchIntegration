using System.Threading.Tasks;
using Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
    public class Connect
    {
        const string FieldKey = "FiledKey";

        readonly TwitchOAuth m_oauth;
        readonly TwitchChatClient m_twitchChatClient;
        AuthorizationData m_authorizationData;
        AuthorizationTokenData m_authorizationTokenData;

        readonly ConnectPanel m_panel;

        string m_secret;

        public Connect(ConnectPanel connectPanel)
        {
            m_panel = connectPanel;
            m_oauth = new TwitchOAuth();
            m_twitchChatClient = new TwitchChatClient();
        }


        public async Task AutoConnectTwitch()
        {
            Log.LogMessage("AUTO Connecting Twitch Chat...");
            m_authorizationData = LoadAuthorizationData();
            m_oauth.AuthorizationTokenData = LoadTokenData();

            if (m_authorizationData == null || m_oauth.AuthorizationTokenData == null)
            {
                m_panel.gameObject.SetActive(true);
                Log.LogMessage("Automatic connection not possible, no data available.");
                return;
            }
            Log.LogMessage("Check valid Twitch OAuth Token...");
            if (await m_oauth.IsTokenValid())
            {
                Log.LogMessage("Twitch OAuth Token is valid");
            }
            else
            {
                if (m_secret != null)
                {
                    Log.LogMessage("Try refresh Token");
                    await m_oauth.RefreshAccessToken(m_authorizationData.ClientId, m_secret);   
                }
                else
                {
                    Log.LogMessage("Secret is absent");
                    m_panel.gameObject.SetActive(true);
                    m_panel.ConnectPanelRefreshVisible();
                    return;
                }
            }

            var userName = CorrectString(m_authorizationData.UserName);
            var channelName = CorrectString(m_authorizationData.ChannelName);
            await m_twitchChatClient.Connect(m_oauth, userName, channelName);
        }


        public async void ConnectTwitch()
        {
            SaveAuthorizationData();
            var userName = CorrectString(m_authorizationData.UserName);
            var channelName = CorrectString(m_authorizationData.ChannelName);
            var redirect = CorrectString(m_authorizationData.Redirect);
            var clientId = CorrectString(m_authorizationData.ClientId);
            m_secret = CorrectString(m_panel.clientSecret.text);
            await m_oauth.Auth(clientId, m_secret, redirect);
            await m_twitchChatClient.Connect(m_oauth, userName, channelName);
        }


        internal void Update()
        {
            m_twitchChatClient?.Update();
        }

        public void OnApplicationQuit()
        {
            m_twitchChatClient?.OnApplicationQuit();
        }


        // public async void Connect()
        // {
        //     // connectButton.gameObject.SetActive(false);
        //     OnConnectButtonEnabled?.Invoke(false);
        //     SaveFields();
        //     await m_oauth.Auth(CorrectString(clientId.text), CorrectString(clientSecret.text), CorrectString(redirect.text));
        //     SaveData(m_oauth.TokenData);
        //     await m_twitchChatClient.Connect(m_oauth, CorrectString(userName.text), CorrectString(channelName.text));
        //     // connectPanel.SetActive(false);
        //     OnConnectPanelVisible?.Invoke(false);
        // }

        public void LogOut()
        {
            PlayerPrefs.DeleteKey(FieldKey);
            // ConnectTwitch();
        }


        [CanBeNull]
        AuthorizationTokenData LoadTokenData()
        {
            if (PlayerPrefs.HasKey(TwitchOAuth.TokenDataKey))
            {
                var json = PlayerPrefs.GetString(TwitchOAuth.TokenDataKey);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<AuthorizationTokenData>(json);
                }
            }

            return null;
        }


        void SaveAuthorizationData()
        {
            m_authorizationData ??= new AuthorizationData();
            m_authorizationData.ChannelName = CorrectString(m_panel.channelName.text);
            m_authorizationData.ClientId = CorrectString(m_panel.clientId.text);
            m_authorizationData.Redirect = CorrectString(m_panel.redirect.text);
            m_authorizationData.UserName = CorrectString(m_panel.userName.text);

            var json = JsonConvert.SerializeObject(m_authorizationData);
            PlayerPrefs.SetString(FieldKey, json);
            // PlayerPrefs.Save();
        }

        [CanBeNull]
        AuthorizationData LoadAuthorizationData()
        {
            if (PlayerPrefs.HasKey(FieldKey))
            {
                var json = PlayerPrefs.GetString(FieldKey);
                m_authorizationData = JsonConvert.DeserializeObject<AuthorizationData>(json);
                m_panel.channelName.text = m_authorizationData.ChannelName;
                m_panel.userName.text = m_authorizationData.UserName;
                m_panel.redirect.text = m_authorizationData.Redirect;
                m_panel.clientId.text = m_authorizationData.ClientId;
                return m_authorizationData;
            }

            return null;
        }


        string CorrectString(string input) => input.Replace("\u200B", "").Trim();
    }
}