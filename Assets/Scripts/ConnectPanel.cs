using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPanel : MonoBehaviour
{
    [SerializeField] GameObject connectPanel;
    [SerializeField] public Button connectButton;

    [Space(8)] [SerializeField] public TMP_InputField clientId;
    [SerializeField] public TMP_InputField clientSecret;
    [SerializeField] public TMP_InputField redirect;
    [SerializeField] public TMP_InputField userName;
    [SerializeField] public TMP_InputField channelName;


    void Start()
    {
        connectButton.transform.SetAsLastSibling();
    }

    public void ClearConnectPanel()
    {
        clientId.text = string.Empty;
        clientSecret.text = string.Empty;
        redirect.text = string.Empty;
        channelName.text = string.Empty;
        userName.text = string.Empty;
        ConnectPanelVisible(true);
        
    }
    

    public void ConnectButtonEnabled(bool isEnabled) => connectButton.gameObject.SetActive(isEnabled);

    public void ConnectPanelVisible(bool isVisible)
    {
        clientSecret.gameObject.SetActive(isVisible);
        redirect.gameObject.SetActive(isVisible);
        clientId.gameObject.SetActive(isVisible);
        userName.gameObject.SetActive(isVisible);
        channelName.gameObject.SetActive(isVisible);
        connectButton.gameObject.SetActive(isVisible);
        connectButton.enabled = isVisible;
    }

    public void ConnectPanelRefreshVisible()
    {
        connectButton.gameObject.SetActive(true);
        clientSecret.gameObject.SetActive(true);
        redirect.gameObject.SetActive(false);
        clientId.gameObject.SetActive(false);
        userName.gameObject.SetActive(false);
        channelName.gameObject.SetActive(false);
        connectButton.enabled = true;
    }
}