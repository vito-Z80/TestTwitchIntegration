using System.Threading.Tasks;
using Data;
using Twitch;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Core : MonoBehaviour
{
    [SerializeField] PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] ConnectPanel connectPanel;

    AppSettingsData m_settings;
    [SerializeField] GameObject userInterface;
    [SerializeField] GameObject avatarArea;
    [SerializeField] GameObject masterVolume;
    
    
    [Header("Cursor")]
    [SerializeField] Texture2D arrowCursor;
    [SerializeField] Texture2D handCursor;
    
    Connect m_connect;

    Camera m_camera;

    Vector2Int m_windowSize;
    Vector2 m_worldSize;

    Vector2 m_defaultCursorPivot;
    Vector2 m_handCursorPivot;
    
    
    public static Core Instance { get; private set; }

    public Vector2 WorldSize => m_worldSize;
    public PixelPerfectCamera Ppc => pixelPerfectCamera;
    public Camera Camera => m_camera;


    void Awake()
    {
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();
        m_settings = LocalStorage.GetSettings();
        ChangeCameraPpu(m_settings.cameraPpu);
        m_camera = pixelPerfectCamera.GetComponent<Camera>();
        m_defaultCursorPivot = new Vector2(0.185050011f, 0.964990616f);
        m_handCursorPivot = new Vector2(0.418897033f, 0.92038399f);
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


     void Start()
     {
        connectPanel.gameObject.SetActive(false);
        Application.runInBackground = true;
        m_connect = new Connect(connectPanel);
        Task.FromResult(m_connect.AutoConnectTwitch());     //  its legal here ?
        // await m_connect.AutoConnectTwitch();
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
        SetDefaultCursor();
        LocalStorage.SaveSettings();
        LocalStorage.SaveAvatarsData();
        Log.SaveLog();
        m_connect.OnApplicationQuit();
    }

    public void SetResolution(int width, int height)
    {
        m_windowSize.x = width;
        m_windowSize.y = height;
        Screen.SetResolution(m_windowSize.x, m_windowSize.y, false);
        m_worldSize = Utils.GetScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
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
    
    public  void SetDefaultCursor()
    {
        // if (arrowCursor != null)
        //     Cursor.SetCursor(arrowCursor, m_defaultCursorPivot, CursorMode.Auto);
    }

    public  void SetDragCursor()
    {
        // if (handCursor != null)
        //     Cursor.SetCursor(handCursor, m_handCursorPivot, CursorMode.Auto);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        userInterface.SetActive(hasFocus);
        avatarArea.SetActive(hasFocus);
        masterVolume.SetActive(hasFocus);
    }
}