using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Eyeshadow : MakeupItem
    {
        [field: SerializeField] public Image Image { get; private set; }

        [SerializeField] private Sprite _itemSprite;
        
        public int NumberColor { get; private set; }

        public override void ApplyEffect()
        {
            Character.ApplyEyeshadow(MakeupSprite);
        }
        
        public void GetSprites(Sprite makeupSprite, int numberColor)
        {
            MakeupSprite = makeupSprite;
            ItemSprite = _itemSprite;
            NumberColor = numberColor;
        }
    }
}