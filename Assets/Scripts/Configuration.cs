using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
    public const float MaxAvatarSpeed = 10.0f;
    public const float MaxCameraPpu = 24.0f;

    AppSettingsData m_settings;

    [Header("Sliders")] [SerializeField] Slider avatarSpeedSlider;
    [SerializeField] Slider ppuSizeSlider;
    [SerializeField] Slider avatarAreaHorizontalSlider;
    [SerializeField] Slider avatarAreaVerticalSlider;
    [SerializeField] Slider imageScaleSlider;

    [Header("Toggles")] [SerializeField] Toggle pixelSnap;
    [SerializeField] Toggle randomSpeed;


    void Start()
    {
        m_settings = LocalStorage.GetSettings();
        Initialize();
    }


    void Initialize()
    {
        avatarSpeedSlider.value = m_settings.avatarsSpeed;
        ppuSizeSlider.value = m_settings.cameraPpu;
        avatarAreaHorizontalSlider.value = m_settings.avatarAreaWidth;
        avatarAreaVerticalSlider.value = m_settings.avatarAreaHeight;
        imageScaleSlider.value = m_settings.imageScale;

        pixelSnap.isOn = m_settings.pixelSnapping;
        randomSpeed.isOn = m_settings.randomSpeedEnabled;
    }

    public void FillAvatarAreaHeight()
    {
        m_settings.avatarAreaHeight = 1.0f;
        avatarAreaVerticalSlider.value = 1.0f;
    }

    public void FillAvatarAreaWidth()
    {
        m_settings.avatarAreaWidth = 1.0f;
        avatarAreaHorizontalSlider.value = 1.0f;
    }

    public void IsRandomSpeedEnabled(bool value)
    {
        m_settings.randomSpeedEnabled = value;
    }

    public void IsPixelSnappingEnabled(bool value)
    {
        m_settings.pixelSnapping = value;
    }


    public void SetAvatarsSpeed(float value)
    {
        m_settings.avatarsSpeed = value /* * MaxAvatarSpeed*/;
    }


    public void SetImageScale(float value)
    {
        m_settings.imageScale = value;
    }

    public void SetPpuSize(float value)
    {
        m_settings.cameraPpu = value /* * MaxCameraPpu*/;
    }

    public void SetAvatarAreaWidth(float value)
    {
        m_settings.avatarAreaWidth = /*Launcher.Instance.WorldSize.x **/ value;
    }

    public void SetAvatarAreaHeight(float value)
    {
        m_settings.avatarAreaHeight = /*Launcher.Instance.WorldSize.y **/ value;
    }
}
