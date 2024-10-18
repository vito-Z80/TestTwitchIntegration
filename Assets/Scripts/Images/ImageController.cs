using Data;
using Twitch;
using UnityEngine;

namespace Images
{
    public class ImageController : MonoBehaviour
    {
        LocalImageCollection m_localImageCollection;

        CentralImage m_centralImage;
        ImageData[] m_imageData;


        void OnEnable()
        {
            TwitchChatController.OnMessageSend += ShowImage;
        }

        void Start()
        {
            m_localImageCollection = new LocalImageCollection();
            m_imageData = m_localImageCollection.GetImages();
            m_centralImage = GetComponentInChildren<CentralImage>();
        }

        async void ShowImage(string message)
        {
            if (m_centralImage is not null && m_imageData is not null && m_imageData.Length > 0)
            {
                await m_centralImage.Show(message, m_imageData);    
            }
        }


        void OnDisable()
        {
            TwitchChatController.OnMessageSend -= ShowImage;
        }
    }
}