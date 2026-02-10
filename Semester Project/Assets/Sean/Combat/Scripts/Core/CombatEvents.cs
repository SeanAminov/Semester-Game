using UnityEngine;

namespace Sean.Combat
{
    public static class CombatEvents
    {
        // Enemy telegraph started (direction, telegraph duration)
        public static event System.Action<AttackDirection, float> OnEnemyTelegraph;

        // Enemy attack lands (direction)
        public static event System.Action<AttackDirection> OnEnemyAttackLand;

        // Player defensive action result
        public static event System.Action<CombatResult, AttackDirection> OnPlayerDefenseResult;

        // Player attacks enemy (damage amount)
        public static event System.Action<int> OnPlayerAttackEnemy;

        // Energy changed (fighter type, current, max)
        public static event System.Action<FighterType, int, int> OnEnergyChanged;

        // Fighter defeated
        public static event System.Action<FighterType> OnFighterDefeated;

        // Enemy stunned (duration)
        public static event System.Action<float> OnEnemyStunned;

        // Notification request (text, world position)
        public static event System.Action<string, Vector2> OnNotificationRequested;

        // Combat started
        public static event System.Action OnCombatStarted;

        // Posture changed (current, max)
        public static event System.Action<int, int> OnPostureChanged;

        // Enemy posture broken (stun duration)
        public static event System.Action<float> OnEnemyPostureBroken;

        // Crit meter changed (current, max)
        public static event System.Action<int, int> OnCritMeterChanged;

        // Crit activated
        public static event System.Action OnCritActivated;

        // Block activated
        public static event System.Action OnBlockActivated;

        // Parry drains enemy energy (drain amount)
        public static event System.Action<int> OnParryEnemyEnergyDrain;

        // Raise helpers
        public static void RaiseTelegraph(AttackDirection dir, float duration) =>
            OnEnemyTelegraph?.Invoke(dir, duration);

        public static void RaiseAttackLand(AttackDirection dir) =>
            OnEnemyAttackLand?.Invoke(dir);

        public static void RaiseDefenseResult(CombatResult result, AttackDirection dir) =>
            OnPlayerDefenseResult?.Invoke(result, dir);

        public static void RaisePlayerAttack(int damage) =>
            OnPlayerAttackEnemy?.Invoke(damage);

        public static void RaiseEnergyChanged(FighterType type, int current, int max) =>
            OnEnergyChanged?.Invoke(type, current, max);

        public static void RaiseFighterDefeated(FighterType type) =>
            OnFighterDefeated?.Invoke(type);

        public static void RaiseEnemyStunned(float duration) =>
            OnEnemyStunned?.Invoke(duration);

        public static void RaiseNotification(string text, Vector2 worldPos) =>
            OnNotificationRequested?.Invoke(text, worldPos);

        public static void RaiseCombatStarted() =>
            OnCombatStarted?.Invoke();

        public static void RaisePostureChanged(int current, int max) =>
            OnPostureChanged?.Invoke(current, max);

        public static void RaiseEnemyPostureBroken(float stunDuration) =>
            OnEnemyPostureBroken?.Invoke(stunDuration);

        public static void RaiseCritMeterChanged(int current, int max) =>
            OnCritMeterChanged?.Invoke(current, max);

        public static void RaiseCritActivated() =>
            OnCritActivated?.Invoke();

        public static void RaiseBlockActivated() =>
            OnBlockActivated?.Invoke();

        public static void RaiseParryEnemyEnergyDrain(int amount) =>
            OnParryEnemyEnergyDrain?.Invoke(amount);

        // Clear all subscribers (called on scene reload to prevent leaks)
        public static void ClearAll()
        {
            OnEnemyTelegraph = null;
            OnEnemyAttackLand = null;
            OnPlayerDefenseResult = null;
            OnPlayerAttackEnemy = null;
            OnEnergyChanged = null;
            OnFighterDefeated = null;
            OnEnemyStunned = null;
            OnNotificationRequested = null;
            OnCombatStarted = null;
            OnPostureChanged = null;
            OnEnemyPostureBroken = null;
            OnCritMeterChanged = null;
            OnCritActivated = null;
            OnBlockActivated = null;
            OnParryEnemyEnergyDrain = null;
        }
    }
}
