using Data;
using Newtonsoft.Json;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.U2D;

public class Launcher : MonoBehaviour
{
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] ConnectPanel connectPanel;
    [SerializeField] public Configuration config;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject avatarArea;
    Connect m_connect;

    Camera m_camera;

    Vector2Int m_windowSize;
    Vector2 m_worldSize;

    Color m_baseColor;

    const string ConfigKey = "ConfigKey";

    public static Launcher Instance { get; private set; }

    public Vector2 WorldSize => m_worldSize;
    public PixelPerfectCamera Ppc => pixelPerfectCamera;
    public Camera Camera => m_camera;

    void Awake()
    {
        m_camera = pixelPerfectCamera.GetComponent<Camera>();
        m_baseColor = m_camera.backgroundColor;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    void OnEnable()
    {
        TwitchChatClient.OnConnectPanelVisible += ConnectPanelVisible;
        Configuration.OnWorldSizeChanged += ChangeWorldSize;
        Configuration.OnPixelSnap += ChangePixelSnap;
    }


    async void Start()
    {
        // PlayerPrefs.DeleteAll();
        Log.LogMessage($"Starting Twitch Chat Client: {GetHashCode()}");
        m_camera.backgroundColor = m_baseColor;
        connectPanel.gameObject.SetActive(false);
        Application.runInBackground = true;
        m_connect = new Connect(connectPanel);
        await m_connect.AutoConnectTwitch();
        Application.targetFrameRate = 60;
    }


    void Update()
    {
        CorrectRuntimeSizeWindowSize();

        m_connect?.Update();
    }


    public void Connect()
    {
        m_connect?.ConnectTwitch();
    }

    public void LogOut()
    {
        m_connect.LogOut();
        connectPanel.ClearConnectPanel();
    }

    void ConnectPanelVisible(bool isVisible)
    {
        connectPanel.gameObject.SetActive(isVisible);
    }

    void OnDisable()
    {
        TwitchChatClient.OnConnectPanelVisible -= ConnectPanelVisible;
        Configuration.OnWorldSizeChanged -= ChangeWorldSize;
        Configuration.OnPixelSnap -= ChangePixelSnap;
    }

    
    void OnApplicationQuit()
    {
        m_connect.OnApplicationQuit();
    }

    void CorrectRuntimeSizeWindowSize()
    {
        if (!WasWindowResized()) return;
        m_windowSize.x = Screen.width / 2 * 2;
        m_windowSize.y = Screen.height / 2 * 2;
        var windowPosition = Screen.mainWindowPosition;
        var displayInfo = Screen.mainWindowDisplayInfo;
        displayInfo.width = m_windowSize.x;
        displayInfo.height = m_windowSize.y;
        Screen.MoveMainWindowTo(displayInfo, windowPosition);
        m_worldSize = GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
    }

    public bool WasWindowResized()
    {
        return Screen.width != m_windowSize.x || Screen.height != m_windowSize.y;
    }

    static Vector2 GetScreenSize(PixelPerfectCamera pixelPerfectCamera)
    {
        int refResolutionX = pixelPerfectCamera.refResolutionX;
        int refResolutionY = pixelPerfectCamera.refResolutionY;

        // Log.LogMessage(refResolutionX + "|" + refResolutionY);   TODO через кадр другие размеры, не то что бы на 1 а раза в 3.

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


    internal void ChangeWorldSize(float scale)
    {
        pixelPerfectCamera.assetsPPU = (int)scale;
        m_worldSize = GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
    }

    internal void ChangePixelSnap(bool isPixelSnap)
    {
        pixelPerfectCamera.pixelSnapping = isPixelSnap;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ShowAll();
        }
        else
        {
            HideAll();
        }
    }
    

    AppSettings HideAll()
    {
        menu.SetActive(false);
        avatarArea.SetActive(false);
        var settings = config.GetSettings();
        m_camera.backgroundColor = Utils.GetChromakeyColor(settings.chromakeyId);
        settings.areaPosX = avatarArea.transform.position.x;
        settings.areaPosY = avatarArea.transform.position.y;
        var json = JsonConvert.SerializeObject(settings);
        PlayerPrefs.SetString(ConfigKey, json);
        PlayerPrefs.Save();
        // Log.LogMessage($"HIDE: {json}");
        Log.SaveLog();
        return settings;
    }

    void ShowAll()
    {
        m_camera.backgroundColor = m_baseColor;
        if (PlayerPrefs.HasKey(ConfigKey))
        {
            var json = PlayerPrefs.GetString(ConfigKey);
            var settings = JsonConvert.DeserializeObject<AppSettings>(json);
            config.SetSettings(settings);
            avatarArea.GetComponent<AvatarArea>().RestoreRect(new Vector3(settings.areaPosX, settings.areaPosY, 0.0f));
            // Log.LogMessage($"SHOW {avatarArea.transform.position}: {json}");
        }
        else
        {
            HideAll();
        }

        avatarArea.SetActive(true);
        menu.SetActive(true);
    }
}