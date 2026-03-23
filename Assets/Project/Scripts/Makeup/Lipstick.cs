using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Makeup
{
    public class Lipstick : MakeupItem
    {
        [field: SerializeField] public Image Image { get; private set; }

        public override void ApplyEffect()
        {
            Character.ApplyLipstick(MakeupSprite);
        }

        public void GetSprites(Sprite makeupSprite, Sprite itemSprite)
        {
            MakeupSprite = makeupSprite;
            ItemSprite = itemSprite;
        }
    }
}