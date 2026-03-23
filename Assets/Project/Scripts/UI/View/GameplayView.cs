using System.Collections.Generic;
using Project.Scripts.Makeup;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.View
{
    public class GameplayView : MonoBehaviour, IView
    {
        [SerializeField] private List<Sprite> _blusheColors;
        [SerializeField] private List<Sprite> _lipstickColors;
        [SerializeField] private List<Sprite> _eyeshadowColors;

        [SerializeField] private List<Lipstick> _lipsticks;
        [SerializeField] private List<Sprite> _lipsticksMakeup;

        [SerializeField] private List<Blush> _blushes;
        [SerializeField] private List<Sprite> _blushesMakeup;
        [SerializeField] private Image _blush;

        [SerializeField] private List<Eyeshadow> _eyeshadows;
        [SerializeField] private List<Sprite> _eyeshadowsMakeup;
        [SerializeField] private Image _eyeshadow;

        [SerializeField] private Cream _cream;
        [SerializeField] private Loofah _loofah;
        [SerializeField] private FaceZone _faceZone;

        [SerializeField] private Button _blushButton;
        [SerializeField] private Button _lipstickButton;
        [SerializeField] private Button _eyeshadowButton;

        [SerializeField] private Button _blushButtonActive;
        [SerializeField] private Button _lipstickButtonActive;
        [SerializeField] private Button _eyeshadowButtonActive;

        public FaceZone FaceZone => _faceZone;

        private void OnEnable()
        {
            _blushButton.onClick.AddListener(OnSetBlushes);
            _lipstickButton.onClick.AddListener(OnSetLipsticks);
            _eyeshadowButton.onClick.AddListener(OnSetEyeshadows);
        }

        private void OnDisable()
        {
            _blushButton.onClick.RemoveListener(OnSetBlushes);
            _lipstickButton.onClick.RemoveListener(OnSetLipsticks);
            _eyeshadowButton.onClick.RemoveListener(OnSetEyeshadows);
        }

        public void InitMakeupItems(PlayerHand hand, Character character)
        {
            _cream.Construct(hand, character);
            _loofah.Construct(hand, character);

            foreach (var lipstick in _lipsticks)
            {
                lipstick.Construct(hand, character);
            }

            foreach (var blush in _blushes)
            {
                blush.Construct(hand, character);
            }

            foreach (var eyeshadow in _eyeshadows)
            {
                eyeshadow.Construct(hand, character);
            }
            
            OnSetBlushes();
        }

        private void ResetAllButtons()
        {
            _blushButton.gameObject.SetActive(true);
            _blushButtonActive.gameObject.SetActive(false);
            _blush.gameObject.SetActive(false);

            _lipstickButton.gameObject.SetActive(true);
            _lipstickButtonActive.gameObject.SetActive(false);

            _eyeshadowButton.gameObject.SetActive(true);
            _eyeshadowButtonActive.gameObject.SetActive(false);
            _eyeshadow.gameObject.SetActive(false);

            foreach (var lipstick in _lipsticks)
            {
                lipstick.gameObject.SetActive(false);
            }
            
            foreach (var eyeshadow in _eyeshadows)
            {
                eyeshadow.gameObject.SetActive(false);
            }
            
            foreach (var blush in _blushes)
            {
                blush.gameObject.SetActive(false);
            }
        }

        private void OnSetBlushes()
        {
            ResetAllButtons();
            SetBlushes();

            _blush.gameObject.SetActive(true);
            _blushButtonActive.gameObject.SetActive(true);
            _blushButton.gameObject.SetActive(false);
        }

        private void OnSetLipsticks()
        {
            ResetAllButtons();
            SetLipsticks();

            _lipstickButtonActive.gameObject.SetActive(true);
            _lipstickButton.gameObject.SetActive(false);
        }

        private void OnSetEyeshadows()
        {
            ResetAllButtons();
            SetEyeshadows();

            _eyeshadow.gameObject.SetActive(true);
            _eyeshadowButtonActive.gameObject.SetActive(true);
            _eyeshadowButton.gameObject.SetActive(false);
        }

        private void SetLipsticks()
        {
            for (int i = 0; i < _lipsticks.Count; i++)
            {
                if (i < _lipstickColors.Count)
                {
                    _lipsticks[i].Image.sprite = _lipstickColors[i];
                    _lipsticks[i].gameObject.SetActive(true);
                    _lipsticks[i].Image.preserveAspect = true;
                    _lipsticks[i].GetSprite(_lipsticksMakeup[i]);
                }
                else
                {
                    _lipsticks[i].Image.sprite = _lipstickColors[i];
                    _lipsticks[i].gameObject.SetActive(true);
                }
            }
        }

        private void SetBlushes()
        {
            for (int i = 0; i < _blushes.Count; i++)
            {
                if (i < _blusheColors.Count)
                {
                    _blushes[i].Image.sprite = _blusheColors[i];
                    _blushes[i].gameObject.SetActive(true);
                    _blushes[i].Image.preserveAspect = true;
                    _blushes[i].GetSprite(_blushesMakeup[i]);
                }
                else
                {
                    _blushes[i].Image.sprite = _blusheColors[i];
                    _blushes[i].gameObject.SetActive(true);
                }
            }
        }

        private void SetEyeshadows()
        {
            for (int i = 0; i < _eyeshadows.Count; i++)
            {
                if (i < _eyeshadowColors.Count)
                {
                    _eyeshadows[i].Image.sprite = _eyeshadowColors[i];
                    _eyeshadows[i].gameObject.SetActive(true);
                    _eyeshadows[i].Image.preserveAspect = true;
                    _eyeshadows[i].GetSprite(_eyeshadowsMakeup[i]);
                }
                else
                {
                    _eyeshadows[i].Image.sprite = _eyeshadowColors[i];
                    _eyeshadows[i].gameObject.SetActive(true);
                }
            }
        }
    }
}