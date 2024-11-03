using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avatars;
using Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public static class LocalStorage
{
    static AppSettingsData m_settings;
    static string[] m_greetingsVariants;


    public const string ImagesFolder = "Graphics/Images";
    public const string AvatarsFolder = "Graphics/Avatars";

    const string SettingsKey = "AppSettings";
    const string GreetingsVariantsKey = "GreetingsVariants";
    const string LastWriteTimeAvatarFolderKey = "lwtafk";

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

    //  atlas

    public static void SaveAtlasData(Texture2D texture, Rect[] rects)
    {
        var atlas = texture.EncodeToPNG();
        var atlasPath = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}/atlas.png");
        File.WriteAllBytes(atlasPath, atlas);
        Log.LogMessage($"Saved atlas: {atlasPath}");

        var rectsPath = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}/atlasRects.json");
        var uvRects = Utils.RectToUVRect(rects);
        var json = JsonConvert.SerializeObject(uvRects);
        File.WriteAllText(rectsPath, json);
        Log.LogMessage($"Saved {uvRects.Length} UV rects for atlas: {rectsPath}");
        //  всегда нужно менять время последнего изменения в папке аватаров, потому что после проверки этого времени были
        //      сохранены атлас и uv, тем самым время изменилось...  я хрен пойму потом что я тут написал...
        SaveLastWriteTimeAvatarFolder();
        Log.LogMessage($"Было обновлено последнее время изменения в папке Аватаров, так как был пересоздан атлас и uv.");
    }


    public static void LoadAtlasData(out Texture2D atlas, out Rect[] uvRects)
    {
        var atlasPath = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}/atlas.png");
        var rectsPath = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}/atlasRects.json");
        if (!File.Exists(atlasPath) || !File.Exists(rectsPath))
        {
            Log.LogMessage($"Atlas or UV Rects not found: {atlasPath}");
            atlas = null;
            uvRects = null;
            return;
        }

        var bytes = File.ReadAllBytes(atlasPath);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        texture.LoadImage(bytes);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.mipMapBias = 0;
        texture.Apply();
        Log.LogMessage($"Loaded atlas texture: {texture.width}x{texture.height}");
        atlas = texture;

        uvRects = JsonConvert.DeserializeObject<Rect[]>(File.ReadAllText(rectsPath));
        Log.LogMessage($"Loaded {uvRects.Length} UV Rects for atlas: {rectsPath}");
    }

    //  jsons

    public static void SaveAvatarsData()
    {
        var json = JsonConvert.SerializeObject(AvatarsStorage.GetAvatars());
        var path = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}/avatars.json");
        File.WriteAllText(path, json);
        Log.LogMessage($"Saved avatars data: {path}");
    }

    [CanBeNull]
    public static Dictionary<string, AvatarData> LoadAvatarsData()
    {
        var path = $"{Application.streamingAssetsPath}/{AvatarsFolder}/avatars.json";
        var json = File.ReadAllText(path);
        var avatarsData = JsonConvert.DeserializeObject<Dictionary<string, AvatarData>>(json);
        Log.LogMessage($"Loaded avatars data: {path}");
        return avatarsData;
    }

    //  utils

    static void SaveLastWriteTimeAvatarFolder()
    {
        var path = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}");
        var ticks = Directory.GetLastWriteTime(path).Ticks.ToString();
        PlayerPrefs.SetString(LastWriteTimeAvatarFolderKey, ticks);
    }

    /// <summary>
    /// Были ли изменения в папке Avatar
    /// </summary>
    /// <param name="path">Путь папки.</param>
    /// <returns>true - Если папка не была изменена (ничего в ней не добавилось, не удалилось.)</returns>
    public static bool IsAvatarFolderNotChanged()
    {
        var path = Utils.NormalizePath($"{Application.streamingAssetsPath}/{AvatarsFolder}");
        if (PlayerPrefs.HasKey(LastWriteTimeAvatarFolderKey))
        {
            var currentTicks = Directory.GetLastWriteTime(path).Ticks.ToString();
            var lastTick = PlayerPrefs.GetString(LastWriteTimeAvatarFolderKey);
            var result = currentTicks == lastTick;
            if (result)
            {
                Log.LogMessage($"The contents of the folder have not been changed: {path}");
            }
            else
            {
                Log.LogMessage($"The contents of the folder have been changed: {path}");
            }
            return result;
        }

        Log.LogMessage($"The folder time was checked for the first time.: {path}");
        SaveLastWriteTimeAvatarFolder();
        return false;
    }
}