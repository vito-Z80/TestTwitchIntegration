using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.U2D;

public static class Utils
{
    public static Vector2 GetScreenSize(PixelPerfectCamera pixelPerfectCamera)
    {
        var settings = LocalStorage.GetSettings();
        int refResolutionX = pixelPerfectCamera.refResolutionX;
        int refResolutionY = pixelPerfectCamera.refResolutionY;

        int screenWidth = settings.windowWidth;
        int screenHeight = settings.windowHeight;

        // zoom level (PPU scale)
        int verticalZoom = screenHeight / refResolutionY;
        int horizontalZoom = screenWidth / refResolutionX;
        var zoom = Mathf.Max(1, Mathf.Min(verticalZoom, horizontalZoom));

        var offscreenRTWidth = screenWidth / zoom / 2 * 2;
        var offscreenRTHeight = screenHeight / zoom / 2 * 2;

        return new Vector2(offscreenRTWidth, offscreenRTHeight);
    }

    public static string GetHexColor(Color color)
    {
        var r = ((int)(color.r * 255)).ToString("X2");
        var g = ((int)(color.g * 255)).ToString("X2");
        var b = ((int)(color.b * 255)).ToString("X2");
        return "#" + r + g + b;
    }

    public static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }

        return path.Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }

    public static string[] GetDirectory(string path)
    {
        return Directory.GetDirectories(path)
            .Select(NormalizePath)
            .ToArray();
    }

    public static UVRect[] RectToUVRect(Rect[] rects)
    {
        return rects.Select(r => new UVRect(r)).ToArray();
    }

    public static Rect[] UVRectToRect(UVRect[] uvRects)
    {
        return uvRects.Select(r => new Rect(r.x, r.y, r.width, r.height)).ToArray();
    }


    public static ChatUserData DisassembleAccessValue(int avatarAccess)
    {
        var userdata = new ChatUserData
        {
            IsModerator = GetAccessBit(ChatterStatus.Moderator),
            IsPartner = GetAccessBit(ChatterStatus.Partner),
            IsPremium = GetAccessBit(ChatterStatus.Premium),
            IsAdmin = GetAccessBit(ChatterStatus.Admin),
            IsStuff = GetAccessBit(ChatterStatus.Stuff),
            IsVip = GetAccessBit(ChatterStatus.Vip),
            IsSubscriber = GetAccessBit(ChatterStatus.Subscriber)
        };
        return userdata;

        bool GetAccessBit(ChatterStatus chatterStatus)
        {
            return (avatarAccess & (int)chatterStatus) == (int)chatterStatus;
        }
    }

    public static bool AccessSuccess(int avatarAccess, ChatUserData chatUserData)
    {
        if (avatarAccess == 0) return true;
        var userAccess = GetAccessValue(chatUserData);
        return (userAccess & avatarAccess) > 0;
    }

    public static int GetAccessValue(ChatUserData chatUserData)
    {
        var access = 0;

        SetAccessBit(chatUserData.IsModerator, ChatterStatus.Moderator);
        SetAccessBit(chatUserData.IsPartner, ChatterStatus.Partner);
        SetAccessBit(chatUserData.IsPremium, ChatterStatus.Premium);
        SetAccessBit(chatUserData.IsAdmin, ChatterStatus.Admin);
        SetAccessBit(chatUserData.IsStuff, ChatterStatus.Stuff);
        SetAccessBit(chatUserData.IsVip, ChatterStatus.Vip);
        SetAccessBit(chatUserData.IsSubscriber, ChatterStatus.Subscriber);
        return access;

        void SetAccessBit(bool prev, ChatterStatus chatterStatus)
        {
            if (prev) access |= (int)chatterStatus;
        }
    }
}