using Data;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.U2D;

public class Core : MonoBehaviour
{
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] ConnectPanel connectPanel;

    AppSettingsData m_settings;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject avatarArea;
    Connect m_connect;

    Camera m_camera;

    Vector2Int m_windowSize;
    Vector2 m_worldSize;
    
    public static Core Instance { get; private set; }

    public Vector2 WorldSize => m_worldSize;
    public PixelPerfectCamera Ppc => pixelPerfectCamera;
    public Camera Camera => m_camera;


    void Awake()
    {
        m_settings = LocalStorage.GetSettings();
        ChangeCameraPpu(m_settings.cameraPpu);
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
    }
    
    void OnDisable()
    {
        TwitchChatClient.OnConnectPanelVisible -= ConnectPanelVisible;
    }


    async void Start()
    {
        // PlayerPrefs.DeleteAll();
        connectPanel.gameObject.SetActive(false);
        Application.runInBackground = true;
        m_connect = new Connect(connectPanel);
        await m_connect.AutoConnectTwitch();
        Application.targetFrameRate = 60;
    }


    void Update()
    {
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
    


    void OnApplicationQuit()
    {
        LocalStorage.SaveSettings();
        Log.SaveLog();
        m_connect.OnApplicationQuit();
    }

    public void SetResolution(int width, int height)
    {
        m_windowSize.x = width;
        m_windowSize.y = height;
        Screen.SetResolution(m_windowSize.x, m_windowSize.y, false);
        m_worldSize = Utils.GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
        Log.LogMessage(m_worldSize.ToString());
        Log.LogMessage(m_windowSize.ToString());
    }

    public void ChangeCameraPpu(float scale)
    {
        pixelPerfectCamera.assetsPPU = (int)scale;
        m_worldSize = Utils.GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
    }

    public void ChangePixelSnap(bool isPixelSnap)
    {
        pixelPerfectCamera.pixelSnapping = isPixelSnap;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        menu.SetActive(hasFocus);
        avatarArea.SetActive(hasFocus);
    }
}