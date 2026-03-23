using Cysharp.Threading.Tasks;
using Project.Scripts.Game.Gameplay.Root.View;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Player;
using Project.Scripts.Services;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Project.Scripts.UI.View
{
    public class ViewFactory : MonoBehaviour
    {
        private const string GameplayView = nameof(GameplayView);
        private const string PlayerHand = nameof(PlayerHand);
        
        private IResourceService _resourceService;

        private UIRootView _uiRoot;
        private UIGameplayRootBinder _uiScene;
        private Container _container;
        
        [Inject]
        public void Construct(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }
        
        public void GetUIRootAndUIScene(UIRootView uiRoot, UIGameplayRootBinder uiScene, Container container)
        {
            _uiRoot = uiRoot;
            _uiScene = uiScene;
            _container = container;

            GameObjectInjector.InjectRecursive(_uiScene.gameObject, _container);
        }

        public async UniTask<GameplayView> CreateGameplayView()
        {
            var gameplayViewTemplate = await _resourceService.Load<GameObject>(GameplayView);
            gameplayViewTemplate = Instantiate(gameplayViewTemplate);

            GameplayView gameplayView = gameplayViewTemplate.GetComponent<GameplayView>();
            GameObjectInjector.InjectObject(gameplayView.gameObject, _container);
            gameplayView.transform.SetParent(_uiScene.transform, false);

            return gameplayView;
        }

        public async UniTask<PlayerHand> CreatePlayerHand()
        {
            var playerHandTemplate = await _resourceService.Load<GameObject>(PlayerHand);
            playerHandTemplate = Instantiate(playerHandTemplate);

            PlayerHand playerHand = playerHandTemplate.GetComponent<PlayerHand>();
            GameObjectInjector.InjectObject(playerHand.gameObject, _container);
            playerHand.transform.SetParent(_uiScene.transform, false);

            return playerHand;
        }
    }
}