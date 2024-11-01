using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

namespace UI.AvatarAreaWindow
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
            var pos = new Vector3(
                (m_settings.areaPosX - 0.5f + m_settings.avatarAreaWidth / 2.0f) * m_.WorldSize.x,
                (m_settings.areaPosY - 0.5f + m_settings.avatarAreaHeight / 2.0f) * m_.WorldSize.y,
                0.0f);
            transform.position = pos;
            ChangeAvatarAreaWidth();
            ChangeAvatarAreaHeight();
            FitScreen(m_spriteRenderer.bounds, pos);
        }

        void Update()
        {
            Drag();
            if (m_offsetDragPosition != Vector3.zero)
            {
                FitScreen(m_spriteRenderer.bounds, transform.position += m_offsetDragPosition);
            }

        }


        void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                m_.SetDefaultCursor();
            }
            else
            {
                m_.SetDragCursor();
            }
        }

        void OnMouseExit()
        {
            m_.SetDefaultCursor();
        }

        void Drag()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
            var halfWorldWidth = m_.WorldSize.x / 2.0f;
            var halfWorldHeight = m_.WorldSize.y / 2.0f;

            var minX = m_pixelPerfectCamera.transform.position.x - halfWorldWidth;
            var maxX = m_pixelPerfectCamera.transform.position.x + halfWorldWidth;
            var minY = m_pixelPerfectCamera.transform.position.y - halfWorldHeight;
            var maxY = m_pixelPerfectCamera.transform.position.y + halfWorldHeight;
            return pos.x < minX || pos.y < minY || pos.x > maxX || pos.y > maxY;
        }

        void FitScreen(Bounds bounds, Vector3 position)
        {
            var halfWorldWidth = m_.WorldSize.x / 2.0f;
            var halfWorldHeight = m_.WorldSize.y / 2.0f;
            var minX = m_pixelPerfectCamera.transform.position.x - halfWorldWidth + bounds.extents.x;
            var maxX = m_pixelPerfectCamera.transform.position.x + halfWorldWidth - bounds.extents.x;

            var minY = m_pixelPerfectCamera.transform.position.y - halfWorldHeight + bounds.extents.y;
            var maxY = m_pixelPerfectCamera.transform.position.y + halfWorldHeight - bounds.extents.y;

            var x = Mathf.Clamp(position.x, minX, maxX);
            var y = Mathf.Clamp(position.y, minY, maxY);

            var pos = new Vector3(x, y, 0); //  fitting world position
            transform.position = pos;
            Rect.position = pos - bounds.extents; //  top-left
            Rect.size = bounds.size; //  size

            m_settings.areaPosX = (pos.x - bounds.extents.x) / m_.WorldSize.x + 0.5f;
            m_settings.areaPosY = (pos.y - bounds.extents.y) / m_.WorldSize.y + 0.5f;
        }

        public void ChangeAvatarPosition()
        {
            var bounds = m_spriteRenderer.bounds;
            var pos = new Vector3(
                          m_settings.areaPosX * m_.WorldSize.x + bounds.extents.x,
                          m_settings.areaPosY * m_.WorldSize.y + bounds.extents.y,
                          0.0f)
                      - (Vector3)m_.WorldSize / 2.0f;
            transform.position = pos;
            Rect.position = pos - bounds.extents; //  top-left
            Rect.size = bounds.size; //  size
        }

        public void ChangeAvatarAreaWidth()
        {
            transform.localScale = new Vector3(m_settings.avatarAreaWidth * m_.WorldSize.x, transform.localScale.y, 1.0f);
            FitScreen(m_spriteRenderer.bounds, transform.position);
        }

        public void ChangeAvatarAreaHeight()
        {
            transform.localScale = new Vector3(transform.localScale.x, m_settings.avatarAreaHeight * m_.WorldSize.y, 1.0f);
            FitScreen(m_spriteRenderer.bounds, transform.position);
        }

        public void SetVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}