using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
    [Header("Sliders")] [SerializeField] public Slider avatarSpeedSlider;
    [SerializeField] public Slider worldSizeSlider;
    [SerializeField] public Slider avatarAreaHorizontalSlider;
    [SerializeField] public Slider avatarAreaVerticalSlider;
    [SerializeField] public Slider imageScaleSlider;

    [Header("Toggles")] [SerializeField] Toggle show;
    [SerializeField] Toggle pixelSnap;
    [SerializeField] public Toggle randomSpeed;

    [Header("Values")] [SerializeField] public float avatarsSpeed;
    [SerializeField] public float worldSize;

    [Header("Dropdown")] [SerializeField] public TMP_Dropdown chromakey;

    const float MaxAvatarSpeed = 10.0f;
    public const float MaxWorldSize = 24.0f;

    public static event Action<float> OnWorldSizeChanged;
    public static event Action<float> OnAvatarAreaHorizontalChanged;
    public static event Action<float> OnAvatarAreaVerticalChanged;
    public static event Action<bool> OnShowTest;
    public static event Action<bool> OnPixelSnap;
    public static event Action<bool> OnRandomSpeed;
    public static event Action<float> OnImageScaleChanged;
    public void SetImageScale()
    {
        OnImageScaleChanged?.Invoke(imageScaleSlider.value);
    }

    public void SetRandomSpeed()
    {
        OnRandomSpeed?.Invoke(randomSpeed.isOn);
    }

    public void PixelSnap()
    {
        OnPixelSnap?.Invoke(pixelSnap.isOn);
    }

    public void Show()
    {
        OnShowTest?.Invoke(show.isOn);
    }

    public void SetAvatarsSpeed()
    {
        avatarsSpeed = avatarSpeedSlider.value * MaxAvatarSpeed;
    }

    public void SetWorldSize()
    {
        worldSize = worldSizeSlider.value * MaxWorldSize;
        OnWorldSizeChanged?.Invoke(worldSize);
        ChangeAvatarAreaHorizontalSize();
        ChangeAvatarAreaVerticalSize();
    }

    public void ChangeAvatarAreaHorizontalSize()
    {
        var sizeX = Launcher.Instance.WorldSize.x * avatarAreaHorizontalSlider.value;
        OnAvatarAreaHorizontalChanged?.Invoke(sizeX);
    }

    public void ChangeAvatarAreaVerticalSize()
    {
        var sizeY = Launcher.Instance.WorldSize.y * avatarAreaVerticalSlider.value;
        OnAvatarAreaVerticalChanged?.Invoke(sizeY);
    }

    public AppSettings GetSettings()
    {
        var settings = new AppSettings
        {
            avatarAreaHeight = avatarAreaVerticalSlider.value,
            avatarAreaWidth = avatarAreaHorizontalSlider.value,
            worldSize = worldSize,
            avatarsSpeed = avatarSpeedSlider.value,
            randomSpeedEnabled = randomSpeed.isOn,
            pixelSnapping = pixelSnap.isOn,
            chromakeyId = chromakey.value,
            imageScale = imageScaleSlider.value,
        };
        return settings;
    }

    public void SetSettings(AppSettings settings)
    {
        avatarAreaVerticalSlider.value = settings.avatarAreaHeight;
        avatarAreaHorizontalSlider.value = settings.avatarAreaWidth;
        worldSize = settings.worldSize;
        avatarSpeedSlider.value = settings.avatarsSpeed;
        randomSpeed.isOn = settings.randomSpeedEnabled;
        pixelSnap.isOn = settings.pixelSnapping;
        chromakey.value = settings.chromakeyId;
        imageScaleSlider.value = settings.imageScale;

        worldSizeSlider.value = 1.0f / MaxWorldSize * worldSize;
        SetWorldSize();
    }
}