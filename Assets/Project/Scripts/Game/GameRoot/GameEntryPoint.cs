using Cysharp.Threading.Tasks;
using Project.Scripts.Game.Gameplay;
using Project.Scripts.Game.MainMenu;
using Project.Scripts.Services;
using Project.Scripts.UI.StateMachine.States;
using R3;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using YG;

namespace Project.Scripts.Game.GameRoot
{
    public class GameEntryPoint : MonoBehaviour
    {
        private const float TargetValue = 1f;
        private const float MinValue = 0f;
        private const float SpeedLoadingScene = 5f;
        private const float SpeedFinalLoadingScene = 0.5f;
        private const float MinLoadTime = 2.0f;
        private const float ActivationThreshold = 0.9f;

        private const int DelayOfTransition = 100;

        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        private bool _isLoadingScene;

        private UIRootView _uiRoot;
        private IPauseService _pauseService;

        [Inject]
        private void Construct(
            UIRootView uiRoot,
            IPauseService pauseService)
        {
            _uiRoot = uiRoot;
            _pauseService = pauseService;

            EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
            _pauseService.GetEventSystem(eventSystem);
        }

        private async void Start()
        {
            await Addressables.InitializeAsync().Task;

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            await StartGame();

            _pauseService.OnPlayGame();

            YG2.onShowWindowGame += _pauseService.OnPlayGame;
            YG2.onHideWindowGame += _pauseService.OnStopGameWithMusic;
            YG2.onOpenAnyAdv += _pauseService.OnStopGameWithMusic;
            YG2.onOpenAnyAdv += _pauseService.DisableEventSystem;
            YG2.onCloseAnyAdv += _pauseService.EnableEventSystem;
            YG2.onCloseInterAdv += _pauseService.OnPlayGame;
        }

        private void OnDestroy()
        {
            YG2.onShowWindowGame -= _pauseService.OnPlayGame;
            YG2.onHideWindowGame -= _pauseService.OnStopGameWithMusic;
            YG2.onOpenAnyAdv -= _pauseService.OnStopGameWithMusic;
            YG2.onOpenAnyAdv -= _pauseService.DisableEventSystem;
            YG2.onCloseAnyAdv -= _pauseService.EnableEventSystem;
            YG2.onCloseInterAdv -= _pauseService.OnPlayGame;
        }

        private async UniTask StartGame()
        {
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName != Scenes.Boot)
            {
                InitiateSceneScope();

                if (sceneName == Scenes.MainMenu)
                {
                    var sceneEntryPoint = FindFirstObjectByType<MainMenuEntryPoint>();
                    sceneEntryPoint.Run(_uiRoot, null).Subscribe(mainMenuExitParameters =>
                    {
                        LoadAndStartGameplay(mainMenuExitParameters
                            .TargetSceneEnterParameters.As<GameplayEnterParameters>()).Forget();
                    });
                }
                else
                {
                    var sceneEntryPoint = FindFirstObjectByType<GameplayEntryPoint>();
                    var observable = await sceneEntryPoint.Run(_uiRoot, null);

                    var exitParameters = await observable.FirstAsync();
                    await HandleExitGameplayScene(exitParameters);
                }
                return;
            }
#endif
            _uiRoot.UIStateMachine.EnterIn<LoadingPanelState>();
            
            await LoadAndStartMainMenu();
        }

        private async UniTask LoadAndStartMainMenu(MainMenuEnterParameters enterParameters = null)
        {
            _uiRoot.UIStateMachine.EnterIn<LoadingPanelState>();

            await LoadScene(Scenes.MainMenu);

            var sceneEntryPoint = FindFirstObjectByType<MainMenuEntryPoint>();
            sceneEntryPoint.Run(_uiRoot, enterParameters).Subscribe(mainMenuExitParameters =>
            {
                LoadAndStartGameplay(mainMenuExitParameters
                    .TargetSceneEnterParameters.As<GameplayEnterParameters>()).Forget();
            });
        }

        private async UniTask LoadAndStartGameplay(GameplayEnterParameters enterParameters)
        {
            _uiRoot.UIStateMachine.EnterIn<LoadingPanelState>();

            await LoadScene(enterParameters.SceneName);

            var sceneEntryPoint = FindFirstObjectByType<GameplayEntryPoint>();
            var observable = await sceneEntryPoint.Run(_uiRoot, enterParameters);

            var exitParameters = await observable.FirstAsync();
            await HandleExitGameplayScene(exitParameters);
        }

        private async UniTask<GameplayExitParameters> HandleExitGameplayScene(
            GameplayExitParameters gameplayExitParameters)
        {
            YG2.InterstitialAdvShow();

            var targetSceneName = gameplayExitParameters.TargetSceneEnterParameters.SceneName;

            if (targetSceneName == Scenes.MainMenu)
            {
                await LoadAndStartMainMenu(gameplayExitParameters
                    .TargetSceneEnterParameters.As<MainMenuEnterParameters>());
            }
            else
            {
                await LoadAndStartGameplay(gameplayExitParameters
                    .TargetSceneEnterParameters.As<GameplayEnterParameters>());
            }

            return gameplayExitParameters;
        }

        private async UniTask LoadScene(string sceneName)
        {
            if (_isLoadingScene) 
                return;
            
            _isLoadingScene = true;

            try
            {
                var newSceneHandle = Addressables.LoadSceneAsync(
                    sceneName,
                    LoadSceneMode.Single,
                    false);

                await newSceneHandle.Task;

                if (sceneName != Scenes.Boot)
                {
                    await SimulateLoadingProgress(newSceneHandle);
                }

                await UniTask.Yield();

                var activateOp = newSceneHandle.Result.ActivateAsync();
                await activateOp;

                if (_sceneHandle.IsValid())
                {
                    await UniTask.Delay(DelayOfTransition);
                    Addressables.Release(_sceneHandle);
                }

                _sceneHandle = newSceneHandle;
                
                InitiateSceneScope();
            }
            finally
            {
                _isLoadingScene = false;
            }
        }

        private void InitiateSceneScope()
        {
            ContainerScope sceneScope = FindAnyObjectByType<ContainerScope>();

            if (sceneScope != null)
            {
                ContainerScope.OnSceneContainerBuilding += OnPreInstallScene;
            }
        }

        private void OnPreInstallScene(Scene scene, ContainerBuilder sceneContainerBuilder)
        {
            sceneContainerBuilder.RegisterValue("Container");
        }

        private async UniTask SimulateLoadingProgress(AsyncOperationHandle<SceneInstance> sceneHandle)
        {
            float timer = MinValue;
            float fakeProgress = MinValue;

            while (fakeProgress < ActivationThreshold)
            {
                timer += Time.deltaTime;

                float realProgress = sceneHandle.PercentComplete;

                fakeProgress = Mathf.Lerp(fakeProgress, realProgress, Time.deltaTime * SpeedLoadingScene);
                fakeProgress = Mathf.Clamp01(Mathf.Max(fakeProgress, timer / MinLoadTime));
                fakeProgress = Mathf.Min(fakeProgress, ActivationThreshold);

                _uiRoot.ShowLoadingProgress(fakeProgress);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            fakeProgress = ActivationThreshold;

            while (fakeProgress < TargetValue)
            {
                fakeProgress = Mathf.MoveTowards(
                    fakeProgress,
                    TargetValue,
                    Time.deltaTime * SpeedFinalLoadingScene);

                _uiRoot.ShowLoadingProgress(fakeProgress);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            _uiRoot.ShowLoadingProgress(TargetValue);
        }
    }
}