using UnityEngine;

namespace Project.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _faceWithAcne;
        [SerializeField] private SpriteRenderer _eyeshadowRenderer;
        [SerializeField] private SpriteRenderer _lipsRenderer;   
        [SerializeField] private SpriteRenderer _blushRenderer;   

        public void RemoveAcne()
        {
            _faceWithAcne.gameObject.SetActive(false);
        }

        public void ApplyBlushes(Sprite sprite)
        {
            _blushRenderer.sprite = sprite;
            _blushRenderer.gameObject.SetActive(true);
        }

        public void ApplyEyeshadow(Sprite sprite)
        {
            _eyeshadowRenderer.sprite = sprite;
            _eyeshadowRenderer.gameObject.SetActive(true);
        }

        public void ApplyLipstick(Sprite sprite)
        {
            _lipsRenderer.sprite = sprite;
            _lipsRenderer.gameObject.SetActive(true);
        }

        public void ResetMakeup()
        {
            _faceWithAcne.gameObject.SetActive(true);
            _eyeshadowRenderer.gameObject.SetActive(false);
            _lipsRenderer.gameObject.SetActive(false);
            _blushRenderer.gameObject.SetActive(false);
        }
    }
}