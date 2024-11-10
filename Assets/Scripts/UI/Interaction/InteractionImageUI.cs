using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interaction
{
    public class InteractionImageUI : MonoBehaviour
    {
        Image m_image;

        int m_animationPlayTimes = 1;
        readonly List<InteractionFrame> m_frames = new();


        //  TODO кэшировать использованные анимации ?

        IEnumerator Play()
        {
            if (m_frames.Count == 0) yield break;
            var frameId = 0;
            m_image.sprite = m_frames[frameId].Sprite;
            m_image.color = Color.white;
            for (var i = 0; i < m_animationPlayTimes; i++)
            {
                while (frameId < m_frames.Count)
                {
                    m_image.sprite = m_frames[frameId].Sprite;
                    yield return new WaitForSeconds(m_frames[frameId].Delay);
                    frameId++;
                }

                yield return new WaitForSeconds(m_frames[0].Delay);
            }

            yield return null;
        }

        public async Task ShowImage(string imagesPath)
        {
            m_image ??= GetComponent<Image>();
            m_image.color = new Color(1, 1, 1, 0);

            if (!Directory.Exists(imagesPath)) return;

            await CreateFrames(imagesPath);
            StartCoroutine(Play());
        }

        Task CreateFrames(string imagesPath)
        {
            m_frames.Clear();
            var paths = LocalStorage.GetFilesByExtension(imagesPath, "png");

            foreach (var path in paths)
            {
                var sprite = LocalStorage.LoadSprite(path);
                var frame = new InteractionFrame(sprite, 1.0f / 12.0f);
                m_frames.Add(frame);
            }

            return Task.CompletedTask;
        }
    }

    struct InteractionFrame
    {
        public readonly Sprite Sprite;
        public readonly float Delay;

        public InteractionFrame(Sprite sprite, float delay)
        {
            Sprite = sprite;
            Delay = delay;
        }
    }
}