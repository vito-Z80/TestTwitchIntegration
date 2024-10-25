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
}