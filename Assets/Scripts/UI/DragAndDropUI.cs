using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DragAndDropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Canvas canvas;
        RectTransform m_rectTransform;
        RectTransform m_canvasRect;
        CanvasGroup m_canvasGroup;

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvasRect = canvas.GetComponent<RectTransform>();
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnEnable()
        {
            m_rectTransform.anchoredPosition = FitToScreen(m_rectTransform.anchoredPosition);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
            m_canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var newPosition = m_rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;
            m_rectTransform.anchoredPosition = FitToScreen(newPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_canvasGroup.blocksRaycasts = true;
        }

        Vector2 FitToScreen(Vector2 position)
        {
            var canvasSize = m_canvasRect.sizeDelta;
            var elementSize = m_rectTransform.sizeDelta;

            var canvasHalfWidth = canvasSize.x / 2.0f;
            var canvasHalfHeight = canvasSize.y / 2.0f;

            var minX = -canvasHalfWidth + elementSize.x * m_rectTransform.pivot.x + canvasHalfWidth;
            var maxX = canvasHalfWidth - elementSize.x * (1 - m_rectTransform.pivot.x) + canvasHalfWidth;
            var minY = -canvasHalfHeight + elementSize.y * m_rectTransform.pivot.y - canvasHalfHeight;
            var maxY = canvasHalfHeight - elementSize.y * (1 - m_rectTransform.pivot.y) - canvasHalfHeight;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("OnPointerEnter");
            Core.Instance.SetDragCursor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("OnPointerExit");
            Core.Instance.SetDefaultCursor();
        }
    }
}