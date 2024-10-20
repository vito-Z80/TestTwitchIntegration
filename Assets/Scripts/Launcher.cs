using Twitch;
using UI;
using UnityEngine;
using UnityEngine.U2D;

public class Launcher : MonoBehaviour
{
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] ConnectPanel connectPanel;
    [SerializeField] public Configuration config;
    [Header("Configs")]
    [SerializeField] GameObject avatarsConfig;
    Connect m_connect;

    Camera m_camera;

    Vector2Int m_windowSize;
    Vector2 m_worldSize;

    public static Launcher Instance { get; private set; }

    public Vector2Int WindowSize => m_windowSize;
    public Vector2 WorldSize => m_worldSize;
    public PixelPerfectCamera Ppc => pixelPerfectCamera;
    public Camera Camera => m_camera;

    void Awake()
    {
        m_camera = pixelPerfectCamera.GetComponent<Camera>();
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
        connectPanel.gameObject.SetActive(false);
        Application.runInBackground = true;
        m_connect = new Connect(connectPanel);
        await m_connect.AutoConnectTwitch();
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
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
    }

    void OnApplicationQuit()
    {
        m_connect.OnApplicationQuit();
    }

    void CorrectRuntimeSizeWindowSize()
    {
        if (!WasWindowResized()) return;
        // if (!Input.GetMouseButtonUp(0)) return;
        m_windowSize.x = Screen.width / 2 * 2;
        m_windowSize.y = Screen.height / 2 * 2;
        var windowPosition = Screen.mainWindowPosition;
        // Screen.SetResolution(m_windowSize.x, m_windowSize.y, FullScreenMode.Windowed);
        var displayInfo = Screen.mainWindowDisplayInfo;
        displayInfo.width = m_windowSize.x;
        displayInfo.height = m_windowSize.y;
        Screen.MoveMainWindowTo(displayInfo, windowPosition);
        m_worldSize = GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
        Debug.Log(Screen.width + "|" + Screen.height);
        Debug.Log(m_worldSize);
    }

    public bool WasWindowResized()
    {
        return Screen.width != m_windowSize.x || Screen.height != m_windowSize.y;
    }

    static Vector2 GetScreenSize(PixelPerfectCamera pixelPerfectCamera)
    {
        int refResolutionX = pixelPerfectCamera.refResolutionX;
        int refResolutionY = pixelPerfectCamera.refResolutionY;

        Debug.Log(refResolutionX + "|" + refResolutionY);
        
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


    public void ChangeWorldSize(int scale)
    {
        pixelPerfectCamera.assetsPPU = scale;
        m_worldSize = GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
    }
    
    void ChangePixelSnap(bool isPixelSnap)
    {
        pixelPerfectCamera.pixelSnapping = isPixelSnap;
    }


    bool m_isAvatarConfigShowing;

    public void ShowAvatarsConfig()
    {
        avatarsConfig.SetActive(m_isAvatarConfigShowing);
        m_isAvatarConfigShowing = !m_isAvatarConfigShowing;
    }
    public void HideAvatarsConfig() => avatarsConfig.SetActive(false);
}