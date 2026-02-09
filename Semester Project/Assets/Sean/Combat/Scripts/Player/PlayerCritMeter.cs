using UnityEngine;

namespace Sean.Combat
{
    public class PlayerCritMeter : MonoBehaviour
    {
        private int _currentMeter;
        private bool _critReady;
        private bool _combatActive;

        public int CurrentMeter => _currentMeter;
        public bool CritReady => _critReady;

        private CombatRuntimeConfig Config => CombatRuntimeConfig.Instance;

        private void OnEnable()
        {
            CombatEvents.OnPlayerDefenseResult += HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy += HandlePlayerAttack;
            CombatEvents.OnCombatStarted += OnCombatStarted;
            CombatEvents.OnFighterDefeated += OnFighterDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnPlayerDefenseResult -= HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy -= HandlePlayerAttack;
            CombatEvents.OnCombatStarted -= OnCombatStarted;
            CombatEvents.OnFighterDefeated -= OnFighterDefeated;
        }

        private void OnCombatStarted()
        {
            _combatActive = true;
            _currentMeter = 0;
            _critReady = false;
            if (Config.CritMeterEnabled)
                CombatEvents.RaiseCritMeterChanged(_currentMeter, Config.CritMeterMax);
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
        }

        private void HandleDefenseResult(CombatResult result, AttackDirection direction)
        {
            if (!_combatActive || !Config.CritMeterEnabled) return;

            if (result == CombatResult.Parry)
            {
                AddMeter(Config.CritMeterGainOnParry);
            }
            else if ((result == CombatResult.Dodge) && Config.CritDecreasesOnDefensive)
            {
                AddMeter(-Config.CritDecreaseAmount);
            }
        }

        private void HandlePlayerAttack(int damage)
        {
            if (!_combatActive || !Config.CritMeterEnabled) return;
            AddMeter(Config.CritMeterGainOnAttack);
        }

        public void OnBlockPerformed()
        {
            if (!_combatActive || !Config.CritMeterEnabled) return;
            if (Config.CritDecreasesOnDefensive)
            {
                AddMeter(-Config.CritDecreaseAmount);
            }
        }

        private void AddMeter(int amount)
        {
            if (_critReady && amount > 0) return; // Already full, don't overflow

            _currentMeter = Mathf.Clamp(_currentMeter + amount, 0, Config.CritMeterMax);
            CombatEvents.RaiseCritMeterChanged(_currentMeter, Config.CritMeterMax);

            if (_currentMeter >= Config.CritMeterMax && !_critReady)
            {
                _critReady = true;
                CombatEvents.RaiseNotification("CRIT READY!", transform.position);
            }
        }

        public void ConsumeCrit()
        {
            _critReady = false;
            _currentMeter = 0;
            CombatEvents.RaiseCritMeterChanged(_currentMeter, Config.CritMeterMax);
            CombatEvents.RaiseCritActivated();
        }
    }
}
