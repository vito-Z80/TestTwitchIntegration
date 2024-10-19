using System;
using UnityEngine;
using UnityEngine.U2D;

public static class Utils
{
    public static Vector2 GetOffScreenSize(PixelPerfectCamera pixelPerfectCamera)
    {
        int refResolutionX = pixelPerfectCamera.refResolutionX;
        int refResolutionY = pixelPerfectCamera.refResolutionY;

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // zoom level (PPU scale)
        int verticalZoom = screenHeight / refResolutionY;
        int horizontalZoom = screenWidth / refResolutionX;
        var zoom = Mathf.Max(1, Mathf.Min(verticalZoom, horizontalZoom));

        var offscreenRTWidth = screenWidth / zoom / 2 * 2;
        var offscreenRTHeight = screenHeight / zoom / 2 * 2;

        return new Vector2(offscreenRTWidth, offscreenRTHeight);
    }
}