using Cysharp.Threading.Tasks;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Makeup
{
    public abstract class MakeupItem : MonoBehaviour, IPointerClickHandler
    {
        private PlayerHand _hand;
        
        protected Character Character;
        
        [field: SerializeField] public MakeupItemType Type { get; private set; }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (_hand.IsBusy) return;
            _hand.StartTakingItem(this).Forget();
        }

        public void Construct(PlayerHand hand, Character character)
        {
            _hand = hand;
            Character = character;
        }

        public virtual void OnReturn()
        {
            gameObject.SetActive(true);
        }

        public abstract void ApplyEffect();
    }
}