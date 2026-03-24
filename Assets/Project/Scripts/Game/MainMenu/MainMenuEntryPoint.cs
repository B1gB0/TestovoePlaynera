using Project.Scripts.Audio.Sounds;
using Project.Scripts.Game.Gameplay;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Game.MainMenu.Root.View;
using Project.Scripts.Services;
using R3;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Project.Scripts.Game.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        private UIMainMenuRootBinder _uiScene;
        private MainMenuExitParameters _exitParameters;

        private AudioSoundsService _audioSoundsService;
        private ITweenAnimationService _tweenAnimationService;

        [Inject]
        private void Construct(AudioSoundsService audioSoundsService, ITweenAnimationService tweenAnimationService)
        {
            _audioSoundsService = audioSoundsService;
            _tweenAnimationService = tweenAnimationService;
        }

        private async void Start()
        {
            await _audioSoundsService.Init();
            await _tweenAnimationService.Init();
            
            _audioSoundsService.PlayMusic(SoundsType.Music);
        }

        public Observable<MainMenuExitParameters> Run(UIRootView uiRoot, MainMenuEnterParameters enterParameters)
        {
            _uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(_uiScene.gameObject);

            _uiScene.OnGameplayStarted += GetMainMenuExitParameters;

            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, container);

            _uiScene.GetUIStateMachineAndStates(uiRoot.UIStateMachine);

            var exitSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSignalSubject);

            var exitToGameplaySceneSignal = exitSignalSubject.Select(_ => _exitParameters);

            return exitToGameplaySceneSignal;
        }

        private void GetMainMenuExitParameters()
        {
            var sceneName = Scenes.Gameplay;

            var gameplayEnterParameters = new GameplayEnterParameters(sceneName);

            _exitParameters = new MainMenuExitParameters(gameplayEnterParameters);
        }
    }
}