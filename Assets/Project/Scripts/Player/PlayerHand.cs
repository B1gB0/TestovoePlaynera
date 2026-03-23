using System;
using System.Collections.Generic;
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
        [SerializeField] private Image _makeupIcon;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private List<Sprite> _makeupSprites;
        [SerializeField] private RectTransform _handRect;
        [SerializeField] private Animator _handAnimator;
        [SerializeField] private float _moveSpeed = 800f;

        private FaceZone _faceZone;
        private Canvas _canvas;
        
        private Vector2 _defaultAnchoredPos;
        private Vector2 _waitAnchoredPos;
        private Vector2 _additionalMakeupItemPosition;

        private bool _canDrag;
        private bool _isApplying;
        
        private MakeupItem _currentItem;
        private RectTransform _currentRectTransformItem;
        private RectTransform _parent;

        private Vector2 _dragOffset;

        public bool IsBusy { get; private set; }

        public void Construct(
            Canvas canvas,
            Transform defaultPosition,
            Transform waitPosition,
            FaceZone faceZone,
            Transform makeupItemPosition)
        {
            _canvas = canvas;
            _faceZone = faceZone;
            
            _parent = _handRect.parent as RectTransform;

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
                _handRect.parent as RectTransform, eventData.position, _canvas.worldCamera, out _dragOffset);
            _dragOffset = _handRect.anchoredPosition - _dragOffset;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag || _isApplying) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _handRect.parent as RectTransform, eventData.position, _canvas.worldCamera, out Vector2 localPoint);
            _handRect.anchoredPosition = localPoint + _dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_canDrag || _isApplying) return;
            if (_faceZone.IsPointerOverZone(eventData.position))
            {
                ApplyMakeupAsync().Forget();
            }
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
                SetMakeupItem();
                await MoveToAsync(_waitAnchoredPos);
            }
            else if(_currentItem.Type != MakeupItemType.Loofah)
            {
                await MoveToAsync(_additionalMakeupItemPosition);
                SetMakeupItem();
                await MoveToAsync(targetPos);
                await MoveToAsync(_waitAnchoredPos);
            }
            
            _canDrag = true;
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
            await UniTask.Delay(TimeSpan.FromSeconds(_handAnimator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false);
            _currentItem.ApplyEffect();
            await ReturnItemAsync();
        }

        private async UniTask ReturnItemAsync()
        {
            IsBusy = false;
            _canDrag = false;
            _isApplying = false;
            _handAnimator.SetTrigger("Return");

            Vector2 targetPos = WorldToAnchored(_currentRectTransformItem.position);
            await MoveToAsync(targetPos);
            
            _makeupIcon.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
            _currentItem.OnReturn();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            await MoveToAsync(_defaultAnchoredPos);
            _currentItem = null;
            _canDrag = false;
        }

        private void SetMakeupItem()
        {
            _currentItem.gameObject.SetActive(false);
            _makeupIcon.gameObject.SetActive(true);
            
            switch (_currentItem.Type)
            {
                case MakeupItemType.Cream:
                    _makeupIcon.sprite = _makeupSprites[0];
                    _text.gameObject.SetActive(true);
                    break;
                case MakeupItemType.Blush:
                    _makeupIcon.sprite = _makeupSprites[1];
                    _text.gameObject.SetActive(false);
                    break;
                case MakeupItemType.Eyeshadow:
                    _makeupIcon.sprite = _makeupSprites[2];
                    _text.gameObject.SetActive(false);
                    break;
                case MakeupItemType.Lipstick:
                    _makeupIcon.sprite = _makeupSprites[3];
                    _text.gameObject.SetActive(false);
                    break;
            }
        }
    }
}