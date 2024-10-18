// using System;
// using System.Runtime.InteropServices;
using Twitch;
using UI;
using UnityEngine;

//  transparent window: https://youtube.com/watch?v=RqgsGaMPZTw

public class Launcher : MonoBehaviour
{
    [SerializeField] ConnectPanel connectPanel;
    Connect m_connect;

    // [DllImport("user32.dll")]
    // static extern IntPtr GetActiveWindow();
    // [DllImport("user32.dll", SetLastError = true)]
    // static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    // [DllImport("Dwmapi.dll")]
    // private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    // [DllImport("user32.dll", SetLastError = true)]
    // static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    // const int GWL_EXSTYLE = -20;
    // const uint WS_EX_LAYERED = 0x00080000;
    // const uint WS_EX_TRANSPARENT = 0x00000020;
    // static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    // private struct MARGINS
    // {
    //     public int cxLeftWidth;
    //     public int cxRightWidth;
    //     public int cyTopHeight;
    //     public int cyBottomHeight;
    // }

    void OnEnable()
    {
        TwitchChatClient.OnConnectPanelVisible += ConnectPanelVisible;
    }

    async void Start()
    {

// #if !UNITY_EDITOR
//         IntPtr hWnd = GetActiveWindow();
//         MARGINS margins = new MARGINS { cxLeftWidth = -1 };
//         DwmExtendFrameIntoClientArea(hWnd, ref margins);
//         SetWindowLong(hWnd, GWL_EXSTYLE , WS_EX_LAYERED | WS_EX_TRANSPARENT);
//         SetWindowPos(hWnd,  HWND_TOPMOST, 0, 0, 0, 0, 0);
// #endif
        
        // PlayerPrefs.DeleteAll();
        connectPanel.gameObject.SetActive(false);
        Application.runInBackground = true;
        m_connect = new Connect(connectPanel);
        await m_connect.AutoConnectTwitch();
    }


    void Update()
    {
        m_connect.Update();
    }

    public void Connect()
    {
        m_connect.ConnectTwitch();
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
    }

    void OnApplicationQuit()
    {
        m_connect.OnApplicationQuit();
    }
}