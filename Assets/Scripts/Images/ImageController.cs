﻿using System.Collections.Generic;
using Twitch;
using UnityEngine;

namespace Images
{
    public class ImageController : MonoBehaviour
    {
        LocalImageCollection m_localImageCollection;

        Dictionary<string, Sprite> m_images;
        CentralImage m_centralImage;


        void OnEnable()
        {
            TwitchChatController.OnImageShown += ShowImage;
        }
        
        void Start()
        {
            m_localImageCollection = new LocalImageCollection();
            m_images = m_localImageCollection.GetImages();
            m_centralImage = GetComponentInChildren<CentralImage>();
        }



        public Dictionary<string, Sprite>.KeyCollection GetImageNames() => m_images.Keys;
        
        void ShowImage(string imageName)
        {
            if (m_images.TryGetValue(imageName, out var sprite))
            {
                m_centralImage.Show(sprite);
            }
        }


        void OnDisable()
        {
            TwitchChatController.OnImageShown -= ShowImage;
        }
    }
}