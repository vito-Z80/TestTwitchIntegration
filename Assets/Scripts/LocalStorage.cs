using System.Linq;
using System.Text.RegularExpressions;
using Data;
using Newtonsoft.Json;
using UnityEngine;

public static class LocalStorage
{
    static AppSettingsData m_settings;
    static string[] m_greetingsVariants;

    const string SettingsKey = "AppSettings";
    const string GreetingsVariantsKey = "GreetingsVariants";
    const string GreetingsVariantsPattern = @"[.,;\s]+";

    public static AppSettingsData GetSettings()
    {
        // PlayerPrefs.DeleteKey(SettingsKey);
        if (m_settings != null) return m_settings;
        if (PlayerPrefs.HasKey(SettingsKey))
        {
            var json = PlayerPrefs.GetString(SettingsKey);
            Log.LogMessage(json);
            m_settings = JsonConvert.DeserializeObject<AppSettingsData>(json);
        }
        else
        {
            m_settings = new AppSettingsData();
        }

        return m_settings;
    }

    public static void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(m_settings);
        PlayerPrefs.SetString(SettingsKey, json);
        PlayerPrefs.Save();
    }


    public static string[] GetGreetingsVariants()
    {
        if (m_greetingsVariants != null) return m_greetingsVariants;
        if (PlayerPrefs.HasKey(GreetingsVariantsKey))
        {
            var data = PlayerPrefs.GetString(GreetingsVariantsKey);
            m_greetingsVariants = Regex.Split(data, GreetingsVariantsPattern, RegexOptions.IgnoreCase)
                .Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();
        }
        else
        {
            m_greetingsVariants = new[] { "hi", "hello", "привет", "hola" };
        }

        return m_greetingsVariants;
    }

    public static void SaveGreetingsVariants(string data)
    {
        m_greetingsVariants = Regex.Split(data, GreetingsVariantsPattern, RegexOptions.IgnoreCase)
            .Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();
        PlayerPrefs.SetString(GreetingsVariantsKey, data);
        PlayerPrefs.Save();
    }
}