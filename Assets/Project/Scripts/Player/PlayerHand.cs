using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float _moveSpeed = 800f; // скорость перемещения (пикселей/сек)

        private FaceZone _faceZone;
        private Canvas _canvas;
        private Vector2 _defaultAnchoredPos;   // исходная позиция руки
        private Vector2 _waitAnchoredPos;      // позиция ожидания (у груди)

        private bool _canDrag;
        private bool _isApplying;
        private MakeupItem _currentItem;
        private Vector2 _dragOffset;
        private Coroutine _moveCoroutine;

        public bool IsBusy { get; private set; }

        public void Construct(Canvas canvas, Transform defaultPosition, Transform waitPosition, FaceZone faceZone)
        {
            _canvas = canvas;
            _faceZone = faceZone;
            RectTransform parent = _handRect.parent as RectTransform;

            // Преобразуем мировые позиции в anchoredPosition внутри Canvas
            _defaultAnchoredPos = WorldToAnchored(defaultPosition.position);
            _waitAnchoredPos = WorldToAnchored(waitPosition.position);
        }

        private void Update()
        {
            Debug.Log(_canDrag);
        }

        private Vector2 WorldToAnchored(Vector3 worldPos)
        {
            RectTransform parent = _handRect.parent as RectTransform;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, _canvas.worldCamera, out Vector2 localPoint);
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
                StartCoroutine(ApplyMakeup());
            }
        }

        public void StartTakingItem(MakeupItem item)
        {
            if (IsBusy) return;

            _currentItem = item;
            IsBusy = true;
            _canDrag = false;

            // Получаем позицию предмета на Canvas (предполагается, что предмет — UI с RectTransform)
            RectTransform itemRect = item.GetComponent<RectTransform>();
            if (itemRect == null)
            {
                Debug.LogError("MakeupItem must have RectTransform to move hand to it!");
                return;
            }

            // Конвертируем позицию предмета в anchoredPosition относительно родителя руки
            Vector2 targetPos = WorldToAnchored(itemRect.position);
            _moveCoroutine = StartCoroutine(MoveTo(targetPos, OnReachedItem));
        }

        private IEnumerator MoveTo(Vector2 targetPos, Action onComplete)
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
                yield return null;
            }
            _handRect.anchoredPosition = targetPos;
            onComplete?.Invoke();
        }

        private void OnReachedItem()
        {
            // Предмет скрывается
            //_currentItem.OnTaken(); 
            
            _currentItem.gameObject.SetActive(false);
            _makeupIcon.gameObject.SetActive(true);
            if (_currentItem.Type == MakeupItemType.Cream)
            {
                _makeupIcon.sprite = _makeupSprites[0];
                _text.gameObject.SetActive(true);
            }
            else if (_currentItem.Type == MakeupItemType.Blush)
            {
                _makeupIcon.sprite = _makeupSprites[1];
                _text.gameObject.SetActive(false);
            }
            else if (_currentItem.Type == MakeupItemType.Eyeshadow)
            {
                _makeupIcon.sprite = _makeupSprites[2];
                _text.gameObject.SetActive(false);
            }
            else if (_currentItem.Type == MakeupItemType.Lipstick)
            {
                _makeupIcon.sprite = _makeupSprites[3];
                _text.gameObject.SetActive(false);
            }

            // Перемещаем руку в позицию ожидания
            StartCoroutine(MoveTo(_waitAnchoredPos, () =>
            {
                _canDrag = true;
            }));
        }

        private IEnumerator ApplyMakeup()
        {
            _canDrag = false;
            _isApplying = true;
            _handAnimator.SetTrigger("Apply");
            yield return new WaitForSeconds(_handAnimator.GetCurrentAnimatorStateInfo(0).length);
            _currentItem.ApplyEffect();
            ReturnItem();
        }

        private void ReturnItem()
        {
            IsBusy = false;
            StartCoroutine(ReturnCoroutine());
        }

        private IEnumerator ReturnCoroutine()
        {
            _canDrag = false;
            _isApplying = false;
            _handAnimator.SetTrigger("Return");
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(MoveTo(_defaultAnchoredPos, () =>
            {
                _currentItem?.OnReturn(); // предмет снова появляется на месте
                _currentItem = null;
                _canDrag = false;
                _makeupIcon.gameObject.SetActive(false);
                _text.gameObject.SetActive(false);
            }));
        }
    }
}