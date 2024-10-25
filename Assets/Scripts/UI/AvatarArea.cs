using Data;
using UnityEngine;
using UnityEngine.U2D;

namespace UI
{
    public class AvatarArea : MonoBehaviour
    {
        SpriteRenderer m_spriteRenderer;

        Core m_;
        PixelPerfectCamera m_pixelPerfectCamera;
        Camera m_camera;

        bool m_startDrag;

        Vector3 m_startDragPosition;
        Vector3 m_offsetDragPosition;
        Vector3 m_lastMousePosition;
        AppSettingsData m_settings;

        public static Rect Rect;

        void Awake()
        {
            m_ = Core.Instance;
            m_settings = LocalStorage.GetSettings();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_pixelPerfectCamera = m_.Ppc;
            m_camera = m_.Camera;
        }


        void OnEnable()
        {
            transform.position = new Vector3(m_settings.areaPosX, m_settings.areaPosY, 0);
            var pos = new Vector3(m_settings.areaPosX, m_settings.areaPosY, 0.0f);
            FitScreen(m_spriteRenderer.bounds, pos);
        }

        void Update()
        {
            ChangeAvatarAreaHorizontalSize();
            ChangeAvatarAreaVertical();
            
            Drag();
            if (m_offsetDragPosition != Vector3.zero)
            {
                FitScreen(m_spriteRenderer.bounds, transform.position += m_offsetDragPosition);
            }
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

        void FitScreen(Bounds bounds, Vector3 position)
        {
            var minX = m_pixelPerfectCamera.transform.position.x - m_.WorldSize.x / 2.0f + bounds.extents.x;
            var maxX = m_pixelPerfectCamera.transform.position.x + m_.WorldSize.x / 2.0f - bounds.extents.x;

            var minY = m_pixelPerfectCamera.transform.position.y - m_.WorldSize.y / 2.0f + bounds.extents.y;
            var maxY = m_pixelPerfectCamera.transform.position.y + m_.WorldSize.y / 2.0f - bounds.extents.y;

            var x = Mathf.Clamp(position.x, minX, maxX);
            var y = Mathf.Clamp(position.y, minY, maxY);

            var pos = new Vector3(x, y, 0);
            transform.position = pos;
            Rect.position = pos - m_spriteRenderer.bounds.extents;
            Rect.size = m_spriteRenderer.bounds.size;

            m_settings.areaPosX = pos.x;
            m_settings.areaPosY = pos.y;
        }

        void ChangeAvatarAreaHorizontalSize()
        {
            transform.localScale = new Vector3(m_settings.avatarAreaWidth * m_.WorldSize.x, transform.localScale.y, 1.0f);
            FitScreen(m_spriteRenderer.bounds, transform.position);
        }

        void ChangeAvatarAreaVertical()
        {
            transform.localScale = new Vector3(transform.localScale.x, m_settings.avatarAreaHeight * m_.WorldSize.y, 1.0f);
            FitScreen(m_spriteRenderer.bounds, transform.position);
        }
    }
}