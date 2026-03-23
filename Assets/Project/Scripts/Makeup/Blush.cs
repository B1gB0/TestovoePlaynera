using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Blush : MakeupItem
    {
        [SerializeField] private Sprite _sprite;
        [field: SerializeField] public Image Image { get; private set; }

        public override void ApplyEffect()
        {
            Character.ApplyBlushes(_sprite);
        }
        
        public void GetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }
    }
}