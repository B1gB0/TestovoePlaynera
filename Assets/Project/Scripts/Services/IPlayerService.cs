using Project.Scripts.Player;

namespace Project.Scripts.Services
{
    public interface IPlayerService : IService
    {
        public PlayerHand PlayerHand { get; }
        public Character Character { get; }
    }
}