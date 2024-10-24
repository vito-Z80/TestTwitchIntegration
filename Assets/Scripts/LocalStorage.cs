using Data;
using Newtonsoft.Json;
using UnityEngine;

public static class LocalStorage
{
    static AppSettingsData m_settings;
    const string SettingsKey = "AppSettings";

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
}