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
        }
    }
}
