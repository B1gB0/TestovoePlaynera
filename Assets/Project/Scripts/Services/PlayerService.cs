using Cysharp.Threading.Tasks;
using Project.Scripts.Player;

namespace Project.Scripts.Services
{
    public class PlayerService : IPlayerService
    {
        public bool IsInitiated { get; private set; }
        public PlayerHand PlayerHand { get; private set; }
        public Character Character { get; private set; }

        public void GetPlayer(PlayerHand playerHand, Character character)
        {
            PlayerHand = playerHand;
            Character = character;
        }
    }
}