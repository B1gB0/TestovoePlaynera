using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Blush : MakeupItem
    {
        [SerializeField] private Sprite _itemSprite;
        
        [field: SerializeField] public Image Image { get; private set; }

        public int NumberColor { get; private set; }

        public override void ApplyEffect()
        {
            Character.ApplyBlushes(MakeupSprite);
        }
        
        public void GetSprites(Sprite makeupSprite, int numberColor)
        {
            MakeupSprite = makeupSprite;
            ItemSprite = _itemSprite;
            NumberColor = numberColor;
        }
    }
}