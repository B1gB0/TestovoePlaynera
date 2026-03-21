using System;

namespace Project.Scripts.Services
{
    public interface ICurrencyService : IService
    {
        public event Action<int> OnGoldValueChanged;
        public event Action<int> OnAlienCocoonValueChanged;
        public event Action OnAllAlienCocoonsCollected;

        public int Gold { get; }

        public void AddGold(int gold);
        public void SpendGold(int gold);
    }
}