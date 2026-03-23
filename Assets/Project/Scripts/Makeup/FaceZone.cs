using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class FaceZone : MonoBehaviour
    {
        [SerializeField] private Collider2D _zoneCollider;

        public bool IsPointerOverZone(Vector2 screenPos)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
            return _zoneCollider.OverlapPoint(worldPoint);
        }
    }
}