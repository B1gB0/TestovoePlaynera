namespace Project.Scripts.Makeup
{
    public class Cream : MakeupItem
    {
        public override void ApplyEffect()
        {
            Character.RemoveAcne();
        }
    }
}