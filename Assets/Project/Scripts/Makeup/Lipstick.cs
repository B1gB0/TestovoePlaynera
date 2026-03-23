using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class Lipstick : MakeupItem
    {
        [SerializeField] private Sprite _sprite;
        
        public override void ApplyEffect()
        {
            Character.ApplyLipstick(_sprite);
        }
    }
}