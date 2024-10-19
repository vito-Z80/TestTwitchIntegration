using UnityEngine;
using UnityEngine.U2D;

namespace gl
{
    public class Areas : MonoBehaviour
    {
        [SerializeField] PixelPerfectCamera pixelPerfectCamera;
        [SerializeField] SpriteRenderer spriteRenderer;

        Camera m_camera;
        Vector2 m_screenSize;


        void Start()
        {
            m_camera = pixelPerfectCamera.GetComponent<Camera>();
            m_screenSize = Utils.GetOffScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
            FillScreen();
        }


        void Update()
        {
            m_screenSize = Utils.GetOffScreenSize(pixelPerfectCamera) / pixelPerfectCamera.assetsPPU;
            Drag();

            if (m_offsetDragPosition != Vector3.zero)
            {
                var pos = FitScreen(spriteRenderer.bounds, spriteRenderer.transform.position += m_offsetDragPosition);
                spriteRenderer.transform.position = pos;
            }
        }

        void FillHorizontal()
        {
        }

        void FIllVertical()
        {
        }

        void FillScreen()
        {
        }

        Vector3 FitScreen(Bounds bounds, Vector3 position)
        {
            var minX = pixelPerfectCamera.transform.position.x - m_screenSize.x / 2.0f + bounds.extents.x;
            var maxX = pixelPerfectCamera.transform.position.x + m_screenSize.x / 2.0f - bounds.extents.x;

            var minY = pixelPerfectCamera.transform.position.y - m_screenSize.y / 2.0f + bounds.extents.y;
            var maxY = pixelPerfectCamera.transform.position.y + m_screenSize.y / 2.0f - bounds.extents.y;

            var x = Mathf.Clamp(position.x, minX, maxX);
            var y = Mathf.Clamp(position.y, minY, maxY);

            return new Vector3(x, y, 0.0f);
        }


        bool m_startDrag;
        Vector3 m_startDragPosition;
        Vector3 m_offsetDragPosition;
        Vector3 m_lastMousePosition;

        void Drag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_startDragPosition = GetCursorPosition();
                m_lastMousePosition = m_startDragPosition;
                m_startDrag = Contains(spriteRenderer.bounds, m_startDragPosition);
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
            var minX = pixelPerfectCamera.transform.position.x - m_screenSize.x / 2.0f;
            var maxX = pixelPerfectCamera.transform.position.x + m_screenSize.x / 2.0f;
            var minY = pixelPerfectCamera.transform.position.y - m_screenSize.y / 2.0f;
            var maxY = pixelPerfectCamera.transform.position.y + m_screenSize.y / 2.0f;
            return pos.x < minX || pos.y < minY || pos.x > maxX || pos.y > maxY;
        }
    }
}