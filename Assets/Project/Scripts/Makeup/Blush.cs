using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class Blush : MakeupItem
    {
        [SerializeField] private Sprite _sprite;
        
        public override void ApplyEffect()
        {
            Character.ApplyBlushes(_sprite);
        }
    }
}