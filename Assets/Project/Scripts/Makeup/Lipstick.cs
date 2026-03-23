using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Lipstick : MakeupItem
    {
        [SerializeField] private Sprite _sprite;
        
        [field: SerializeField] public Image Image { get; private set; }

        public override void ApplyEffect()
        {
            Character.ApplyLipstick(_sprite);
        }

        public void GetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }
    }
}