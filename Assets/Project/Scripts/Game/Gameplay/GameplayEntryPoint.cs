using Cysharp.Threading.Tasks;
using Project.Scripts.Game.Gameplay.Root.View;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Makeup;
using Project.Scripts.Player;
using Project.Scripts.UI.View;
using R3;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
        [SerializeField] private Character _girl;
        [SerializeField] private ViewFactory _viewFactory;

        private PlayerHand _hand;
        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        private GameplayExitParameters _exitParameters;

        public async UniTask<Observable<GameplayExitParameters>> Run(
            UIRootView uiRoot,
            GameplayEnterParameters enterParameters = null)
        {
            _container = gameObject.scene.GetSceneContainer();

            _uiRoot = uiRoot;

            _uiScene = Instantiate(_sceneUIRootPrefab);

            _viewFactory.GetUIRootAndUIScene(uiRoot, _uiScene, _container);


            uiRoot.AttachSceneUI(_uiScene.gameObject);
            
            var container = gameObject.scene.GetSceneContainer();
            GameObjectInjector.InjectRecursive(uiRoot.gameObject, container);

            _uiScene.GetUIStateMachine(uiRoot.UIStateMachine);

            GameplayView gameplayView = await _viewFactory.CreateGameplayView();
            
            _hand = await _viewFactory.CreatePlayerHand();
            _hand.Construct(
                _uiRoot.Canvas,
                _uiScene.DefaultPosition,
                _uiScene.WaitPosition,
                gameplayView.FaceZone,
                _uiScene.AdditionalMakeupPosition,
                gameplayView.Blush,
                gameplayView.Eyeshadow);
            
            gameplayView.InitMakeupItems(_hand, _girl);

            var exitSceneSignalSubject = new Subject<Unit>();
            _uiScene.Bind(exitSceneSignalSubject);

            var exitToSceneSignal = exitSceneSignalSubject.Select(_ => _exitParameters);

            return exitToSceneSignal;
        }
    }
}