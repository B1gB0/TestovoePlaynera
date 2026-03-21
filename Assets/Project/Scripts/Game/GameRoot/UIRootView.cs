using Cysharp.Threading.Tasks;
using Project.Scripts.Audio.Sounds;
using Project.Scripts.Services;
using Project.Scripts.UI.Panel;
using Project.Scripts.UI.StateMachine;
using Project.Scripts.UI.StateMachine.States;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.Game.GameRoot
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private UISceneContainer _uiSceneContainer;

        [SerializeField] private LoadingPanel _loadingPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [SerializeField] private Button _settingsButton;

        private AudioSoundsService _audioSoundsService;
        private IPauseService _pauseService;

        public UIStateMachine UIStateMachine { get; private set; }

        [Inject]
        private void Construct(AudioSoundsService audioSoundsService, IPauseService pauseService)
        {
            _audioSoundsService = audioSoundsService;
            _pauseService = pauseService;
        }

        private void Awake()
        {
            UIStateMachine = new UIStateMachine();
            UIStateMachine.AddState(new LoadingPanelState(_loadingPanel));
        }

        private void OnEnable()
        {
            _settingsPanel.OnBackToSceneButtonPressed += ShowUIScene;
            _settingsButton.onClick.AddListener(StopGame);
            _settingsButton.onClick.AddListener(_settingsPanel.Show);
        }

        private void OnDisable()
        {
            _settingsPanel.OnBackToSceneButtonPressed -= ShowUIScene;
            _settingsButton.onClick.RemoveListener(StopGame);
            _settingsButton.onClick.RemoveListener(_settingsPanel.Show);
        }

        public void ShowLoadingProgress(float progress)
        {
            _loadingPanel.SetProgressText(progress);
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_uiSceneContainer.transform, false);
        }

        private void StopGame()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();
            
            if (SceneManager.GetActiveScene().name == Scenes.MainMenu)
                return;

            _pauseService.OnStopGameWithoutMusic();
        }

        private void ShowUIScene()
        {
            _audioSoundsService.PlaySound(SoundsType.UIButtonClick).Forget();

            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.MainMenu)
                UIStateMachine.EnterIn<MainMenuState>();

            if (sceneName != Scenes.MainMenu)
                UIStateMachine.EnterIn<GameplayState>();
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.transform.GetChild(i).gameObject);
            }
        }
    }
}