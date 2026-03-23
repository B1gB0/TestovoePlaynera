using UnityEngine.EventSystems;

namespace Project.Scripts.Makeup
{
    public class Loofah : MakeupItem
    {
        public override void ApplyEffect()
        {
            Character.ResetMakeup();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            ApplyEffect();
        }

        public override void OnReturn() { }
    }
}