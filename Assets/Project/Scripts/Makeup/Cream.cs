using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class Cream : MakeupItem
    {
        [SerializeField] private Sprite _itemSprite;
        
        public override void ApplyEffect()
        {
            Character.RemoveAcne();
        }
        
        public void GetSprite()
        {
            ItemSprite = _itemSprite;
        }
    }
}