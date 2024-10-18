using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
    public class TwitchOAuth
    {
        public const string TokenDataKey = "TokenDataKey";
        [HideInInspector] public TokenData TokenData;
        
        public async Task Auth(string clientId, string clientSecret, string redirectUri)
        {
            var code = await OpenAuthorizationUrl(clientId, redirectUri);
            await GetOAuthToken(code, clientId, clientSecret, redirectUri);
        }


        async Task<string> OpenAuthorizationUrl(string clientId, string redirectUri)
        {
            // Открываем страницу авторизации
            var url = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=chat:read+chat:edit";
            var encodedUrl = Uri.EscapeUriString(url);
            // Log.LogMessage($"Encoded URL: {encodedUrl}");
            Application.OpenURL(encodedUrl);
            // Log.LogMessage("Открыл страницу авторизации в браузере.");
            // Запускаем локальный HTTP-сервер для получения кода
            var code = await StartHttpServer(redirectUri);
            // Log.LogMessage($"Получен код: {code}");
            return code;
        }

        async Task GetOAuthToken(string code, string clientId, string clientSecret, string redirectUri)
        {
            var url = "https://id.twitch.tv/oauth2/token";
            WWWForm form = new WWWForm();
            form.AddField("client_id", clientId);
            form.AddField("client_secret", clientSecret);
            form.AddField("code", code);
            form.AddField("grant_type", "authorization_code");
            form.AddField("redirect_uri", redirectUri);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Log.LogMessage("Успешно получили токен: " /*+ www.downloadHandler.text*/);
                    ProcessTokenResponse(www.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Ошибка получения токена: " + www.error);
                    Log.LogMessage("Ошибка получения токена: " + www.error);
                }
            }
        }


        async Task<string> StartHttpServer(string redirectUri)
        {
            string code = null;
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            Log.LogMessage("Ожидание ответа от браузера...");

            var context = await listener.GetContextAsync();
            var request = context.Request;

            // Извлекаем код из URL
            var query = request.Url.Query;
            var queryParams = HttpUtility.ParseQueryString(query);
            code = queryParams["code"];

            // Отправляем ответ пользователю
            var response = context.Response;
            var responseString = "<html><head><meta charset=\"utf-8\"></head><body><h1>Авторизация завершена!</h1><p>Вы можете закрыть это окно.</p></body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
            {
                await output.WriteAsync(buffer, 0, buffer.Length);
            }

            listener.Stop();
            return code;
        }

        // Обработка ответа с токеном и сохранение его в PlayerPrefs
        void ProcessTokenResponse(string jsonResponse)
        {
            TokenData = JsonConvert.DeserializeObject<TokenData>(jsonResponse);

            PlayerPrefs.SetString(TokenDataKey, jsonResponse);
            PlayerPrefs.Save();

            Log.LogMessage("Токены сохранены.");
        }

        public async Task<bool> IsTokenValid()
        {
            var url = "https://id.twitch.tv/oauth2/validate";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", "OAuth " + TokenData.access_token);
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Log.LogMessage("Токен валиден.");
                    foreach (var header in www.GetResponseHeaders())
                    {
                        // Log.LogMessage($"{header.Key}: {header.Value}");
                    }

                    return true;
                }
                else if (www.responseCode == 401)
                {
                    Log.LogMessage("Токен истек или недействителен.");
                    foreach (var header in www.GetResponseHeaders())
                    {
                        Log.LogMessage($"{header.Key}: {header.Value}");
                    }

                    return false;
                }
                else
                {
                    Debug.LogError("Ошибка проверки токена: " + www.error);
                    Log.LogMessage("Ошибка проверки токена: " + www.error);
                    foreach (var header in www.GetResponseHeaders())
                    {
                        Log.LogMessage($"{header.Key}: {header.Value}");
                    }

                    return false;
                }
            }
        }

        // Метод для обновления токена по refresh_token
        public async Task RefreshAccessToken(string clientId, string secret)
        {
            var url = "https://id.twitch.tv/oauth2/token";
            var form = new WWWForm();
            form.AddField("client_id", clientId);
            form.AddField("client_secret", secret);
            form.AddField("grant_type", "refresh_token");
            form.AddField("refresh_token", TokenData.refresh_token);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Log.LogMessage("Токен обновлен: " /*+ www.downloadHandler.text*/);
                    ProcessTokenResponse(www.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Ошибка обновления токена: " + www.error);
                    Log.LogMessage("Ошибка обновления токена: " + www.error);
                }
            }
        }
    }
}