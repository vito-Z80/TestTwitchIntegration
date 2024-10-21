using System;
using UnityEngine;
using UnityEngine.U2D;

namespace UI
{
    public class AvatarArea : MonoBehaviour
    {
        SpriteRenderer m_spriteRenderer;


        public static Action<Rect> OnRectSizeChanged;

        Launcher m_;
        PixelPerfectCamera m_pixelPerfectCamera;
        Camera m_camera;

        bool m_startDrag;

        Vector3 m_startDragPosition;
        Vector3 m_offsetDragPosition;
        Vector3 m_lastMousePosition;


        void OnEnable()
        {
            Configuration.OnAvatarAreaHorizontalChanged += ChangeAvatarAreaHorizontalSize;
            Configuration.OnAvatarAreaVerticalChanged += ChangeAvatarAreaVertical;
        }

        void OnDisable()
        {
            Configuration.OnAvatarAreaHorizontalChanged -= ChangeAvatarAreaHorizontalSize;
            Configuration.OnAvatarAreaVerticalChanged -= ChangeAvatarAreaVertical;
        }

        void Awake()
        {
            m_ = Launcher.Instance;
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_pixelPerfectCamera = m_.Ppc;
            m_camera = m_.Camera;
        }


        void Update()
        {
            Drag();

            if (m_offsetDragPosition != Vector3.zero)
            {
                var pos = FitScreen(m_spriteRenderer.bounds, transform.position += m_offsetDragPosition);
                transform.position = pos;
            }
        }

        public void RestoreRect(Vector3 position)
        {
            var pos = FitScreen(m_spriteRenderer.bounds, position);
            transform.position = pos;
        }

        void Drag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_startDragPosition = GetCursorPosition();
                m_lastMousePosition = m_startDragPosition;
                m_startDrag = Contains(m_spriteRenderer.bounds, m_startDragPosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_startDrag = false;
            }


            if (m_startDrag)
            {
                var currentPosition = GetCursorPosition();
                m_offsetDragPosition = currentPosition - m_lastMousePosition;
                m_lastMousePosition = currentPosition;

                m_startDrag = !OutOfScreen();
            }
            else
            {
                m_offsetDragPosition = Vector3.zero;
            }
        }

        Vector3 GetCursorPosition()
        {
            var pos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            return pos;
        }

        bool Contains(Bounds bounds, Vector3 point)
        {
            return bounds.Contains(point);
        }

        bool OutOfScreen()
        {
            var pos = GetCursorPosition();
            var minX = m_pixelPerfectCamera.transform.position.x - m_.WorldSize.x / 2.0f;
            var maxX = m_pixelPerfectCamera.transform.position.x + m_.WorldSize.x / 2.0f;
            var minY = m_pixelPerfectCamera.transform.position.y - m_.WorldSize.y / 2.0f;
            var maxY = m_pixelPerfectCamera.transform.position.y + m_.WorldSize.y / 2.0f;
            return pos.x < minX || pos.y < minY || pos.x > maxX || pos.y > maxY;
        }

        Vector3 FitScreen(Bounds bounds, Vector3 position)
        {
            var minX = m_pixelPerfectCamera.transform.position.x - m_.WorldSize.x / 2.0f + bounds.extents.x;
            var maxX = m_pixelPerfectCamera.transform.position.x + m_.WorldSize.x / 2.0f - bounds.extents.x;

            var minY = m_pixelPerfectCamera.transform.position.y - m_.WorldSize.y / 2.0f + bounds.extents.y;
            var maxY = m_pixelPerfectCamera.transform.position.y + m_.WorldSize.y / 2.0f - bounds.extents.y;

            var x = Mathf.Clamp(position.x, minX, maxX);
            var y = Mathf.Clamp(position.y, minY, maxY);

            var pos = new Vector3(x, y, 0);
            var rect = new Rect(pos - m_spriteRenderer.bounds.extents, m_spriteRenderer.bounds.size);
            OnRectSizeChanged?.Invoke(rect);
            return pos;
        }

        public void FillAvatarsAreaHorizontal()
        {
            transform.localScale = new Vector3(m_.WorldSize.x, transform.localScale.y, 1.0f);
            var pos = FitScreen(m_spriteRenderer.bounds, transform.position);
            transform.position = pos;
            Launcher.Instance.config.avatarAreaHorizontalSlider.value = m_.WorldSize.x / transform.localScale.x;
        }

        public void FIllAvatarsAreaVertical()
        {
            transform.localScale = new Vector3(transform.localScale.x, m_.WorldSize.y, 1.0f);
            var pos = FitScreen(m_spriteRenderer.bounds, transform.position);
            transform.position = pos;
            Launcher.Instance.config.avatarAreaVerticalSlider.value = m_.WorldSize.y / transform.localScale.y;
        }

        void ChangeAvatarAreaHorizontalSize(float sizeX)
        {
            transform.localScale = new Vector3(sizeX, transform.localScale.y, 1.0f);
            var pos = FitScreen(m_spriteRenderer.bounds, transform.position);
            transform.position = pos;
        }

        void ChangeAvatarAreaVertical(float sizeY)
        {
            transform.localScale = new Vector3(transform.localScale.x, sizeY, 1.0f);
            var pos = FitScreen(m_spriteRenderer.bounds, transform.position);
            transform.position = pos;
        }
    }
}