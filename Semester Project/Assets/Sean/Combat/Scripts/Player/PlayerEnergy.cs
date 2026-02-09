using UnityEngine;

namespace Sean.Combat
{
    public class PlayerEnergy : MonoBehaviour
    {
        [Header("--- Starting Energy ---")]
        [SerializeField] private int startingEnergy = 20;

        private int _currentEnergy;
        private int _maxEnergy;

        public int CurrentEnergy => _currentEnergy;
        public int MaxEnergy => _maxEnergy;

        public void Initialize()
        {
            _maxEnergy = startingEnergy;
            _currentEnergy = _maxEnergy;
            CombatEvents.RaiseEnergyChanged(FighterType.Player, _currentEnergy, _maxEnergy);
        }

        public void ModifyEnergy(int amount)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0, _maxEnergy);
            CombatEvents.RaiseEnergyChanged(FighterType.Player, _currentEnergy, _maxEnergy);

            if (_currentEnergy <= 0)
            {
                CombatEvents.RaiseFighterDefeated(FighterType.Player);
            }
        }

        public bool HasEnergy(int amount)
        {
            return _currentEnergy >= amount;
        }
    }
}
