using Cysharp.Threading.Tasks;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Makeup
{
    public abstract class MakeupItem : MonoBehaviour, IPointerClickHandler
    {
        protected Character Character;
        
        private PlayerHand _hand;

        [field: SerializeField] public MakeupItemType Type { get; private set; }
        public Sprite MakeupSprite { get; protected set; }
        public Sprite ItemSprite { get; protected set; }

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

        public virtual void OnTakeItem()
        {
            gameObject.SetActive(false);
        }

        public abstract void ApplyEffect();
    }
}