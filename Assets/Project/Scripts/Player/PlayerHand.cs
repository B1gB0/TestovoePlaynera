using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Makeup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.Player
{
    public class PlayerHand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float _lipstickSize = 115f;
        private const float _otherSize = 250f;

        [SerializeField] private Image _playerHandParent;
        [SerializeField] private Image _makeupItemIcon;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _handRect;
        [SerializeField] private Animator _handAnimator;
        [SerializeField] private float _moveSpeed = 800f;

        private FaceZone _faceZone;
        private Canvas _canvas;

        private Vector2 _defaultAnchoredPos;
        private Vector2 _lipsAnchoredPos;
        private Vector2 _eyeAnchoredPos;
        private Vector2 _blushAnchoredPos;
        private Vector2 _waitAnchoredPos;
        private Vector2 _additionalMakeupItemPosition;

        private bool _canDrag;
        private bool _isApplying;

        private MakeupItem _currentItem;
        private RectTransform _currentRectTransformItem;
        private RectTransform _parent;

        private Image _blush;
        private Image _eyeshadow;

        private Vector2 _dragOffset;

        public bool IsBusy { get; private set; }

        public void Construct(
            Canvas canvas,
            Transform defaultPosition,
            Transform waitPosition,
            FaceZone faceZone,
            Transform makeupItemPosition,
            Image blush,
            Image eyeshadow,
            Transform blushPosition,
            Transform eyePosition,
            Transform lipsPosition)
        {
            _canvas = canvas;
            _faceZone = faceZone;
            _eyeshadow = eyeshadow;
            _blush = blush;

            _parent = _handRect.parent as RectTransform;

            _lipsAnchoredPos = WorldToAnchored(lipsPosition.position);
            _blushAnchoredPos = WorldToAnchored(blushPosition.position);
            _eyeAnchoredPos = WorldToAnchored(eyePosition.position);
            _defaultAnchoredPos = WorldToAnchored(defaultPosition.position);
            _waitAnchoredPos = WorldToAnchored(waitPosition.position);
            _additionalMakeupItemPosition = WorldToAnchored(makeupItemPosition.position);
        }

        private Vector2 WorldToAnchored(Vector3 worldPos)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parent,
                screenPoint,
                _canvas.worldCamera,
                out Vector2 localPoint);

            return localPoint;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_canDrag || _isApplying) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _handRect.parent as RectTransform,
                eventData.position,
                _canvas.worldCamera,
                out _dragOffset);

            _dragOffset = _handRect.anchoredPosition - _dragOffset;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag || _isApplying) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _handRect.parent as RectTransform, eventData.position, _canvas.worldCamera, out Vector2 localPoint);
            _handRect.anchoredPosition = localPoint + _dragOffset;
        }

        public async void OnEndDrag(PointerEventData eventData)
        {
            if (!_canDrag || _isApplying) return;
            if (!_faceZone.IsPointerOverZone(eventData.position))
                return;

            switch (_currentItem.Type)
            {
                case MakeupItemType.Cream:
                    await ApplyMakeupAsync();
                    Vector2 targetPos = WorldToAnchored(_currentRectTransformItem.position);
                    await MoveToAsync(targetPos);
                    break;
                case MakeupItemType.Lipstick:
                    await MoveToAsync(_lipsAnchoredPos);
                    await ApplyMakeupAsync();
                    break;
                case MakeupItemType.Blush:
                    await MoveToAsync(_blushAnchoredPos);
                    await ApplyMakeupAsync();
                    await MoveToAsync(_additionalMakeupItemPosition);
                    _blush.gameObject.SetActive(true);
                    break;
                case MakeupItemType.Eyeshadow:
                    await MoveToAsync(_eyeAnchoredPos);
                    await ApplyMakeupAsync();
                    await MoveToAsync(_additionalMakeupItemPosition);
                    _eyeshadow.gameObject.SetActive(true);
                    break;
            }
            
            await ReturnItemAsync();
        }

        public async UniTask StartTakingItem(MakeupItem item)
        {
            if (IsBusy) return;

            _currentItem = item;
            IsBusy = true;
            _canDrag = false;

            _currentRectTransformItem = item.GetComponent<RectTransform>();
            if (_currentRectTransformItem == null)
            {
                Debug.LogError("MakeupItem must have RectTransform to move hand to it!");
                return;
            }

            Vector2 targetPos = WorldToAnchored(_currentRectTransformItem.position);

            if (_currentItem.Type == MakeupItemType.Cream || _currentItem.Type == MakeupItemType.Lipstick)
            {
                await MoveToAsync(targetPos);
                _currentItem.OnTakeItem();
                SetMakeupItem();
                await MoveToAsync(_waitAnchoredPos);
            }
            else if (_currentItem.Type != MakeupItemType.Loofah)
            {
                await MoveToAsync(_additionalMakeupItemPosition);
                SetMakeupItem();

                switch (_currentItem.Type)
                {
                    case MakeupItemType.Blush:
                        _blush.gameObject.SetActive(false);
                        break;
                    case MakeupItemType.Eyeshadow:
                        _eyeshadow.gameObject.SetActive(false);
                        break;
                }

                await MoveToAsync(targetPos);
                await MoveToAsync(_waitAnchoredPos);
            }

            _canDrag = true;
            _playerHandParent.raycastTarget = true;
        }

        private async UniTask MoveToAsync(Vector2 targetPos)
        {
            Vector2 startPos = _handRect.anchoredPosition;
            float distance = Vector2.Distance(startPos, targetPos);
            float duration = distance / _moveSpeed;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _handRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                await UniTask.Yield();
            }

            _handRect.anchoredPosition = targetPos;
        }

        private async UniTask ApplyMakeupAsync()
        {
            _canDrag = false;
            _isApplying = true;
            _handAnimator.SetTrigger("Apply");
            await UniTask.Delay(TimeSpan.FromSeconds(_handAnimator.GetCurrentAnimatorStateInfo(0).length),
                ignoreTimeScale: false);
            _currentItem.ApplyEffect();
        }

        private async UniTask ReturnItemAsync()
        {
            IsBusy = false;
            _canDrag = false;
            _isApplying = false;
            _playerHandParent.raycastTarget = false;
            _handAnimator.SetTrigger("Return");

            _makeupItemIcon.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
            _currentItem.OnReturn();

            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            await MoveToAsync(_defaultAnchoredPos);
            _currentItem = null;
            _canDrag = false;
        }

        private void SetMakeupItem()
        {
            _makeupItemIcon.gameObject.SetActive(true);

            switch (_currentItem.Type)
            {
                case MakeupItemType.Cream:
                    _makeupItemIcon.sprite = _currentItem.ItemSprite;
                    _makeupItemIcon.rectTransform.sizeDelta = new Vector2(_otherSize, _otherSize);
                    _text.gameObject.SetActive(true);
                    break;
                case MakeupItemType.Blush:
                    _makeupItemIcon.sprite = _currentItem.ItemSprite;
                    _makeupItemIcon.rectTransform.sizeDelta = new Vector2(_otherSize, _otherSize);
                    _text.gameObject.SetActive(false);
                    break;
                case MakeupItemType.Eyeshadow:
                    _makeupItemIcon.sprite = _currentItem.ItemSprite;
                    _makeupItemIcon.rectTransform.sizeDelta = new Vector2(_otherSize, _otherSize);
                    _text.gameObject.SetActive(false);
                    break;
                case MakeupItemType.Lipstick:
                    _makeupItemIcon.sprite = _currentItem.ItemSprite;
                    _makeupItemIcon.rectTransform.sizeDelta = new Vector2(_lipstickSize, _lipstickSize);
                    _text.gameObject.SetActive(false);
                    break;
            }
        }
    }
}