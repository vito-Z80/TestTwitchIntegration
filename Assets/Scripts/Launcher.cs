using System;
using Twitch;
using UI;
using UnityEngine;


public class Launcher : MonoBehaviour
{
    [SerializeField] ConnectPanel connectPanel;
    Connect m_connect;
    
    
    
    public static Launcher Instance { get; private set; }

    void Awake()
    {
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