using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.View
{
    public class GameplayView : MonoBehaviour, IView
    {
        [SerializeField] private List<Sprite> _blushes;
        [SerializeField] private List<Sprite> _lipsticks;
        [SerializeField] private List<Sprite> _eyeshadows;
        
        [SerializeField] private List<Image> _items;

        [SerializeField] private Button _blushButton;
        [SerializeField] private Button _lipstickButton;
        [SerializeField] private Button _eyeshadowButton;
        
        [SerializeField] private Button _blushButtonActive;
        [SerializeField] private Button _lipstickButtonActive;
        [SerializeField] private Button _eyeshadowButtonActive;

        private void Start()
        {
            OnSetBlushes();
        }

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
        
        private void ResetAllButtons()
        {
            _blushButton.gameObject.SetActive(true);
            _blushButtonActive.gameObject.SetActive(false);
            
            _lipstickButton.gameObject.SetActive(true);
            _lipstickButtonActive.gameObject.SetActive(false);
            
            _eyeshadowButton.gameObject.SetActive(true);
            _eyeshadowButtonActive.gameObject.SetActive(false);
        }

        private void OnSetBlushes()
        {
            SetItems(_blushes);
            ResetAllButtons();
            
            _blushButtonActive.gameObject.SetActive(true);
            _blushButton.gameObject.SetActive(false);
        }

        private void OnSetLipsticks()
        {
            SetItems(_lipsticks);
            ResetAllButtons();
            
            _lipstickButtonActive.gameObject.SetActive(true);
            _lipstickButton.gameObject.SetActive(false);
        }

        private void OnSetEyeshadows()
        {
            SetItems(_eyeshadows);
            ResetAllButtons();
            
            _eyeshadowButtonActive.gameObject.SetActive(true);
            _eyeshadowButton.gameObject.SetActive(false);
        }

        private void SetItems(List<Sprite> sprites)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (i < sprites.Count)
                {
                    _items[i].sprite = sprites[i];
                    _items[i].gameObject.SetActive(true);
                    _items[i].preserveAspect = true;
                }
                else
                {
                    _items[i].sprite = null;
                    _items[i].gameObject.SetActive(false);
                }
            }
        }
    }
}