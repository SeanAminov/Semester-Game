using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class EnemyCombatController : MonoBehaviour
    {
        [SerializeField] private EnemyEnergy energy;
        [SerializeField] private FighterVisual visual;

        [Header("--- Attack Timing (seconds) ---")]
        [SerializeField] private float telegraphDurationMin = 0.5f;
        [SerializeField] private float telegraphDurationMax = 1.0f;
        [SerializeField] private float attackCooldownMin = 1.5f;
        [SerializeField] private float attackCooldownMax = 3.0f;
        [SerializeField] private float vulnerabilityWindow = 0.2f;

        [Header("--- Energy Costs ---")]
        [SerializeField] private int attackEnergyCost = 3;
        [SerializeField] private int attackDamage = 5;

        [Header("--- Stun ---")]
        [SerializeField] private float parryStunDuration = 1.0f;

        [Header("--- Visual ---")]
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.15f;

        private EnemyState _state = EnemyState.Idle;
        private bool _combatActive;

        private enum EnemyState { Idle, Telegraphing, Attacking, Stunned, Defeated }

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
            _state = EnemyState.Idle;
            StartCoroutine(AttackLoop());
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
            StopAllCoroutines();
            if (type == FighterType.Enemy)
                _state = EnemyState.Defeated;
        }

        private IEnumerator AttackLoop()
        {
            // Initial delay before first attack
            yield return new WaitForSeconds(1.0f);

            while (_combatActive)
            {
                // Wait for cooldown
                float cooldown = Random.Range(attackCooldownMin, attackCooldownMax);
                yield return new WaitForSeconds(cooldown);

                if (!_combatActive || _state != EnemyState.Idle) continue;

                // Pick random direction
                AttackDirection direction = (AttackDirection)Random.Range(0, 4);
                float telegraphDuration = Random.Range(telegraphDurationMin, telegraphDurationMax);

                // Telegraph phase
                _state = EnemyState.Telegraphing;
                CombatEvents.RaiseTelegraph(direction, telegraphDuration);
                yield return new WaitForSeconds(telegraphDuration);

                if (!_combatActive) yield break;

                // Attack lands
                _state = EnemyState.Attacking;
                energy.ModifyEnergy(-attackEnergyCost);
                CombatEvents.RaiseAttackLand(direction);

                // Brief vulnerability window after attack
                yield return new WaitForSeconds(vulnerabilityWindow);

                if (_state == EnemyState.Attacking)
                    _state = EnemyState.Idle;
            }
        }

        private void HandleDefenseResult(CombatResult result, AttackDirection direction)
        {
            if (result == CombatResult.Parry)
            {
                // Enemy gets stunned on parry
                StopAllCoroutines();
                StartCoroutine(StunCoroutine(parryStunDuration));
            }
        }

        private IEnumerator StunCoroutine(float duration)
        {
            _state = EnemyState.Stunned;
            CombatEvents.RaiseEnemyStunned(duration);
            visual.FlashColor(Color.white, duration);
            yield return new WaitForSeconds(duration);

            if (!_combatActive) yield break;

            _state = EnemyState.Idle;
            StartCoroutine(AttackLoop());
        }

        private void HandlePlayerAttack(int damage)
        {
            if (!_combatActive || _state == EnemyState.Defeated) return;

            energy.ModifyEnergy(-damage);
            visual.FlashColor(hitColor, hitFlashDuration);
            CombatEvents.RaiseNotification($"-{damage}", transform.position);
        }
    }
}
