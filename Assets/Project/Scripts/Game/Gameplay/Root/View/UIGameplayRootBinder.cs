using Project.Scripts.UI.StateMachine;
using Project.Scripts.UI.StateMachine.States;
using R3;
using UnityEngine;

namespace Project.Scripts.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubject;
        private UIStateMachine _uiStateMachine;
        
        [field: SerializeField] public Transform DefaultPosition { get; private set; }
        [field: SerializeField] public Transform LipsPosition { get; private set; }
        [field: SerializeField] public Transform EyePosition { get; private set; }
        [field: SerializeField] public Transform BlushPosition { get; private set; }
        [field: SerializeField] public Transform WaitPosition { get; private set; }
        [field: SerializeField] public Transform AdditionalMakeupPosition { get; private set; }
        [field: SerializeField] public GameplayElements UIScene { get; private set; }
        
        public void GetUIStateMachine(UIStateMachine uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;
            _uiStateMachine.RemoveState<GameplayState>();
            _uiStateMachine.AddState(new GameplayState(UIScene));
            _uiStateMachine.EnterIn<GameplayState>();
        }
        
        public void Bind(Subject<Unit> exitSceneSignalSubject)
        {
            _exitSceneSignalSubject = exitSceneSignalSubject;
        }
        
        public void HandleGoToNextSceneButtonClick()
        {
            _exitSceneSignalSubject?.OnNext(Unit.Default);
        }
    }
}