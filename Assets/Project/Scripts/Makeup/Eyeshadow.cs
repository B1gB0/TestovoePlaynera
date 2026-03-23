using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Eyeshadow : MakeupItem
    {
        [field: SerializeField] public Image Image { get; private set; }

        [SerializeField] private Sprite _itemSprite;

        public override void ApplyEffect()
        {
            Character.ApplyEyeshadow(MakeupSprite);
        }
        
        public void GetSprites(Sprite makeupSprite)
        {
            MakeupSprite = makeupSprite;
            ItemSprite = _itemSprite;
        }
    }
}