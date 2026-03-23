using UnityEngine;

namespace Project.Scripts.Makeup
{
    public class Eyeshadow : MakeupItem
    {
        [SerializeField] private Sprite _sprite;
        
        public override void ApplyEffect()
        {
            Character.ApplyEyeshadow(_sprite);
        }
    }
}