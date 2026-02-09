using UnityEngine;

namespace Sean.Combat
{
    public class EnemyEnergy : MonoBehaviour
    {
        [Header("--- Starting Energy ---")]
        [SerializeField] private int startingEnergy = 50;

        private int _currentEnergy;
        private int _maxEnergy;

        public int CurrentEnergy => _currentEnergy;
        public int MaxEnergy => _maxEnergy;

        public void Initialize()
        {
            _maxEnergy = startingEnergy;
            _currentEnergy = _maxEnergy;
            CombatEvents.RaiseEnergyChanged(FighterType.Enemy, _currentEnergy, _maxEnergy);
        }

        public void ModifyEnergy(int amount)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0, _maxEnergy);
            CombatEvents.RaiseEnergyChanged(FighterType.Enemy, _currentEnergy, _maxEnergy);

            if (_currentEnergy <= 0)
            {
                CombatEvents.RaiseFighterDefeated(FighterType.Enemy);
            }
        }
    }
}
