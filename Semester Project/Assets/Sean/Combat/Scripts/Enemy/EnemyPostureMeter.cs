using UnityEngine;

namespace Sean.Combat
{
    public class EnemyPostureMeter : MonoBehaviour
    {
        private int _currentPosture;
        private bool _combatActive;

        public int CurrentPosture => _currentPosture;

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
            _currentPosture = Config.PostureEnabled ? Config.MaxPosture : 0;
            if (Config.PostureEnabled)
                CombatEvents.RaisePostureChanged(_currentPosture, Config.MaxPosture);
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
        }

        private void HandleDefenseResult(CombatResult result, AttackDirection direction)
        {
            if (!_combatActive || !Config.PostureEnabled) return;

            if (result == CombatResult.Parry)
            {
                DamagePosture(Config.PostureDamageOnParry);
            }
        }

        private void HandlePlayerAttack(int damage)
        {
            if (!_combatActive || !Config.PostureEnabled) return;
            DamagePosture(Config.PostureDamageOnAttack);
        }

        private void DamagePosture(int amount)
        {
            _currentPosture = Mathf.Max(0, _currentPosture - amount);
            CombatEvents.RaisePostureChanged(_currentPosture, Config.MaxPosture);

            if (_currentPosture <= 0)
            {
                _currentPosture = Config.MaxPosture; // Reset posture after break
                CombatEvents.RaisePostureChanged(_currentPosture, Config.MaxPosture);
                CombatEvents.RaiseEnemyPostureBroken(Config.PostureStunDuration);
                CombatEvents.RaiseNotification("POSTURE BREAK!", transform.position);
            }
        }
    }
}
