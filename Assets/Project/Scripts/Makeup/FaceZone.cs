using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class FaceZone : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        
            if (_canvas == null)
                Debug.LogError("FaceZone must be inside a Canvas!");
        }

        public bool IsPointerOverZone(Vector2 screenPos)
        {
            if (_rectTransform == null) return false;
            return RectTransformUtility.RectangleContainsScreenPoint(
                _rectTransform,
                screenPos,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera);
        }
    }
}