using System.Threading.Tasks;
using Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Twitch
{
    public class Connect
    {
        const string FieldKey = "FiledKey";

        readonly TwitchOAuth m_oauth;
        readonly TwitchChatClient m_twitchChatClient;
        FieldsData m_fieldsData;
        TokenData m_tokenData;

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
            m_fieldsData = LoadFieldsData();
            m_oauth.TokenData = LoadTokenData();

            if (m_fieldsData == null && m_oauth.TokenData == null) return;

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
                    await m_oauth.RefreshAccessToken(m_fieldsData.ClientId, m_secret);   
                }
                else
                {
                    Log.LogMessage("Try refresh Token but secret is absent");
                    m_panel.gameObject.SetActive(true);
                    m_panel.ConnectPanelRefreshVisible();
                    return;
                }
            }

            var userName = CorrectString(m_fieldsData.UserName);
            var channelName = CorrectString(m_fieldsData.ChannelName);
            await m_twitchChatClient.Connect(m_oauth, userName, channelName);
        }


        public async void ConnectTwitch()
        {
            SaveFieldsData();
            var userName = CorrectString(m_fieldsData.UserName);
            var channelName = CorrectString(m_fieldsData.ChannelName);
            var redirect = CorrectString(m_fieldsData.Redirect);
            var clientId = CorrectString(m_fieldsData.ClientId);
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
            PlayerPrefs.DeleteAll();
            ConnectTwitch();
        }


        [CanBeNull]
        TokenData LoadTokenData()
        {
            if (PlayerPrefs.HasKey(TwitchOAuth.TokenDataKey))
            {
                var json = PlayerPrefs.GetString(TwitchOAuth.TokenDataKey);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<TokenData>(json);
                }
            }

            return null;
        }


        void SaveFieldsData()
        {
            m_fieldsData ??= new FieldsData();
            m_fieldsData.ChannelName = CorrectString(m_panel.channelName.text);
            m_fieldsData.ClientId = CorrectString(m_panel.clientId.text);
            m_fieldsData.Redirect = CorrectString(m_panel.redirect.text);
            m_fieldsData.UserName = CorrectString(m_panel.userName.text);

            var json = JsonConvert.SerializeObject(m_fieldsData);
            PlayerPrefs.SetString(FieldKey, json);
            PlayerPrefs.Save();
        }

        [CanBeNull]
        FieldsData LoadFieldsData()
        {
            if (PlayerPrefs.HasKey(FieldKey))
            {
                var json = PlayerPrefs.GetString(FieldKey);
                m_fieldsData = JsonConvert.DeserializeObject<FieldsData>(json);
                m_panel.channelName.text = m_fieldsData.ChannelName;
                m_panel.userName.text = m_fieldsData.UserName;
                m_panel.redirect.text = m_fieldsData.Redirect;
                m_panel.clientId.text = m_fieldsData.ClientId;
                return m_fieldsData;
            }

            return null;
        }


        string CorrectString(string input) => input.Replace("\u200B", "").Trim();
    }
}