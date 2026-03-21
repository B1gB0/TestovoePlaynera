using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Services;
using Project.Scripts.UI.StateMachine;
using Project.Scripts.UI.StateMachine.States;
using R3;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Game.MainMenu.Root.View
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        [SerializeField] private MainMenuElements _uiScene;

        [SerializeField] private Button _playButton;

        private Subject<Unit> _exitSceneSubjectSignal;
        private AudioSoundsService _audioSoundsService;
        private UIStateMachine _uiStateMachine;

        public event Action OnGameplayStarted;

        [Inject]
        public void Construct(AudioSoundsService audioSoundsService)
        {
            _audioSoundsService = audioSoundsService;
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(HandleGoToGameplayButtonClick);
        }
        
        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(HandleGoToGameplayButtonClick);
        }

        private void OnDestroy()
        {
            _uiStateMachine.RemoveState<MainMenuState>();
        }

        public void GetUIStateMachineAndStates(UIStateMachine uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;

            _uiStateMachine.AddState(new MainMenuState(_uiScene));

            _uiStateMachine.EnterIn<MainMenuState>();
        }

        public void Bind(Subject<Unit> exitSceneSignalSubject)
        {
            _exitSceneSubjectSignal = exitSceneSignalSubject;
        }

        private void HandleGoToGameplayButtonClick()
        {
            _audioSoundsService.PlaySound(SoundsType.PaperButton).Forget();
            OnGameplayStarted?.Invoke();
            _exitSceneSubjectSignal?.OnNext(Unit.Default);
        }
    }
}