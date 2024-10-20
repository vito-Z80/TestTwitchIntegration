using System;
using UnityEngine;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
    [Header("Sliders")] [SerializeField] public Slider avatarSpeedSlider;
    [SerializeField] public Slider worldSizeSlider;
    [SerializeField] public Slider avatarAreaHorizontalSlider;
    [SerializeField] public Slider avatarAreaVerticalSlider;

    [Header("Toggles")] [SerializeField] Toggle testAvatars;
    [SerializeField] Toggle pixelSnap;

    [Header("Values")] [SerializeField] public float avatarsSpeed;
    [SerializeField] public float worldSize;

    const float MaxAvatarSpeed = 10.0f;
    const float MaxWorldSize = 24.0f;

    public static event Action<int> OnWorldSizeChanged;
    public static event Action<float> OnAvatarAreaHorizontalChanged;
    public static event Action<float> OnAvatarAreaVerticalChanged;
    public static event Action<bool> OnAvatarsTest;
    public static event Action<bool> OnPixelSnap;

    void Awake()
    {
        worldSizeSlider.value = 1.0f / MaxWorldSize * worldSize;
        avatarSpeedSlider.value = 1.0f / MaxAvatarSpeed * avatarsSpeed;
    }

    public void PixelSnap()
    {
        OnPixelSnap?.Invoke(pixelSnap.isOn);
    }

    public void TestAvatars()
    {
        OnAvatarsTest?.Invoke(testAvatars.isOn);
    }

    public void SetAvatarsSpeed()
    {
        avatarsSpeed = avatarSpeedSlider.value * MaxAvatarSpeed;
    }

    public void SetWorldSize()
    {
        worldSize = worldSizeSlider.value * MaxWorldSize;
        OnWorldSizeChanged?.Invoke((int)worldSize);
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
}