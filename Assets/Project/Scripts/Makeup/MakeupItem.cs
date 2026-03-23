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
            // Если рука уже занята, не реагируем
            if (_hand.IsBusy) return;
            _hand.StartTakingItem(this).Forget();
        }

        protected virtual void OnTaken()
        {
            gameObject.SetActive(false); // предмет исчезает (в руке он появится)
        }

        public void Construct(PlayerHand hand, Character character)
        {
            _hand = hand;
            Character = character;
        }

        public virtual void OnReturn()
        {
            gameObject.SetActive(true); // предмет возвращается на место
        }

        public abstract void ApplyEffect(); // нанесение эффекта на персонажа
    }
}